﻿namespace Nest500
{
	public class Murmur3HashAttribute : ElasticsearchDocValuesPropertyAttributeBase, IMurmur3HashProperty
	{
		public Murmur3HashAttribute() : base("murmur3") { }
	}
}
