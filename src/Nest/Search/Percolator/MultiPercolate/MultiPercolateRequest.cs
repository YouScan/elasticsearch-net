﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nest500
{
	[JsonConverter(typeof(MultiPercolateJsonConverter))]
	[Obsolete("Deprecated. Will be removed in the next major release. Use a percolate query with multi search api")]
	public partial interface IMultiPercolateRequest
	{
		IList<IPercolateOperation> Percolations { get; set; }
	}

	[Obsolete("Deprecated. Will be removed in the next major release. Use a percolate query with multi search api")]
	public partial class MultiPercolateRequest
	{
		public IList<IPercolateOperation> Percolations { get; set; }
	}

	[DescriptorFor("Mpercolate")]
	[Obsolete("Deprecated. Will be removed in the next major release. Use a percolate query with multi search api")]
	public partial class MultiPercolateDescriptor
	{
		IList<IPercolateOperation> IMultiPercolateRequest.Percolations { get; set; } = new SynchronizedCollection<IPercolateOperation>();

		public MultiPercolateDescriptor Percolate<T>(Func<PercolateDescriptor<T>, IPercolateOperation> percolateSelector)
			where T : class
		{
			var percolation = percolateSelector.InvokeOrDefault((new PercolateDescriptor<T>(typeof(T), typeof(T))));
			Self.Percolations.Add(percolation);
			return this;
		}

		public MultiPercolateDescriptor PercolateMany<T>(IEnumerable<T> sources, Func<PercolateDescriptor<T>, T, IPercolateOperation> percolateSelector)
			where T : class
		{
			foreach (var source in sources)
			{
				var percolation = percolateSelector.InvokeOrDefault(new PercolateDescriptor<T>(typeof(T), typeof(T)), source);
				Self.Percolations.Add(percolation);
			}
			return this;
		}

		public MultiPercolateDescriptor Count<T>(Func<PercolateCountDescriptor<T>, IPercolateOperation> countSelector)
			where T : class
		{
			var percolation = countSelector.InvokeOrDefault(new PercolateCountDescriptor<T>(typeof(T), typeof(T)));
			Self.Percolations.Add(percolation);
			return this;
		}
	}
}
