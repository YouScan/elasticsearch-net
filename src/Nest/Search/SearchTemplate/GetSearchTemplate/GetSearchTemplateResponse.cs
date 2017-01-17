﻿using Newtonsoft.Json;

namespace Nest252
{
	public interface IGetSearchTemplateResponse : IResponse
	{
		string Template { get; set; }
	}

	[JsonObject]
	public class GetSearchTemplateResponse : ResponseBase, IGetSearchTemplateResponse
	{
		[JsonProperty("template")]
		public string Template { get; set; }
	}
}
