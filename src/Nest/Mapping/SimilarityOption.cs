﻿using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Nest252
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum SimilarityOption
	{
		[EnumMember(Value = "default")]
		Default,
		[EnumMember(Value = "BM25")]
		BM25
	}
}
