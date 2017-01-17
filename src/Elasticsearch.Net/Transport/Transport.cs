﻿using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace Elasticsearch252.Net
{
	public class Transport<TConnectionSettings> : ITransport<TConnectionSettings>
		where TConnectionSettings : IConnectionConfigurationValues
	{
		//TODO should all of these be public?
		public TConnectionSettings Settings { get; }
		public IDateTimeProvider DateTimeProvider { get; }
		public IMemoryStreamFactory MemoryStreamFactory { get; }
		public IRequestPipelineFactory PipelineProvider { get; }

		/// <summary>
		/// Transport coordinates the client requests over the connection pool nodes and is in charge of falling over on different nodes
		/// </summary>
		/// <param name="configurationValues">The connectionsettings to use for this transport</param>
		public Transport(TConnectionSettings configurationValues)
			: this(configurationValues, null, null, null)
		{ }

		/// <summary>
		/// Transport coordinates the client requests over the connection pool nodes and is in charge of falling over on different nodes
		/// </summary>
		/// <param name="configurationValues">The connectionsettings to use for this transport</param>
		/// <param name="pipelineProvider">In charge of create a new pipeline, safe to pass null to use the default</param>
		/// <param name="dateTimeProvider">The date time proved to use, safe to pass null to use the default</param>
		/// <param name="memoryStreamFactory">The memory stream provider to use, safe to pass null to use the default</param>
		public Transport(
			TConnectionSettings configurationValues,
			IRequestPipelineFactory pipelineProvider,
			IDateTimeProvider dateTimeProvider,
			IMemoryStreamFactory memoryStreamFactory
			)
		{
			configurationValues.ThrowIfNull(nameof(configurationValues));
			configurationValues.ConnectionPool.ThrowIfNull(nameof(configurationValues.ConnectionPool));
			configurationValues.Connection.ThrowIfNull(nameof(configurationValues.Connection));
			configurationValues.Serializer.ThrowIfNull(nameof(configurationValues.Serializer));

			this.Settings = configurationValues;
			this.PipelineProvider = pipelineProvider ?? new RequestPipelineFactory();
			this.DateTimeProvider = dateTimeProvider ?? Net.DateTimeProvider.Default;
			this.MemoryStreamFactory = memoryStreamFactory ?? new MemoryStreamFactory();
		}

		public ElasticsearchResponse<TReturn> Request<TReturn>(HttpMethod method, string path, PostData<object> data = null, IRequestParameters requestParameters = null)
			where TReturn : class
		{
			using (var pipeline = this.PipelineProvider.Create(this.Settings, this.DateTimeProvider, this.MemoryStreamFactory, requestParameters))
			{
				pipeline.FirstPoolUsage(this.Settings.BootstrapLock);

				var requestData = new RequestData(method, path, data, this.Settings, requestParameters, this.MemoryStreamFactory);
				this.Settings.OnRequestDataCreated?.Invoke(requestData);
				ElasticsearchResponse<TReturn> response = null;

				var seenExceptions = new List<PipelineException>();
				foreach (var node in pipeline.NextNode())
				{
					requestData.Node = node;
					try
					{
						pipeline.SniffOnStaleCluster();
						Ping(pipeline, node);
						response = pipeline.CallElasticsearch<TReturn>(requestData);
						if (!response.SuccessOrKnownError)
						{
							pipeline.MarkDead(node);
							pipeline.SniffOnConnectionFailure();
						}
					}
					catch (PipelineException pipelineException) when (!pipelineException.Recoverable)
					{
						pipeline.MarkDead(node);
						seenExceptions.Add(pipelineException);
						break;
					}
					catch (PipelineException pipelineException)
					{
						pipeline.MarkDead(node);
						seenExceptions.Add(pipelineException);
					}
					catch (Exception killerException)
					{
						throw new UnexpectedElasticsearchClientException(killerException, seenExceptions)
						{
							Request = requestData,
							Response = response,
							AuditTrail = pipeline?.AuditTrail
						};
					}
					if (response == null || !response.SuccessOrKnownError) continue;
					pipeline.MarkAlive(node);
					break;
				}
				if (response == null || !response.Success)
					pipeline.BadResponse(ref response, requestData, seenExceptions);

				this.Settings.OnRequestCompleted?.Invoke(response);

				return response;
			}
		}

		public async Task<ElasticsearchResponse<TReturn>> RequestAsync<TReturn>(HttpMethod method, string path, PostData<object> data = null, IRequestParameters requestParameters = null)
			where TReturn : class
		{
			using (var pipeline = this.PipelineProvider.Create(this.Settings, this.DateTimeProvider, this.MemoryStreamFactory, requestParameters))
			{
				await pipeline.FirstPoolUsageAsync(this.Settings.BootstrapLock).ConfigureAwait(false);

				var requestData = new RequestData(method, path, data, this.Settings, requestParameters, this.MemoryStreamFactory);
				this.Settings.OnRequestDataCreated?.Invoke(requestData);
				ElasticsearchResponse<TReturn> response = null;

				var seenExceptions = new List<PipelineException>();
				foreach (var node in pipeline.NextNode())
				{
					requestData.Node = node;
					try
					{
						await pipeline.SniffOnStaleClusterAsync().ConfigureAwait(false);
						await PingAsync(pipeline, node).ConfigureAwait(false);
						response = await pipeline.CallElasticsearchAsync<TReturn>(requestData).ConfigureAwait(false);
						if (!response.SuccessOrKnownError)
						{
							pipeline.MarkDead(node);
							await pipeline.SniffOnConnectionFailureAsync().ConfigureAwait(false);
						}
					}
					catch (PipelineException pipelineException) when (!pipelineException.Recoverable)
					{
						pipeline.MarkDead(node);
						seenExceptions.Add(pipelineException);
						break;
					}
					catch (PipelineException pipelineException)
					{
						pipeline.MarkDead(node);
						seenExceptions.Add(pipelineException);
					}
					catch (Exception killerException)
					{
						throw new UnexpectedElasticsearchClientException(killerException, seenExceptions)
						{
							Request = requestData,
							Response = response,
							AuditTrail = pipeline.AuditTrail
						};
					}
					if (response == null || !response.SuccessOrKnownError) continue;
					pipeline.MarkAlive(node);
					break;
				}
				if (response == null || !response.Success)
					pipeline.BadResponse(ref response, requestData, seenExceptions);

				this.Settings.OnRequestCompleted?.Invoke(response);

				return response;
			}
		}

		private static void Ping(IRequestPipeline pipeline, Node node)
		{
			try
			{
				pipeline.Ping(node);
			}
			catch (PipelineException e) when (e.Recoverable)
			{
				pipeline.SniffOnConnectionFailure();
				throw;
			}
		}

		private static async Task PingAsync(IRequestPipeline pipeline, Node node)
		{
			try
			{
				await pipeline.PingAsync(node).ConfigureAwait(false);
			}
			catch (PipelineException e) when (e.Recoverable)
			{
				await pipeline.SniffOnConnectionFailureAsync().ConfigureAwait(false);
				throw;
			}
		}

	}
}
