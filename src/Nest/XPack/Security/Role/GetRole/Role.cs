using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nest252
{
	public class Role
	{
		[JsonProperty("cluster")]
		public IEnumerable<string> Cluster { get; set; }

		[JsonProperty("run_as")]
		public IEnumerable<string> RunAs { get; set; }

		[JsonProperty("indices")]
		public IEnumerable<IIndicesPrivileges> Indices { get; set; }

		[JsonProperty("metadata")]
		public IDictionary<string, object> Metadata { get; internal set; }
	}
}
