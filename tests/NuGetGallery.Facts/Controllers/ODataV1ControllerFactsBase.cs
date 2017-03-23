// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using NuGetGallery.Configuration;
using NuGetGallery.OData;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.OData.Query;

namespace NuGetGallery.Controllers
{
    public abstract class ODataV1ControllerFactsBase
        : ODataFeedControllerFactsBase<ODataV1FeedController>
    {
        protected override ODataV1FeedController CreateController(IEntityRepository<Package> packagesRepository,
            IGalleryConfigurationService configurationService, ISearchService searchService)
        {
            return new ODataV1FeedController(packagesRepository, configurationService, searchService);
        }

        protected async Task<IReadOnlyCollection<V1FeedPackage>> GetCollection(
            Func<ODataV1FeedController, ODataQueryOptions<V1FeedPackage>, IHttpActionResult> controllerAction,
            string requestPath)
        {
            var queryResult = InvokeODataFeedControllerAction(controllerAction, requestPath);

            return await GetValueFromQueryResult(queryResult);
        }

        protected async Task<int> GetInt(
            Func<ODataV1FeedController, ODataQueryOptions<V1FeedPackage>, IHttpActionResult> controllerAction,
            string requestPath)
        {
            var queryResult = InvokeODataFeedControllerAction(controllerAction, requestPath);

            return int.Parse(await GetValueFromQueryResult(queryResult));
        }

        protected async Task<IReadOnlyCollection<V1FeedPackage>> GetCollectionAsync(
            Func<ODataV1FeedController, ODataQueryOptions<V1FeedPackage>, Task<IHttpActionResult>> asyncControllerAction,
            string requestPath)
        {
            var queryResult = await InvokeODataFeedControllerActionAsync(asyncControllerAction, requestPath);

            return await GetValueFromQueryResult(queryResult);
        }

        protected async Task<int> GetIntAsync(
            Func<ODataV1FeedController, ODataQueryOptions<V1FeedPackage>, Task<IHttpActionResult>> asyncControllerAction,
            string requestPath)
        {
            var queryResult = await InvokeODataFeedControllerActionAsync(asyncControllerAction, requestPath);

            return int.Parse(await GetValueFromQueryResult(queryResult));
        }
    }
}