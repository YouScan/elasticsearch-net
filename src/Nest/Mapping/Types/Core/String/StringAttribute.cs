﻿using System;

namespace Nest252
{
	public class StringAttribute : ElasticsearchPropertyAttributeBase, IStringProperty
	{
		IStringProperty Self => this;

		FieldIndexOption? IStringProperty.Index { get; set; }
        TermVectorOption? IStringProperty.TermVector { get; set; }
        double? IStringProperty.Boost { get; set; }
        string IStringProperty.NullValue { get; set; }
		INorms IStringProperty.Norms { get; set; }
		IndexOptions? IStringProperty.IndexOptions { get; set; }
        string IStringProperty.Analyzer { get; set; }
        string IStringProperty.SearchAnalyzer { get; set; }
        bool? IStringProperty.IncludeInAll { get; set; }
        int? IStringProperty.IgnoreAbove { get; set; }
		[Obsolete("Removed in 5.0.0. Use PositionIncrementGap instead.")]
        int? IStringProperty.PositionOffsetGap { get { return Self.PositionIncrementGap; } set { Self.PositionIncrementGap = value; } }
		int? IStringProperty.PositionIncrementGap { get; set; }
		IStringFielddata IStringProperty.Fielddata { get; set; }

		public string Analyzer { get { return Self.Analyzer; } set { Self.Analyzer = value; } }
		public double Boost { get { return Self.Boost.GetValueOrDefault(); } set { Self.Boost = value; } }
		public int IgnoreAbove { get { return Self.IgnoreAbove.GetValueOrDefault(); } set { Self.IgnoreAbove = value; } }
		public bool IncludeInAll { get { return Self.IncludeInAll.GetValueOrDefault(); } set { Self.IncludeInAll = value; } }
		public FieldIndexOption Index { get { return Self.Index.GetValueOrDefault(); } set { Self.Index = value; } }
		public IndexOptions IndexOptions { get { return Self.IndexOptions.GetValueOrDefault(); } set { Self.IndexOptions = value; } }
		public string NullValue { get { return Self.NullValue; } set { Self.NullValue = value; } }
		[Obsolete("Scheduled to be removed in 5.0.0. Use PositionIncrementGap instead.")]
		public int PositionOffsetGap { get { return Self.PositionIncrementGap.GetValueOrDefault(); } set { Self.PositionIncrementGap = value; } }
		public int PositionIncrementGap { get { return Self.PositionIncrementGap.GetValueOrDefault(); } set { Self.PositionIncrementGap = value; } }
		public string SearchAnalyzer { get { return Self.SearchAnalyzer; } set { Self.SearchAnalyzer = value; } }
		public TermVectorOption TermVector { get { return Self.TermVector.GetValueOrDefault(); } set { Self.TermVector = value; } }

		public StringAttribute() : base("string") { }
	}
}
