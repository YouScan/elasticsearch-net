﻿using System.Threading.Tasks;
using Nest252;
using Tests.Framework;
using Tests.Framework.MockData;
using static Nest252.Indices;
using static Nest252.Types;
using static Tests.Framework.UrlTester;

namespace Tests.Indices.IndexManagement.TypeExists
{
	public class TypeExistsUrlTests
	{
		[U] public async Task Urls()
		{
			var indices = Index<Project>().And<CommitActivity>();
			var index = "project";
			var types = Type<Project>().And<CommitActivity>();
			var type = "project%2Ccommits";
			await HEAD($"/{index}/{type}")
				.Fluent(c => c.TypeExists(indices, types))
				.Request(c => c.TypeExists(new TypeExistsRequest(indices, types)))
				.FluentAsync(c => c.TypeExistsAsync(indices, types))
				.RequestAsync(c => c.TypeExistsAsync(new TypeExistsRequest(indices, types)))
				;

		}
	}
}
