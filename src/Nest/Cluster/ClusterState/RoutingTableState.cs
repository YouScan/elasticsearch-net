using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nest252
{
	public class RoutingTableState
	{
		[JsonProperty("indices")]
		[JsonConverter(typeof(VerbatimDictionaryKeysJsonConverter))]
		public Dictionary<string, IndexRoutingTable> Indices { get; internal set; }
	}
}