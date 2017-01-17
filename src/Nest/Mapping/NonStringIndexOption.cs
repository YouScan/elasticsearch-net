﻿using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Nest252
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum NonStringIndexOption
	{
		[EnumMember(Value = "not_analyzed")]
		NotAnalyzed,
		[EnumMember(Value = "no")]
		No
	}
}
