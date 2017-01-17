﻿using System;
using System.Linq;
using Elasticsearch252.Net;
using FluentAssertions;
using Nest252;
using Tests.Framework;
using Tests.Framework.Integration;
using Tests.Framework.MockData;
using Xunit;

namespace Tests.Cluster.TaskManagement.TasksList
{
	[SkipVersion("<2.3.0", "")]
	public class TasksListApiTests : ApiIntegrationTestBase<ReadOnlyCluster, ITasksListResponse, ITasksListRequest, TasksListDescriptor, TasksListRequest>
	{
		public TasksListApiTests(ReadOnlyCluster cluster, EndpointUsage usage) : base(cluster, usage) { }
		protected override LazyResponses ClientUsage() => Calls(
			fluent: (client, f) => client.TasksList(f),
			fluentAsync: (client, f) => client.TasksListAsync(f),
			request: (client, r) => client.TasksList(r),
			requestAsync: (client, r) => client.TasksListAsync(r)
		);

		protected override bool ExpectIsValid => true;
		protected override int ExpectStatusCode => 200;
		protected override HttpMethod HttpMethod => HttpMethod.GET;
		protected override string UrlPath => "/_tasks?actions=%2Alists%2A";

		protected override Func<TasksListDescriptor, ITasksListRequest> Fluent => s => s
			.Actions("*lists*");

		protected override TasksListRequest Initializer => new TasksListRequest
		{
			Actions = new [] { "*lists*" }
		};

		protected override void ExpectResponse(ITasksListResponse response)
		{
			response.Nodes.Should().NotBeEmpty();
			var taskExecutingNode = response.Nodes.First().Value;
			taskExecutingNode.Host.Should().NotBeNullOrWhiteSpace();
			taskExecutingNode.Ip.Should().NotBeNullOrWhiteSpace();
			taskExecutingNode.Name.Should().NotBeNullOrWhiteSpace();
			taskExecutingNode.TransportAddress.Should().NotBeNullOrWhiteSpace();
			taskExecutingNode.Tasks.Should().NotBeEmpty();
			taskExecutingNode.Tasks.Count().Should().BeGreaterOrEqualTo(2);

			var task = taskExecutingNode.Tasks.Values.First(p => p.ParentTaskId != null);
			task.Action.Should().NotBeNullOrWhiteSpace();
			task.Type.Should().NotBeNullOrWhiteSpace();
			task.Id.Should().BePositive();
			task.Node.Should().NotBeNullOrWhiteSpace();
			task.RunningTimeInNanoSeconds.Should().BeGreaterThan(0);
			task.StartTimeInMilliseconds.Should().BeGreaterThan(0);
			task.ParentTaskId.Should().NotBeNull();

			var parentTask = taskExecutingNode.Tasks[task.ParentTaskId];
			parentTask.Should().NotBeNull();
			parentTask.ParentTaskId.Should().BeNull();
		}
	}

	[SkipVersion("<2.3.0", "")]
	public class TasksListDetailedApiTests : ApiIntegrationTestBase<IntrusiveOperationCluster, ITasksListResponse, ITasksListRequest, TasksListDescriptor, TasksListRequest>
	{
		private static TaskId _taskId = new TaskId("fakeid:1");

		public TasksListDetailedApiTests(IntrusiveOperationCluster cluster, EndpointUsage usage) : base(cluster, usage) { }
		protected override LazyResponses ClientUsage() => Calls(
			fluent: (client, f) => client.TasksList(f),
			fluentAsync: (client, f) => client.TasksListAsync(f),
			request: (client, r) => client.TasksList(r),
			requestAsync: (client, r) => client.TasksListAsync(r)
		);

		protected override void IntegrationSetup(IElasticClient client, CallUniqueValues values)
		{
			var seeder = new Seeder(this.Cluster.Node);
			seeder.SeedNode();

			// get a suitable load of projects in order to get a decent task status out
			var bulkResponse = client.IndexMany(Project.Generator.Generate(20000));
			if (!bulkResponse.IsValid)
				throw new Exception("failure in setting up integration");

			var response = client.ReindexOnServer(r => r
				.Source(s => s
					.Index(Infer.Index<Project>())
					.Type(typeof(Project))
				)
				.Destination(d => d
					.Index("tasks-list-projects")
					.OpType(OpType.Create)
				)
				.Conflicts(Conflicts.Proceed)
				.WaitForCompletion(false)
				.Refresh()
			);

			_taskId = response.Task;
		}

		protected override bool ExpectIsValid => true;
		protected override int ExpectStatusCode => 200;
		protected override HttpMethod HttpMethod => HttpMethod.GET;
		protected override string UrlPath => $"/_tasks/{Uri.EscapeDataString(_taskId.ToString())}?detailed=true";

		protected override Func<TasksListDescriptor, ITasksListRequest> Fluent => s => s
			.TaskId(_taskId)
			.Detailed();

		protected override TasksListRequest Initializer => new TasksListRequest(_taskId)
		{
			Detailed = true
		};

		protected override void ExpectResponse(ITasksListResponse response)
		{
			response.Nodes.Should().NotBeEmpty();
			var taskExecutingNode = response.Nodes.First().Value;
			taskExecutingNode.Host.Should().NotBeNullOrWhiteSpace();
			taskExecutingNode.Ip.Should().NotBeNullOrWhiteSpace();
			taskExecutingNode.Name.Should().NotBeNullOrWhiteSpace();
			taskExecutingNode.TransportAddress.Should().NotBeNullOrWhiteSpace();
			taskExecutingNode.Tasks.Should().NotBeEmpty();
			taskExecutingNode.Tasks.Count().Should().Be(1);

			var task = taskExecutingNode.Tasks[_taskId];
			task.Action.Should().NotBeNullOrWhiteSpace();
			task.Type.Should().NotBeNullOrWhiteSpace();
			task.Id.Should().BePositive();
			task.Node.Should().NotBeNullOrWhiteSpace();
			task.RunningTimeInNanoSeconds.Should().BeGreaterThan(0);
			task.StartTimeInMilliseconds.Should().BeGreaterThan(0);

			var status = task.Status;
			status.Should().NotBeNull();
			status.Total.Should().BeGreaterOrEqualTo(0);
			status.Batches.Should().BeGreaterOrEqualTo(0);
		}
	}
}
