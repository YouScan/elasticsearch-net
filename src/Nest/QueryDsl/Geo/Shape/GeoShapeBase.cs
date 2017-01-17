﻿using Newtonsoft.Json;

namespace Nest252
{
	public interface IGeoShape
	{
		[JsonProperty("type")]
		string Type { get; }
	}

	public abstract class GeoShapeBase : IGeoShape
	{
	    protected GeoShapeBase(string type)
		{
			this.Type = type;
		}

		public string Type { get; protected set; }
	}
}
