﻿using System.Threading.Tasks;
using Nest252;
using Tests.Framework;
using Tests.Framework.MockData;
using static Nest252.Infer;
using static Tests.Framework.UrlTester;

namespace Tests.Indices.Warmers.DeleteWarmer
{
	public class DeleteWarmerUrlTests
	{
		[U] public async Task Urls()
		{
			var name = "id";

			await DELETE($"/_all/_warmer/{name}")
				.Fluent(c => c.DeleteWarmer(AllIndices, name))
				.Request(c => c.DeleteWarmer(new DeleteWarmerRequest(AllIndices, name)))
				.FluentAsync(c => c.DeleteWarmerAsync(AllIndices, name))
				.RequestAsync(c => c.DeleteWarmerAsync(new DeleteWarmerRequest(AllIndices, name)))
				;

			await DELETE($"/project/_warmer/{name}")
				.Fluent(c => c.DeleteWarmer(Index<Project>(), name))
				.Request(c => c.DeleteWarmer(new DeleteWarmerRequest(Index<Project>(), name)))
				.FluentAsync(c => c.DeleteWarmerAsync(Index<Project>(), name))
				.RequestAsync(c => c.DeleteWarmerAsync(new DeleteWarmerRequest(Index<Project>(), name)))
				;
		}
	}
}
