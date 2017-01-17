using Newtonsoft.Json;

namespace Nest252
{
	[JsonObject]
	public class Throwable
	{
		[JsonProperty("type")]
		public string Type { get; internal set; }

		[JsonProperty("reason")]
		public string Reason { get; internal set; }

		[JsonProperty("caused_by")]
		public Throwable CausedBy { get; internal set; }
	}
}
