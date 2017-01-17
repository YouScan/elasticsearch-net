﻿using Elasticsearch252.Net;
using FluentAssertions;
using Nest252;
using Tests.Framework;
using Tests.Framework.Integration;
using Xunit;

namespace Tests.Cat.CatNodes
{
	public class CatNodesApiTests : ApiIntegrationTestBase<ReadOnlyCluster, ICatResponse<CatNodesRecord>, ICatNodesRequest, CatNodesDescriptor, CatNodesRequest>
	{
		public CatNodesApiTests(ReadOnlyCluster cluster, EndpointUsage usage) : base(cluster, usage) { }
		protected override LazyResponses ClientUsage() => Calls(
			fluent: (client, f) => client.CatNodes(),
			fluentAsync: (client, f) => client.CatNodesAsync(),
			request: (client, r) => client.CatNodes(r),
			requestAsync: (client, r) => client.CatNodesAsync(r)
		);

		protected override bool ExpectIsValid => true;
		protected override int ExpectStatusCode => 200;
		protected override HttpMethod HttpMethod => HttpMethod.GET;
		protected override string UrlPath => "/_cat/nodes";

		protected override void ExpectResponse(ICatResponse<CatNodesRecord> response)
		{
			response.Records.Should().NotBeEmpty().And.Contain(a => !string.IsNullOrEmpty(a.Name));
		}
	}
}
