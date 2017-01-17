﻿using System;
using Newtonsoft.Json;

namespace Nest252
{
	[JsonConverter(typeof(SourceFilterJsonConverter))]
	public interface ISourceFilter
	{
		[JsonProperty("include")]
		Fields Include { get; set; }

		[JsonProperty("exclude")]
		Fields Exclude { get; set; }

		[JsonIgnore]
		bool Disable { get; set; }
	}

	public class SourceFilter : ISourceFilter
	{
		public static SourceFilter ExcludeAll { get; } = new SourceFilter { Exclude = new [] {"*"} };
		public static SourceFilter IncludeAll { get; } = new SourceFilter { Include = new [] {"*"} };

		public Fields Include { get; set; }
		public Fields Exclude { get; set; }

		public bool Disable { get; set; }
	}

	public class SourceFilterDescriptor<T> : DescriptorBase<SourceFilterDescriptor<T>, ISourceFilter>, ISourceFilter
		where T : class
	{
		Fields ISourceFilter.Include { get; set; }

		Fields ISourceFilter.Exclude { get; set; }

		bool ISourceFilter.Disable { get; set; }


		public SourceFilterDescriptor<T> Include(Func<FieldsDescriptor<T>, IPromise<Fields>> fields) =>
			Assign(a => a.Include = fields?.Invoke(new FieldsDescriptor<T>())?.Value);

		public SourceFilterDescriptor<T> IncludeAll() => Assign(a => a.Include = new[] { "*" });

		public SourceFilterDescriptor<T> Exclude(Func<FieldsDescriptor<T>, IPromise<Fields>> fields) =>
			Assign(a => a.Exclude = fields?.Invoke(new FieldsDescriptor<T>())?.Value);

		public SourceFilterDescriptor<T> ExcludeAll() => Assign(a => a.Exclude = new[] { "*" });

		public SourceFilterDescriptor<T> Disable(bool disable = true) => Assign(a => a.Disable = disable);
	}
}
