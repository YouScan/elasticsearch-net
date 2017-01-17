﻿using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Nest252
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum LicenseStatus
	{
		[EnumMember(Value="active")]
		Active,
		[EnumMember(Value="invalid")]
		Invalid,
		[EnumMember(Value="expired")]
		Expired
	}
}
