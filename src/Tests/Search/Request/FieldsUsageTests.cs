﻿using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Nest252;
using Tests.Framework;
using Tests.Framework.Integration;
using Tests.Framework.MockData;
using static Nest252.Infer;

namespace Tests.Search.Request
{
	/** Allows to selectively load specific stored fields for each document represented by a search hit.
	*
	* WARNING: The `fields` parameter is about fields that are explicitly marked as stored in the mapping,
	* which is off by default and generally not recommended.
	* Use <<source-filtering-usage,source filtering>> instead to select subsets of the original source document to be returned.
	*
	* See the Elasticsearch documentation on {ref_current}/search-request-fields.html[Fields] for more detail.
	*/
	public class FieldsUsageTests : SearchUsageTestBase
	{
		public FieldsUsageTests(ReadOnlyCluster cluster, EndpointUsage usage) : base(cluster, usage) { }

		protected override object ExpectJson => new
		{
			fields = new[] { "name", "startedOn", "dateString" }
		};

		protected override Func<SearchDescriptor<Project>, ISearchRequest> Fluent => s => s
			.Fields(fs => fs
				.Field(p => p.Name)
				.Field(p => p.StartedOn)
				.Field(p => p.DateString)
			);

		protected override SearchRequest<Project> Initializer =>
			new SearchRequest<Project>
			{
				Fields = Fields<Project>(p => p.Name, p => p.StartedOn, p => p.DateString)
			};

		[I] protected Task FieldsAreReturned() => this.AssertOnAllResponses(r =>
		{
			r.Fields.Should().NotBeNull();
			r.Fields.Count().Should().BeGreaterThan(0);
			foreach (var fieldValues in r.Fields)
			{
				fieldValues.Count().Should().Be(3);
				var name = fieldValues.Value<string>(Field<Project>(p => p.Name));
				name.Should().NotBeNullOrWhiteSpace();

				var dateTime = fieldValues.ValueOf<Project, DateTime>(p => p.StartedOn);
				dateTime.Should().BeAfter(default(DateTime));

				var dateTimeAsString = fieldValues.ValueOf<Project, string>(p => p.DateString);
				dateTimeAsString.Should().NotContain("/")
					.And.MatchRegex(@"^\d\d\d\d-\d\d-\d\dT\d\d:\d\d:\d\d.*$");
			}
		});
	}
}
