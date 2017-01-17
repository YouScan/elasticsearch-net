﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Elasticsearch252.Net;
using Newtonsoft.Json;

namespace Nest252
{
	public class FieldMappingProperties : Dictionary<string, FieldMapping> { }

	public class TypeFieldMappings
	{
		[JsonProperty("mappings")]
		public Dictionary<string, FieldMappingProperties> Mappings { get; set; }
	}

	public class FieldMapping
	{
		[JsonProperty("full_name")]
		public string FullName { get; set; }

		[JsonProperty("mapping")]
		[JsonConverter(typeof(FieldMappingJsonConverter))]
		public Dictionary<string, IFieldMapping> Mapping { get; set; }
	}

	public class IndexFieldMappings : Dictionary<string, TypeFieldMappings> { }

	public interface IGetFieldMappingResponse : IResponse
	{
		IndexFieldMappings Indices { get; set; }

		IFieldMapping MappingFor(string indexName, string typeName, string fieldName);

		IFieldMapping MappingFor<T>(string fieldName)
			where T : class;

		IFieldMapping MappingFor<T>(Expression<Func<T, object>> fieldName)
			where T : class;

		FieldMappingProperties MappingsFor<T>(string indexName = null, string typeName = null)
			where T : class;

		FieldMappingProperties MappingsFor(string indexName, string typeName);
	}

	public class GetFieldMappingResponse : ResponseBase, IGetFieldMappingResponse
	{
		private Inferrer _inferrer { get; set; }

		public GetFieldMappingResponse()
		{
			this.Indices = new IndexFieldMappings();
		}

		//if you call get mapping on an existing type and index but no fields match you still get back a 200.
		public override bool IsValid => base.IsValid && this.Indices.HasAny();

		internal GetFieldMappingResponse(IApiCallDetails status, IndexFieldMappings dict, Inferrer inferrer)
		{
			this.Indices = dict ?? new IndexFieldMappings();
			this._inferrer = inferrer;
		}

		public IndexFieldMappings Indices { get; set; }

		public FieldMappingProperties MappingsFor(string indexName, string typeName)
		{
			TypeFieldMappings index;
			FieldMappingProperties type;

			if (!this.Indices.TryGetValue(indexName, out index) || index.Mappings == null) return null;
			return !index.Mappings.TryGetValue(typeName, out type) ? null : type;
		}

		public IFieldMapping MappingFor(string indexName, string typeName, string fieldName)
		{
			if (fieldName.IsNullOrEmpty()) return null;

			var type = this.MappingsFor(indexName, typeName);
			if (type == null) return null;

			FieldMapping field;
			if (!type.TryGetValue(fieldName, out field) || field.Mapping == null) return null;

			var name = fieldName.Split('.').Last();
			return field.Mapping[name];
		}

		public IFieldMapping MappingFor<T>(string fieldName)
			where T : class
		{
			var indexName = this._inferrer.IndexName<T>();
			var typeName = this._inferrer.TypeName<T>();
			return this.MappingFor(indexName, typeName, fieldName);
		}

		public IFieldMapping MappingFor<T>(Expression<Func<T, object>> fieldName)
			where T : class
		{
			var path = this._inferrer.Field(fieldName);
			return this.MappingFor<T>(path);
		}

		public FieldMappingProperties MappingsFor<T>(string indexName = null, string typeName = null)
			where T : class
		{
			indexName = indexName ?? this._inferrer.IndexName<T>();
			typeName = typeName ?? this._inferrer.TypeName<T>();

			return this.MappingsFor(indexName, typeName);
		}
	}
}
