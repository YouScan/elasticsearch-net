﻿using System;
using System.Collections.Generic;
using System.Linq;
using Elasticsearch252.Net;
using FluentAssertions;
using Nest252;
using Tests.Framework;
using Tests.Framework.Integration;
using Xunit;

namespace Tests.Indices.AliasManagement.GetAlias
{
	public class GetAliasApiTests : ApiIntegrationTestBase<ReadOnlyCluster, IGetAliasesResponse, IGetAliasRequest, GetAliasDescriptor, GetAliasRequest>
	{
		private static readonly Names Names = Infer.Names("projects-alias", "alias, x", "y");

		public GetAliasApiTests(ReadOnlyCluster cluster, EndpointUsage usage) : base(cluster, usage) { }

		protected override LazyResponses ClientUsage() => Calls(
			fluent: (client, f) => client.GetAlias(f),
			fluentAsync: (client, f) => client.GetAliasAsync(f),
			request: (client, r) => client.GetAlias(r),
			requestAsync: (client, r) => client.GetAliasAsync(r)
		);

		protected override bool ExpectIsValid => true;
		protected override int ExpectStatusCode => 200;
		protected override HttpMethod HttpMethod => HttpMethod.GET;
		protected override string UrlPath => $"/_all/_alias/projects-alias%2Calias%2Cx%2Cy";
		protected override void ExpectResponse(IGetAliasesResponse response)
		{
			response.Indices.Should().NotBeNull().And.ContainKey("project");
			var projectIndex = response.Indices["project"];
			projectIndex.Should().HaveCount(1);
			projectIndex.First().Name.Should().Be("projects-alias");
		}
		protected override bool SupportsDeserialization => false;

		protected override Func<GetAliasDescriptor, IGetAliasRequest> Fluent => d=>d
			.AllIndices()
			.Name(Names)
		;
		protected override GetAliasRequest Initializer => new GetAliasRequest(Infer.AllIndices, Names);
	}
}
