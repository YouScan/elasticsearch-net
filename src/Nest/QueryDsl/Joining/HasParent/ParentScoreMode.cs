﻿using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Nest252
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum ParentScoreMode
	{
		[EnumMember(Value = "none")]
		None = 0,
		[EnumMember(Value = "score")]
		Score
	}
}
