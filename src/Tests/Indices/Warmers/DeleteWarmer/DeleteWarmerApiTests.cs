﻿using System;
using Elasticsearch252.Net;
using Nest252;
using Tests.Framework;
using Tests.Framework.Integration;
using Tests.Framework.MockData;
using Xunit;
using static Nest252.Infer;

namespace Tests.Indices.Warmers.DeleteWarmer
{
	public class DeleteWarmerApiTests : ApiTestBase<IntrusiveOperationCluster, IDeleteWarmerResponse, IDeleteWarmerRequest, DeleteWarmerDescriptor, DeleteWarmerRequest>
	{
		public DeleteWarmerApiTests(IntrusiveOperationCluster cluster, EndpointUsage usage) : base(cluster, usage) { }

		protected override LazyResponses ClientUsage() => Calls(
			fluent: (client, f) => client.DeleteWarmer(Index<Project>(), "warmer1,warmer2", f),
			fluentAsync: (client, f) => client.DeleteWarmerAsync(Index<Project>(), "warmer1,warmer2", f),
			request: (client, r) => client.DeleteWarmer(r),
			requestAsync: (client, r) => client.DeleteWarmerAsync(r)
		);

		protected override HttpMethod HttpMethod => HttpMethod.DELETE;
		protected override string UrlPath => "/project/_warmer/warmer1%2Cwarmer2";

		protected override DeleteWarmerDescriptor NewDescriptor() => new DeleteWarmerDescriptor(Index<Project>(), "warmer1,warmer2");

		protected override Func<DeleteWarmerDescriptor, IDeleteWarmerRequest> Fluent => d => d;

		protected override DeleteWarmerRequest Initializer => new DeleteWarmerRequest(Index<Project>(), Names("warmer1", "warmer2"));
	}
}
