﻿using Elasticsearch252.Net;
using Nest252;
using Tests.Framework;
using Tests.Framework.Integration;
using Xunit;

namespace Tests.Cat.CatThreadPool
{
	public class CatThreadPoolApiTests : ApiIntegrationTestBase<ReadOnlyCluster, ICatResponse<CatThreadPoolRecord>, ICatThreadPoolRequest, CatThreadPoolDescriptor, CatThreadPoolRequest>
	{
		public CatThreadPoolApiTests(ReadOnlyCluster cluster, EndpointUsage usage) : base(cluster, usage) { }
		protected override LazyResponses ClientUsage() => Calls(
			fluent: (client, f) => client.CatThreadPool(),
			fluentAsync: (client, f) => client.CatThreadPoolAsync(),
			request: (client, r) => client.CatThreadPool(r),
			requestAsync: (client, r) => client.CatThreadPoolAsync(r)
		);

		protected override bool ExpectIsValid => true;
		protected override int ExpectStatusCode => 200;
		protected override HttpMethod HttpMethod => HttpMethod.GET;
		protected override string UrlPath => "/_cat/thread_pool";

	}
}
