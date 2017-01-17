using System;
using Newtonsoft.Json;

namespace Nest252
{
	[JsonObject(MemberSerialization.OptIn)]
	public interface IDateProperty : IProperty
	{
		[JsonProperty("index")]
		NonStringIndexOption? Index { get; set; }

		[JsonProperty("boost")]
        double? Boost { get; set; }

		[JsonProperty("null_value")]
        DateTime? NullValue { get; set; }

		[JsonProperty("include_in_all")]
		bool? IncludeInAll { get; set; }

		[JsonProperty("precision_step")]
		[Obsolete("Removed in 5.0.0")]
		int? PrecisionStep { get; set; }

		[JsonProperty("ignore_malformed")]
		bool? IgnoreMalformed { get; set; }

		[JsonProperty("format")]
		string Format { get; set; }

		[JsonProperty("numeric_resolution")]
		[Obsolete("Removed in 5.0.0")]
		NumericResolutionUnit? NumericResolution { get; set; }

		[JsonProperty("fielddata")]
		INumericFielddata Fielddata { get; set; }
	}

	public class DateProperty : PropertyBase, IDateProperty
	{
		public DateProperty() : base("date") { }

		public NonStringIndexOption? Index { get; set; }
		public double? Boost { get; set; }
		public DateTime? NullValue { get; set; }
		public bool? IncludeInAll { get; set; }
		[Obsolete("Removed in 5.0.0")]
		public int? PrecisionStep { get; set; }
		public bool? IgnoreMalformed { get; set; }
		public string Format { get; set; }
		[Obsolete("Removed in 5.0.0")]
		public NumericResolutionUnit? NumericResolution { get; set; }
		public INumericFielddata Fielddata { get; set; }
	}

	public class DatePropertyDescriptor<T>
		: PropertyDescriptorBase<DatePropertyDescriptor<T>, IDateProperty, T>, IDateProperty
		where T : class
	{
		NonStringIndexOption? IDateProperty.Index { get; set; }
		double? IDateProperty.Boost { get; set; }
		DateTime? IDateProperty.NullValue { get; set; }
		bool? IDateProperty.IncludeInAll { get; set; }
		int? IDateProperty.PrecisionStep { get; set; }
		bool? IDateProperty.IgnoreMalformed { get; set; }
		string IDateProperty.Format { get; set; }
		NumericResolutionUnit? IDateProperty.NumericResolution { get; set; }
		INumericFielddata IDateProperty.Fielddata { get; set; }

		public DatePropertyDescriptor() : base("date") { }

		public DatePropertyDescriptor<T> Index(NonStringIndexOption index = NonStringIndexOption.NotAnalyzed) => Assign(a => a.Index = index);
		public DatePropertyDescriptor<T> Boost(double boost) => Assign(a => a.Boost = boost);
		public DatePropertyDescriptor<T> NullValue(DateTime nullValue) => Assign(a => a.NullValue = nullValue);
		public DatePropertyDescriptor<T> IncludeInAll(bool includeInAll = true) => Assign(a => a.IncludeInAll = includeInAll);
		[Obsolete("Removed in 5.0.0")]
		public DatePropertyDescriptor<T> PrecisionStep(int precisionStep) => Assign(a => a.PrecisionStep = precisionStep);
		public DatePropertyDescriptor<T> IgnoreMalformed(bool ignoreMalformed = true) => Assign(a => a.IgnoreMalformed = ignoreMalformed);
		public DatePropertyDescriptor<T> Format(string format) => Assign(a => a.Format = format);
		[Obsolete("Removed in 5.0.0")]
		public DatePropertyDescriptor<T> NumericResolution(NumericResolutionUnit unit) => Assign(a => a.NumericResolution = unit);
		public DatePropertyDescriptor<T> Fielddata(Func<NumericFielddataDescriptor, INumericFielddata> selector) =>
			Assign(a => a.Fielddata = selector(new NumericFielddataDescriptor()));
	}
}
