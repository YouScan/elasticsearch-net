﻿using System;
using FluentAssertions;
using Nest252;
using Tests.Framework;
using Tests.Framework.Integration;
using Tests.Framework.MockData;
using static Nest252.Infer;

namespace Tests.Aggregations.Bucket.Missing
{
	public class MissingAggregationUsageTests : AggregationUsageTestBase
	{
		public MissingAggregationUsageTests(ReadOnlyCluster i, EndpointUsage usage) : base(i, usage) { }

		protected override object ExpectJson => new
		{
			aggs = new
			{
				projects_without_a_description = new
				{
					missing = new
					{
						field = "description"
					}
				}
			}
		};

		protected override Func<SearchDescriptor<Project>, ISearchRequest> Fluent => s => s
			.Aggregations(a => a
				.Missing("projects_without_a_description", m => m
					.Field(p => p.Description)
				)
			);

		protected override SearchRequest<Project> Initializer =>
			new SearchRequest<Project>
			{
				Aggregations = new MissingAggregation("projects_without_a_description")
				{
					Field = Field<Project>(p => p.Description)
				}
			};

		protected override void ExpectResponse(ISearchResponse<Project> response)
		{
			response.ShouldBeValid();
			var projectsWithoutDesc = response.Aggs.Missing("projects_without_a_description");
			projectsWithoutDesc.Should().NotBeNull();
		}
	}
}
