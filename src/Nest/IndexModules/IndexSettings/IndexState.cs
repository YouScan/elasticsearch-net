﻿using Newtonsoft.Json;

namespace Nest252
{
	[JsonConverter(typeof(ReadAsTypeJsonConverter<IndexState>))]
	public interface IIndexState
	{
		[JsonProperty("settings")]
		IIndexSettings Settings { get; set; }

		[JsonProperty("aliases")]
		IAliases Aliases { get; set; }

		[JsonProperty("warmers")]
		IWarmers Warmers { get; set; }

		[JsonProperty("mappings")]
		IMappings Mappings { get; set; }

		[JsonProperty("similarity")]
		ISimilarities Similarity { get; set; }
	}
	public class IndexState : IIndexState
	{
		public IIndexSettings Settings { get; set; }
		
		public IMappings Mappings { get; set; }

		public IAliases Aliases { get; set; }
			
		public IWarmers Warmers { get; set; }

		public ISimilarities Similarity { get; set; }
	}





}