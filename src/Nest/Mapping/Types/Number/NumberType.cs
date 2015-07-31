using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using System.Linq.Expressions;
using Newtonsoft.Json.Converters;

namespace Nest
{
	[JsonObject(MemberSerialization.OptIn)]
	public interface INumberType : IElasticType
	{
		[JsonProperty("index")]
		NonStringIndexOption Index { get; set; }

		[JsonProperty("boost")]
		double? Boost { get; set; }

		[JsonProperty("null_value")]
		double? NullValue { get; set; }

		[JsonProperty("include_in_all")]
		bool? IncludeInAll { get; set; }

		[JsonProperty("precision_step")]
		int? PrecisionStep { get; set; }

		[JsonProperty("ignore_malformed")]
		bool? IgnoreMalformed { get; set; }

		[JsonProperty("coerce")]
		bool? Coerce { get; set; }
	}

	public class NumberType : ElasticType, INumberType
	{
		public NumberType() : base(NumberTypeName.Double.GetStringValue()) { }
		public NumberType(NumberTypeName typeName) : base(typeName.GetStringValue()) { }
		protected NumberType(string typeName) : base(typeName) { }

		public NonStringIndexOption Index { get; set; }
		public double? Boost { get; set; }
		public double? NullValue { get; set; }
		public bool? IncludeInAll { get; set; }
		public int? PrecisionStep { get; set; }
		public bool? IgnoreMalformed { get; set; }
		public bool? Coerce { get; set; }
	}

	public class NumberTypeDescriptor<T> 
		: TypeDescriptorBase<NumberTypeDescriptor<T>, INumberType, T>, INumberType
		where T : class
	{
		NonStringIndexOption INumberType.Index { get; set; }
		double? INumberType.Boost { get; set; }
		double? INumberType.NullValue { get; set; }
		bool? INumberType.IncludeInAll { get; set; }
		int? INumberType.PrecisionStep { get; set; }
		bool? INumberType.IgnoreMalformed { get; set; }
		bool? INumberType.Coerce { get; set; }

		public NumberTypeDescriptor<T> Type(NumberTypeName type) => Assign(a => a.Type = type.GetStringValue());

		public NumberTypeDescriptor<T> Index(NonStringIndexOption index = NonStringIndexOption.No) => Assign(a => a.Index = index);

		public NumberTypeDescriptor<T> Boost(double boost) => Assign(a => a.Boost = boost);

		public NumberTypeDescriptor<T> NullValue(double nullValue) => Assign(a => a.NullValue = nullValue);

		public NumberTypeDescriptor<T> PrecisionStep(int precisionStep) => Assign(a => a.PrecisionStep = precisionStep);

		public NumberTypeDescriptor<T> IgnoreMalformed(bool ignoreMalformed = true) => Assign(a => a.IgnoreMalformed = ignoreMalformed);

		public NumberTypeDescriptor<T> Coerce(bool coerce = true) => Assign(a => a.Coerce = coerce);

		//public NumberTypeDescriptor<T> FieldData(Func<FieldDataNonStringMappingDescriptor, FieldDataNonStringMappingDescriptor> fieldDataSelector)
		//{
		//	fieldDataSelector.ThrowIfNull("fieldDataSelector");
		//	var selector = fieldDataSelector(new FieldDataNonStringMappingDescriptor());
		//	this._Mapping.FieldData = selector.FieldData;
		//	return this;
		//}

		//public NumberTypeDescriptor<T> FieldData(FieldDataNonStringMapping fieldData)
		//{
		//	this._Mapping.FieldData = fieldData;
		//	return this;
		//}
	}
}