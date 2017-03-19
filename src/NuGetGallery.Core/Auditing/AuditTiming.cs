// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.IO;
using System.Web.Hosting;

namespace NuGetGallery.Auditing
{
    internal sealed class AuditTiming
    {
        private readonly Stopwatch _stopwatch;

        internal DateTimeOffset Start { get; }
        internal string AuditRecordType { get; set; }
        internal TimeSpan DefaultAuditingServiceDuration { get; private set; }
        internal TimeSpan IfxAuditingServiceDuration { get; private set; }
        internal TimeSpan TotalDuration { get; private set; }

        internal AuditTiming(string auditRecordType)
        {
            AuditRecordType = auditRecordType;
            Start = DateTimeOffset.UtcNow;
            _stopwatch = Stopwatch.StartNew();
        }

        internal void MarkDefaultAuditingServiceEnd()
        {
            DefaultAuditingServiceDuration = _stopwatch.Elapsed;
        }

        internal void MarkIfxAuditingServiceEnd()
        {
            IfxAuditingServiceDuration = _stopwatch.Elapsed;
        }

        internal void MarkTotalEnd()
        {
            TotalDuration = _stopwatch.Elapsed;

            _stopwatch.Stop();
        }

        internal void WriteToFile()
        {
            var filePath = Path.Combine(HostingEnvironment.MapPath("~/App_Data") ?? ".", "AuditTimings.csv");

            using (var writer = new StreamWriter(filePath, append: true))
            {
                writer.WriteLine($"{Start},{AuditRecordType},{DefaultAuditingServiceDuration},{IfxAuditingServiceDuration},{TotalDuration}");

                writer.Flush();
            }
        }
    }
}