﻿using Newtonsoft.Json;

namespace Nest252
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	[JsonConverter(typeof(ReadAsTypeJsonConverter<SuggestBucket>))]
	public interface ISuggestBucket
	{
		[JsonProperty("text")]
		string Text { get; set; }

		[JsonProperty("term")]
		ITermSuggester Term { get; set; }

		[JsonProperty("phrase")]
		IPhraseSuggester Phrase { get; set; }

		[JsonProperty("completion")]
		ICompletionSuggester Completion { get; set; }
	}

	public class SuggestBucket : ISuggestBucket
	{
		public string Text { get; set; }

		public ITermSuggester Term { get; set; }

		public IPhraseSuggester Phrase { get; set; }

		public ICompletionSuggester Completion { get; set; }
	}
}
