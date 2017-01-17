﻿using System;
using Nest252;

namespace Tests.Mapping.Types.Specialized.Completion
{
	public class CompletionTest
	{
		[Completion(
			SearchAnalyzer = "mysearchanalyzer",
			Analyzer = "myanalyzer",
			Payloads = true,
			PreserveSeparators = true,
			PreservePositionIncrements = true,
			MaxInputLength = 20)]
		public CompletionField<object> Full { get; set; }

		[Completion]
		public CompletionField<object> Minimal { get; set; }

        public CompletionField<object> Inferred { get; set; } 
	}

	public class CompletionMappingTests : TypeMappingTestBase<CompletionTest>
	{
		protected override object ExpectJson => new
		{
			properties = new
			{
				full = new
				{
					type = "completion",
					search_analyzer = "mysearchanalyzer",
					analyzer = "myanalyzer",
					payloads = true,
					preserve_separators = true,
					preserve_position_increments = true,
					max_input_length = 20
				},
				minimal = new
				{
					type = "completion"
				},
                inferred = new
                {
                    type = "completion"
                }
            }
		};

	    protected override Func<PropertiesDescriptor<CompletionTest>, IPromise<IProperties>> FluentProperties => p => p
	        .Completion(s => s
	            .Name(o => o.Full)
	            .Analyzer("myanalyzer")
	            .SearchAnalyzer("mysearchanalyzer")
	            .Payloads()
	            .PreserveSeparators()
	            .PreservePositionIncrements()
	            .MaxInputLength(20)
	        )
	        .Completion(b => b
	            .Name(o => o.Minimal)
	        )
	        .Completion(b => b
	            .Name(o => o.Inferred)
	        );
	}
}
