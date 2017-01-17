﻿using System;
using Newtonsoft.Json;

namespace Nest252
{
	internal class FuzzinessJsonConverter : JsonConverter
	{
		public override bool CanWrite => true;
		public override bool CanConvert(Type objectType) => true;
		public override bool CanRead => true;

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var v = value as IFuzziness;
			if (v.Auto) writer.WriteValue("AUTO");
			else if (v.EditDistance.HasValue) writer.WriteValue(v.EditDistance.Value);
			else writer.WriteNull();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.String)
				return Fuzziness.Auto;
			if (reader.TokenType == JsonToken.Integer)
			{
				var editDistance = Convert.ToInt32(reader.Value);
				return Fuzziness.EditDistance(editDistance);
			}
			return null;
		}

	}
}
