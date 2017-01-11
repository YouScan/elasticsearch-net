﻿using Newtonsoft.Json;

namespace Nest500
{
	[JsonObject]
	public class ClusterRerouteParameters
	{
		[JsonProperty("index")]
		public string Index { get; set; }

		[JsonProperty("shard")]
		public int Shard { get; set; }

		[JsonProperty("from_node")]
		public string FromNode { get; set; }

		[JsonProperty("to_node")]
		public string ToNode { get; set; }

		[JsonProperty("node")]
		public string Node { get; set; }

		[JsonProperty("allow_primary")]
		public bool? AllowPrimary { get; set; }
	}
}
