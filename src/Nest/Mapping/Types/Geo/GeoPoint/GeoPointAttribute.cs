﻿using System;

namespace Nest252
{
	public class GeoPointAttribute : ElasticsearchPropertyAttributeBase, IGeoPointProperty
	{
		IGeoPointProperty Self => this;

		bool? IGeoPointProperty.LatLon { get; set; }
		bool? IGeoPointProperty.GeoHash { get; set; }
		int? IGeoPointProperty.GeoHashPrecision { get; set; }
		bool? IGeoPointProperty.GeoHashPrefix { get; set; }
		bool? IGeoPointProperty.Validate { get; set; }
		bool? IGeoPointProperty.ValidateLatitude { get; set; }
		bool? IGeoPointProperty.ValidateLongitude { get; set; }
		bool? IGeoPointProperty.Normalize { get; set; }
		bool? IGeoPointProperty.NormalizeLatitude { get; set; }
		bool? IGeoPointProperty.NormalizeLongitude { get; set; }
		int? IGeoPointProperty.PrecisionStep { get; set; }
		IGeoPointFielddata IGeoPointProperty.Fielddata { get; set; }

		[Obsolete("Deprecated in 2.3.0 and Removed in 5.0.0")]
		public bool LatLon { get { return Self.LatLon.GetValueOrDefault(); } set { Self.LatLon = value; } }
		[Obsolete("Deprecated in 2.4.0 and Removed in 5.0.0")]
		public bool GeoHash { get { return Self.GeoHash.GetValueOrDefault(); } set { Self.GeoHash = value; } }
		[Obsolete("Deprecated in 2.4.0 and Removed in 5.0.0")]
		public bool GeoHashPrefix { get { return Self.GeoHashPrefix.GetValueOrDefault(); } set { Self.GeoHashPrefix = value; } }
		[Obsolete("Deprecated in 2.4.0 and Removed in 5.0.0")]
		public int GeoHashPrecision { get { return Self.GeoHashPrecision.GetValueOrDefault(); } set { Self.GeoHashPrecision = value; } }
		[Obsolete("Removed in 5.0.0. Use IgnoreMalformed")]
		public bool Validate { get { return Self.Validate.GetValueOrDefault(); } set { Self.Validate = value; } }
		[Obsolete("Removed in 5.0.0. Use IgnoreMalformed")]
		public bool ValidateLatitude { get { return Self.ValidateLatitude.GetValueOrDefault(); } set { Self.ValidateLatitude = value; } }
		[Obsolete("Removed in 5.0.0. Use IgnoreMalformed")]
		public bool ValidateLongitude { get { return Self.ValidateLongitude.GetValueOrDefault(); } set { Self.ValidateLongitude = value; } }
		[Obsolete("Removed in 5.0.0")]
		public bool Normalize { get { return Self.Normalize.GetValueOrDefault(); } set { Self.Normalize = value; } }
		[Obsolete("Removed in 5.0.0")]
		public bool NormalizeLatitude { get { return Self.NormalizeLatitude.GetValueOrDefault(); } set { Self.NormalizeLatitude = value; } }
		[Obsolete("Removed in 5.0.0")]
		public bool NormalizeLongitude { get { return Self.NormalizeLongitude.GetValueOrDefault(); } set { Self.NormalizeLongitude = value; } }
		[Obsolete("Removed in 5.0.0")]
		public int PrecisionStep { get { return Self.PrecisionStep.GetValueOrDefault(); } set { Self.PrecisionStep = value; } }

		public GeoPointAttribute() : base("geo_point") { }
	}
}
