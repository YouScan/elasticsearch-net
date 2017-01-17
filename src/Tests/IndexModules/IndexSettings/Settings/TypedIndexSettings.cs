﻿using System;
using System.Collections.Generic;
using FluentAssertions;
using Nest252;
using Tests.Framework;

namespace Tests.IndexModules.IndexSettings.Settings
{
	public class TypedIndexSettings
	{
		/**
		 */

		public class Usage : PromiseUsageTestBase<IIndexSettings, IndexSettingsDescriptor, Nest252.IndexSettings>
		{
			protected override object ExpectJson => new Dictionary<string, object>
			{
				{"any.setting", "can be set"},
				{"doubles", 1.1},
				{"bools", false},
				{"enums", "analyzed"},
				{"index.number_of_replicas", 2},
				{"index.auto_expand_replicas", "1-3" },
				{"index.refresh_interval", -1 },
				{"index.blocks.read_only", true},
				{"index.blocks.read", true},
				{"index.blocks.write", true},
				{"index.blocks.metadata", true},
				{"index.priority", 11},
				{"index.recovery.initial_shards", "full-1"},
				{"index.routing.allocation.total_shards_per_node", 10 },
				{"index.unassigned.node_left.delayed_timeout", "1m" },
				{"index.number_of_shards", 1},
				{"index.store.type", "mmapfs"},
			};

			/**
			 *
			 */
			protected override Func<IndexSettingsDescriptor, IPromise<IIndexSettings>> Fluent => s => s
				.Setting("any.setting", "can be set")
				.Setting("doubles", 1.1)
				.Setting("bools", false)
				.Setting("enums", FieldIndexOption.Analyzed)
				.NumberOfShards(1)
				.NumberOfReplicas(2)
				.AutoExpandReplicas("1-3")
				.BlocksMetadata()
				.BlocksRead()
				.BlocksReadOnly()
				.BlocksWrite()
				.Priority(11)
				.RecoveryInitialShards(RecoveryInitialShards.FullMinusOne)
				.TotalShardsPerNode(10)
				.UnassignedNodeLeftDelayedTimeout(TimeSpan.FromMinutes(1))
				.RefreshInterval(-1)
				.FileSystemStorageImplementation(FileSystemStorageImplementation.MMap);

			/**
			 */
			protected override Nest252.IndexSettings Initializer =>
				new Nest252.IndexSettings(new Dictionary<string, object>
				{
					{ "any.setting", "can be set" },
					{ "doubles", 1.1 },
					{ "bools", false },
					{ "enums", FieldIndexOption.Analyzed },
				})
				{
					NumberOfShards = 1,
					NumberOfReplicas = 2,
					AutoExpandReplicas = "1-3",
					BlocksMetadata = true,
					BlocksRead = true,
					BlocksReadOnly = true,
					BlocksWrite = true,
					Priority = 11,
					RecoveryInitialShards = RecoveryInitialShards.FullMinusOne,
					RoutingAllocationTotalShardsPerNode = 10,
					UnassignedNodeLeftDelayedTimeout = "1m",
					RefreshInterval = -1,
					FileSystemStorageImplementation = FileSystemStorageImplementation.MMap
				};
		}


		public class CamelCaseSettingsAreReadCorrectlyTests : SerializationTestBase
		{
			[U] public void CamelCasedTypedSettings()
			{
				var indexSettings = new Nest252.IndexSettings
				{
					{ "index.numberOfShards", 2 }
				};
				var settings = this.Serialize(indexSettings);
				var deserizalized = this.Deserialize<Nest252.IndexSettings>(settings);

				deserizalized.NumberOfShards.Should().Be(2);



			}
		}
	}
}
