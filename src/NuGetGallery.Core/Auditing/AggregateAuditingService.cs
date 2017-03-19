// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NuGetGallery.Auditing
{
    /// <summary>
    /// An auditing service that aggregates multiple auditing services.
    /// </summary>
    public sealed class AggregateAuditingService : IAuditingService
    {
        private readonly IAuditingService[] _services;
        private readonly IAuditingService _defaultAuditingService;
        private readonly IAuditingService _ifxAuditingService;

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="services">An enumerable of <see cref="IAuditingService" /> instances.</param>
        /// <exception cref="ArgumentNullException">Thrown if <see cref="services" /> is <c>null</c>.</exception>
        public AggregateAuditingService(IEnumerable<IAuditingService> services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            _services = services.ToArray();

            Func<IAuditingService, bool> isIfxAuditingService = service => service.GetType().Name == "IfxAuditingService";

            _defaultAuditingService = services.FirstOrDefault(service => !isIfxAuditingService(service));
            _ifxAuditingService = services.SingleOrDefault(isIfxAuditingService);
        }

        /// <summary>
        /// Persists the audit record to storage.
        /// </summary>
        /// <param name="record">An audit record.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous save operation.</returns> 
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="record" /> is <c>null</c>.</exception>
        public async Task SaveAuditRecordAsync(AuditRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            var auditTiming = new AuditTiming(record.GetType().Name);

            var tasks = new[]
            {
                TimeAsync(_defaultAuditingService, record, auditTiming.MarkDefaultAuditingServiceEnd),
                TimeAsync(_ifxAuditingService, record, auditTiming.MarkIfxAuditingServiceEnd)
            };

            await Task.WhenAll(tasks);

            auditTiming.MarkTotalEnd();

            auditTiming.WriteToFile();
        }

        private static async Task TimeAsync(IAuditingService service, AuditRecord record, Action markDone)
        {
            if (service != null)
            {
                await service.SaveAuditRecordAsync(record);
            }

            markDone();
        }
    }
}