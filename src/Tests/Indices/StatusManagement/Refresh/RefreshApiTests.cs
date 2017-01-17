﻿using System;
using Elasticsearch252.Net;
using Nest252;
using Tests.Framework;
using Tests.Framework.Integration;
using Xunit;
using static Nest252.Infer;

namespace Tests.Indices.StatusManagement.Refresh
{
	public class RefreshApiTests
		: ApiIntegrationAgainstNewIndexTestBase
			<IntrusiveOperationCluster, IRefreshResponse, IRefreshRequest, RefreshDescriptor, RefreshRequest>
	{
		public RefreshApiTests(IntrusiveOperationCluster cluster, EndpointUsage usage) : base(cluster, usage) { }

		protected override LazyResponses ClientUsage() => Calls(
			fluent: (client, f) => client.Refresh(CallIsolatedValue, f),
			fluentAsync: (client, f) => client.RefreshAsync(CallIsolatedValue, f),
			request: (client, r) => client.Refresh(r),
			requestAsync: (client, r) => client.RefreshAsync(r)
		);

		protected override bool ExpectIsValid => true;
		protected override int ExpectStatusCode => 200;
		protected override HttpMethod HttpMethod => HttpMethod.POST;
		protected override string UrlPath => $"/{CallIsolatedValue}/_refresh?allow_no_indices=true";

		protected override Func<RefreshDescriptor, IRefreshRequest> Fluent => d => d.AllowNoIndices();

		protected override RefreshRequest Initializer => new RefreshRequest(CallIsolatedValue) { AllowNoIndices = true };
	}
}
