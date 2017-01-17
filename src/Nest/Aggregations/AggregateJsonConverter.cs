﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Nest252
{
	internal class AggregateJsonConverter : JsonConverter
	{
		private static readonly Regex _numeric = new Regex(@"^[\d.]+(\.[\d.]+)?$");

		public override bool CanWrite => false;

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) => this.ReadAggregate(reader, serializer);

		public override bool CanConvert(Type objectType) => objectType == typeof(IAggregate);

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotSupportedException();
		}

		private IAggregate ReadAggregate(JsonReader reader, JsonSerializer serializer)
		{
			if (reader.TokenType != JsonToken.StartObject)
				return null;
			reader.Read();

			if (reader.TokenType != JsonToken.PropertyName)
				return null;

			IAggregate aggregate = null;

			var propertyName = (string)reader.Value;
			if (_numeric.IsMatch(propertyName))
				aggregate = GetPercentilesAggregate(reader, serializer, oldFormat: true);

			var meta = propertyName == "meta"
				? GetMetadata(reader)
				: null;

			if (aggregate != null)
			{
				aggregate.Meta = meta;
				return aggregate;
			}

			propertyName = (string)reader.Value;
			switch (propertyName)
			{
				case "values":
					reader.Read();
					reader.Read();
					aggregate = GetPercentilesAggregate(reader, serializer);
					break;
				case "value":
					aggregate = GetValueAggregate(reader, serializer);
					break;
				case "buckets":
				case "doc_count_error_upper_bound":
					aggregate = GetMultiBucketAggregate(reader, serializer);
					break;
				case "count":
					aggregate = GetStatsAggregate(reader, serializer);
					break;
				case "doc_count":
					aggregate = GetSingleBucketAggregate(reader, serializer);
					break;
				case "bounds":
					aggregate = GetGeoBoundsAggregate(reader, serializer);
					break;
				case "hits":
					aggregate = GetTopHitsAggregate(reader, serializer);
					break;
				case "location":
					aggregate = GetGeoCentroidAggregate(reader, serializer);
					break;
				default:
					return null;
			}
			aggregate.Meta = meta;
			return aggregate;
		}

		private IBucket ReadBucket(JsonReader reader, JsonSerializer serializer)
		{
			if (reader.TokenType != JsonToken.StartObject)
				return null;
			reader.Read();

			if (reader.TokenType != JsonToken.PropertyName)
				return null;

			IBucket item;
			var property = (string)reader.Value;
			switch (property)
			{
				case "key":
					item = GetKeyedBucket(reader, serializer);
					break;
				case "from":
				case "to":
					item = GetRangeBucket(reader, serializer);
					break;
				case "key_as_string":
					item = GetDateHistogramBucket(reader, serializer);
					break;
				case "doc_count":
					item = GetFiltersBucket(reader, serializer);
					break;
				default:
					return null;
			}
			return item;
		}

		private Dictionary<string, object> GetMetadata(JsonReader reader)
		{
			var meta = new Dictionary<string, object>();
			reader.Read();
			reader.Read();
			while (reader.TokenType != JsonToken.EndObject)
			{
				var key = (string)reader.Value;
				reader.Read();
				var value = reader.Value;
				meta.Add(key, value);
				reader.Read();
			}
			reader.Read();
			return meta;
		}

		private IAggregate GetTopHitsAggregate(JsonReader reader, JsonSerializer serializer)
		{
			reader.Read();
			var o = JObject.Load(reader);
			if (o == null)
				return null;

			var total = o["total"].ToObject<long>();
			var maxScore = o["max_score"].ToObject<double?>();
			var hits = o["hits"].Children().OfType<JObject>();
			reader.Read();
			return new TopHitsAggregate(hits, serializer) { Total = total, MaxScore = maxScore };
		}

		private IAggregate GetGeoCentroidAggregate(JsonReader reader, JsonSerializer serializer)
		{
			reader.Read();
			var geoCentroid = new GeoCentroidAggregate { Location = serializer.Deserialize<GeoLocation>(reader) };
			reader.Read();
			return geoCentroid;
		}

		private IAggregate GetGeoBoundsAggregate(JsonReader reader, JsonSerializer serializer)
		{
			reader.Read();
			var o = JObject.Load(reader);
			if (o == null)
				return null;
			var geoBoundsMetric = new GeoBoundsAggregate();
			JToken topLeftToken;
			if (o.TryGetValue("top_left", out topLeftToken) && topLeftToken != null)
			{
				var topLeft = topLeftToken.ToObject<LatLon>();
				if (topLeft != null)
					geoBoundsMetric.Bounds.TopLeft = topLeft;
			}

			JToken bottomRightToken;
			if (o.TryGetValue("bottom_right", out bottomRightToken) && bottomRightToken != null)
			{
				var bottomRight = bottomRightToken.ToObject<LatLon>();
				if (bottomRight != null)
					geoBoundsMetric.Bounds.BottomRight = bottomRight;
			}
			reader.Read();
			return geoBoundsMetric;
		}

		private IAggregate GetPercentilesAggregate(JsonReader reader, JsonSerializer serializer, bool oldFormat = false)
		{
			var metric = new PercentilesAggregate();
			var percentileItems = new List<PercentileItem>();
			if (reader.TokenType == JsonToken.StartObject)
				reader.Read();
			while (reader.TokenType != JsonToken.EndObject)
			{
				var propertyName = (string)reader.Value;
				if (propertyName.Contains("_as_string"))
				{
					reader.Read();
					reader.Read();
				}
				if (reader.TokenType != JsonToken.EndObject)
				{
					var percentileValue = (string)reader.Value;
					var percentile = double.Parse(percentileValue, CultureInfo.InvariantCulture);
					reader.Read();
					var value = reader.Value as double?;
					percentileItems.Add(new PercentileItem()
					{
						Percentile = percentile,
						Value = value.GetValueOrDefault(0)
					});
					reader.Read();
				}
			}
			metric.Items = percentileItems;
			if (!oldFormat) reader.Read();
			return metric;
		}

		private IAggregate GetSingleBucketAggregate(JsonReader reader, JsonSerializer serializer)
		{
			reader.Read();
			var docCount = (reader.Value as long?).GetValueOrDefault(0);
			var bucket = new SingleBucketAggregate { DocCount = docCount };
			reader.Read();
			if (reader.TokenType == JsonToken.PropertyName && (string)reader.Value == "buckets")
			{
				var b = this.GetMultiBucketAggregate(reader, serializer) as BucketAggregate;
				return new BucketAggregate
				{
					DocCount = docCount,
					Items = b.Items
				};
			}

			bucket.Aggregations = this.GetSubAggregates(reader, serializer);

			return bucket;
		}

		private IAggregate GetStatsAggregate(JsonReader reader, JsonSerializer serializer)
		{
			reader.Read();
			var count = (reader.Value as long?).GetValueOrDefault(0);
			reader.Read(); reader.Read();
			var min = reader.Value as double?;
			reader.Read(); reader.Read();
			var max = reader.Value as double?;
			reader.Read(); reader.Read();
			var average = reader.Value as double?;
			reader.Read(); reader.Read();
			var sum = reader.Value as double?;

			var statsMetric = new StatsAggregate()
			{
				Average = average,
				Count = count,
				Max = max,
				Min = min,
				Sum = sum
			};

			reader.Read();

			if (reader.TokenType == JsonToken.EndObject)
				return statsMetric;

			var propertyName = (string)reader.Value;
			while (reader.TokenType != JsonToken.EndObject && propertyName.Contains("_as_string"))
			{
				reader.Read();
				reader.Read();
			}

			if (reader.TokenType == JsonToken.EndObject)
				return statsMetric;

			return GetExtendedStatsAggregate(statsMetric, reader);
		}

		private IAggregate GetExtendedStatsAggregate(StatsAggregate statsMetric, JsonReader reader)
		{
			var extendedStatsMetric = new ExtendedStatsAggregate()
			{
				Average = statsMetric.Average,
				Count = statsMetric.Count,
				Max = statsMetric.Max,
				Min = statsMetric.Min,
				Sum = statsMetric.Sum
			};

			reader.Read();
			extendedStatsMetric.SumOfSquares = (reader.Value as double?);
			reader.Read();
			reader.Read();
			extendedStatsMetric.Variance = (reader.Value as double?);
			reader.Read(); reader.Read();
			extendedStatsMetric.StdDeviation = (reader.Value as double?);
			reader.Read();

			string propertyName;

			if (reader.TokenType != JsonToken.EndObject)
			{
				var bounds = new StandardDeviationBounds();
				reader.Read();
				reader.Read();

				propertyName = (string)reader.Value;
				if (propertyName == "upper")
				{
					reader.Read();
					bounds.Upper = reader.Value as double?;
				}
				reader.Read();

				propertyName = (string)reader.Value;
				if (propertyName == "lower")
				{
					reader.Read();
					bounds.Lower = reader.Value as double?;
				}
				extendedStatsMetric.StdDeviationBounds = bounds;
				reader.Read();
				reader.Read();
			}

			propertyName = (string)reader.Value;
			while (reader.TokenType != JsonToken.EndObject && propertyName.Contains("_as_string"))
			{
				// std_deviation_bounds is an object, so we need to skip its properties
				if (propertyName.Equals("std_deviation_bounds_as_string"))
				{
					reader.Read();
					reader.Read();
					reader.Read();
					reader.Read();
				}
				reader.Read();
				reader.Read();
			}
			return extendedStatsMetric;
		}

		private IDictionary<string, IAggregate> GetSubAggregates(JsonReader reader, JsonSerializer serializer, Dictionary<string, IAggregate> aggregates = null)
		{
			if (reader.TokenType != JsonToken.PropertyName)
				return aggregates;

			var nestedAggs = aggregates ?? new Dictionary<string, IAggregate>();

			var currentDepth = reader.Depth;
			do
			{
				var fieldName = (string)reader.Value;
				reader.Read();
				var agg = this.ReadAggregate(reader, serializer);
				nestedAggs.Add(fieldName, agg);
				reader.Read();
				if (reader.Depth == currentDepth && reader.TokenType == JsonToken.EndObject || reader.Depth < currentDepth)
					break;
			} while (true);
			return nestedAggs;
		}

		private IAggregate GetMultiBucketAggregate(JsonReader reader, JsonSerializer serializer)
		{
			var bucket = new BucketAggregate();
			var propertyName = (string)reader.Value;
			if (propertyName == "doc_count_error_upper_bound")
			{
				reader.Read();
				bucket.DocCountErrorUpperBound = reader.Value as long?;
				reader.Read();
			}
			propertyName = (string)reader.Value;
			if (propertyName == "sum_other_doc_count")
			{
				reader.Read();
				bucket.SumOtherDocCount = reader.Value as long?;
				reader.Read();
			}
			var items = new List<IBucket>();
			reader.Read();

			if (reader.TokenType == JsonToken.StartObject)
			{
				reader.Read();
				var aggs = new Dictionary<string, IAggregate>();
				while (reader.TokenType != JsonToken.EndObject)
				{
					var name = reader.Value.ToString();
					reader.Read();
					var innerAgg = this.ReadAggregate(reader, serializer);
					aggs.Add(name, innerAgg);
					reader.Read();
				}
				reader.Read();
				return new FiltersAggregate(aggs);
			}

			if (reader.TokenType != JsonToken.StartArray)
				return null;
			reader.Read(); //move from start array to start object
			if (reader.TokenType == JsonToken.EndArray)
			{
				reader.Read();
				bucket.Items = Enumerable.Empty<IBucket>();
				return bucket;
			}
			do
			{
				var item = this.ReadBucket(reader, serializer);
				items.Add(item);
				reader.Read();
			} while (reader.TokenType != JsonToken.EndArray);
			bucket.Items = items;
			reader.Read();
			return bucket;
		}

		private IAggregate GetValueAggregate(JsonReader reader, JsonSerializer serializer)
		{
			reader.Read();
			var valueMetric = new ValueAggregate
			{
				Value = reader.Value as double?
			};
			if (valueMetric.Value == null && reader.ValueType == typeof(long))
				valueMetric.Value = reader.Value as long?;

			if (valueMetric.Value != null)
			{
				reader.Read();
				if (reader.TokenType != JsonToken.EndObject)
				{
					if (reader.TokenType == JsonToken.PropertyName)
					{
						var propertyName = (string)reader.Value;

						if (propertyName == "value_as_string")
						{
							valueMetric.ValueAsString = reader.ReadAsString();
							reader.Read();
						}

						if (reader.TokenType == JsonToken.PropertyName)
						{
							propertyName = (string)reader.Value;
							if (propertyName == "keys")
							{
								var keyedValueMetric = new KeyedValueAggregate
								{
									Value = valueMetric.Value
								};
								var keys = new List<string>();
								reader.Read();
								reader.Read();
								while (reader.TokenType != JsonToken.EndArray)
								{
									keys.Add(reader.Value.ToString());
									reader.Read();
								}
								reader.Read();
								keyedValueMetric.Keys = keys;
								return keyedValueMetric;
							}
						}
					}
					else
					{
						reader.Read();
						reader.Read();
					}
				}
				return valueMetric;
			}

			var scriptedMetric = serializer.Deserialize(reader);

			if (scriptedMetric != null)
				return new ScriptedMetricAggregate { _Value = scriptedMetric };

			reader.Read();
			return valueMetric;
		}

		public IBucket GetRangeBucket(JsonReader reader, JsonSerializer serializer, string key = null)
		{
			string fromAsString = null, toAsString = null;
			long? docCount = null;
			double? toDouble = null, fromDouble = null;

			var readExpectedProperty = true;
			while (readExpectedProperty)
			{
				switch (reader.Value as string)
				{
					case "from":
						reader.Read();
						if (reader.ValueType == typeof(double))
							fromDouble = (double)reader.Value;
						reader.Read();
						break;
					case "to":
						reader.Read();
						if (reader.ValueType == typeof(double))
							toDouble = (double)reader.Value;
						reader.Read();
						break;
					case "key":
						key = reader.ReadAsString();
						reader.Read();
						break;
					case "from_as_string":
						fromAsString = reader.ReadAsString();
						reader.Read();
						break;
					case "to_as_string":
						toAsString = reader.ReadAsString();
						reader.Read();
						break;
					case "doc_count":
						reader.Read();
						docCount = (reader.Value as long?).GetValueOrDefault(0);
						reader.Read();
						break;
					default:
						readExpectedProperty = false;
						break;
				}
			}

			var bucket = new RangeBucket
			{
				Key = key,
				From = fromDouble,
				To = toDouble,
				DocCount = docCount.GetValueOrDefault(),
				FromAsString = fromAsString,
				ToAsString = toAsString,
				Aggregations = this.GetSubAggregates(reader, serializer)
			};

			return bucket;
		}

		private IBucket GetDateHistogramBucket(JsonReader reader, JsonSerializer serializer)
		{
			var keyAsString = reader.ReadAsString();
			reader.Read();
			reader.Read();
			var key = (reader.Value as long?).GetValueOrDefault(0);
			reader.Read();
			reader.Read();
			var docCount = (reader.Value as long?).GetValueOrDefault(0);
			reader.Read();

			var dateHistogram = new DateHistogramBucket
			{
				Key = key,
				KeyAsString = keyAsString,
				DocCount = docCount,
				Aggregations = this.GetSubAggregates(reader, serializer)
			};

			return dateHistogram;

		}

		private IBucket GetKeyedBucket(JsonReader reader, JsonSerializer serializer)
		{
			var key = reader.ReadAsString();
			reader.Read();
			var propertyName = (string)reader.Value;
			if (propertyName == "from" || propertyName == "to")
				return GetRangeBucket(reader, serializer, key);

			var bucket = new KeyedBucket {Key = key};

			if (propertyName == "key_as_string")
			{
				bucket.KeyAsString = reader.ReadAsString();
				reader.Read();
			}

			reader.Read(); //doc_count;
			var docCount = reader.Value as long?;
			bucket.DocCount = docCount.GetValueOrDefault(0);
			reader.Read();

			Dictionary<string, IAggregate> aggregates = null;
			var nextProperty = (string)reader.Value;
			if (nextProperty == "score")
			{
				reader.Read();
				if (reader.TokenType == JsonToken.Float || reader.TokenType == JsonToken.Null)
				{
					return GetSignificantTermsBucket(reader, serializer, bucket);
				}

				// score is the name of an aggregation
				aggregates = new Dictionary<string, IAggregate>();
				var aggregate = this.ReadAggregate(reader, serializer);
				reader.Read();
				aggregates.Add(nextProperty, aggregate);
			}

			bucket.Aggregations = this.GetSubAggregates(reader, serializer, aggregates);
			return bucket;

		}

		private IBucket GetSignificantTermsBucket(JsonReader reader, JsonSerializer serializer, KeyedBucket keyItem)
		{
			var score = reader.Value as double?;
			reader.Read();
			reader.Read();
			var bgCount = reader.Value as long?;
			var significantTermItem = new SignificantTermsBucket()
			{
				Key = keyItem.Key,
				DocCount = keyItem.DocCount.GetValueOrDefault(0),
				BgCount = bgCount.GetValueOrDefault(0),
				Score = score.GetValueOrDefault(0)
			};
			reader.Read();
			significantTermItem.Aggregations = this.GetSubAggregates(reader, serializer);
			return significantTermItem;
		}

		private IBucket GetFiltersBucket(JsonReader reader, JsonSerializer serializer)
		{
			reader.Read();
			var filtersBucketItem = new FiltersBucketItem
			{
				DocCount = (reader.Value as long?).GetValueOrDefault(0)
			};
			reader.Read();
			filtersBucketItem.Aggregations = this.GetSubAggregates(reader, serializer);
			return filtersBucketItem;
		}
	}
}
