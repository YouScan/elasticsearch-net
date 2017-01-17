﻿using System.Threading.Tasks;
using Nest252;
using Tests.Framework;
using static Tests.Framework.UrlTester;

namespace Tests.Indices.Warmers.GetWarmer
{
	public class GetWarmerUrlTests
	{
		[U] public async Task Urls()
		{
			var name = "id";

			await GET($"/_warmer")
				.Fluent(c => c.GetWarmer())
				.Request(c => c.GetWarmer(new GetWarmerRequest()))
				.FluentAsync(c => c.GetWarmerAsync())
				.RequestAsync(c => c.GetWarmerAsync(new GetWarmerRequest()))
				;

			var index = "indexx";
			await GET($"/{index}/_warmer")
				.Fluent(c => c.GetWarmer(w=>w.Index(Nest252.Indices.Index(index))))
				.Request(c => c.GetWarmer(new GetWarmerRequest(Nest252.Indices.Index(index))))
				.FluentAsync(c => c.GetWarmerAsync(w=>w.Index(index)))
				.RequestAsync(c => c.GetWarmerAsync(new GetWarmerRequest(Nest252.Indices.Index(index))))
				;

			await GET($"/{index}/_warmer/{name}")
				.Fluent(c => c.GetWarmer(w=>w.Index(index).Name(name)))
				.Request(c => c.GetWarmer(new GetWarmerRequest(index, name)))
				.FluentAsync(c => c.GetWarmerAsync(w=>w.Index(index).Name(name)))
				.RequestAsync(c => c.GetWarmerAsync(new GetWarmerRequest(index, name)))
				;

			await GET($"/_warmer/{name}")
				.Fluent(c => c.GetWarmer(w=>w.Name(name)))
				.Request(c => c.GetWarmer(new GetWarmerRequest((Name)name)))
				.FluentAsync(c => c.GetWarmerAsync(w=>w.Name(name)))
				.RequestAsync(c => c.GetWarmerAsync(new GetWarmerRequest((Name)name)))
				;
		}
	}
}
