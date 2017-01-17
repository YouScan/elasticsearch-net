﻿using System;
using Elasticsearch252.Net;
using Nest252;
using Tests.Framework;
using Tests.Framework.Integration;
using Xunit;
using static Nest252.Infer;

namespace Tests.Indices.StatusManagement.ClearCache
{
	public class ClearCacheApiTests : ApiIntegrationTestBase<ReadOnlyCluster, IClearCacheResponse, IClearCacheRequest, ClearCacheDescriptor, ClearCacheRequest>
	{
		public ClearCacheApiTests(ReadOnlyCluster cluster, EndpointUsage usage) : base(cluster, usage) { }

		protected override LazyResponses ClientUsage() => Calls(
			fluent: (client, f) => client.ClearCache(AllIndices, f),
			fluentAsync: (client, f) => client.ClearCacheAsync(AllIndices, f),
			request: (client, r) => client.ClearCache(r),
			requestAsync: (client, r) => client.ClearCacheAsync(r)
		);

		protected override bool ExpectIsValid => true;
		protected override int ExpectStatusCode => 200;
		protected override HttpMethod HttpMethod => HttpMethod.POST;
		protected override string UrlPath => "/_cache/clear?recycler=true";

		protected override Func<ClearCacheDescriptor, IClearCacheRequest> Fluent => d => d.Recycler();

		protected override ClearCacheRequest Initializer => new ClearCacheRequest(AllIndices) { Recycler = true };
	}
}
