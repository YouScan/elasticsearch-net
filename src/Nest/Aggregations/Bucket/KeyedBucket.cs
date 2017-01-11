﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nest500
{
	public class KeyedBucket<TKey> : BucketBase
	{
		public KeyedBucket() { }
		public KeyedBucket(IDictionary<string, IAggregate> aggregations) : base(aggregations) { }

		public TKey Key { get; set; }
		public string KeyAsString { get; set; }
		public long? DocCount { get; set; }
	}
}
