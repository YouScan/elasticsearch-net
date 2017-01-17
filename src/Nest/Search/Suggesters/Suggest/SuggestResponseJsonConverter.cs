﻿using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Nest252
{
	internal class SuggestResponseJsonConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType) => true;

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotSupportedException();
		}
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var response = new SuggestResponse();
			var jsonObject = JObject.Load(reader);
			foreach (var prop in jsonObject.Properties())
			{
				if (prop.Name == "_shards")
					response.Shards = prop.Value.ToObject<ShardsMetaData>();
				else 
					response.Suggestions.Add(prop.Name, prop.Value.ToObject<Suggest[]>());
			}

			return response;
		}

	}
}
