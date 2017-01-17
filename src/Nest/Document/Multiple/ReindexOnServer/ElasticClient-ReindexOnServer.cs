﻿using System;
using System.Threading.Tasks;
using Elasticsearch252.Net;
using System.IO;

namespace Nest252
{
	public partial interface IElasticClient
	{
		/// <inheritdoc/>
		IReindexOnServerResponse ReindexOnServer(Func<ReindexOnServerDescriptor, IReindexOnServerRequest> selector);

		/// <inheritdoc/>
		IReindexOnServerResponse ReindexOnServer(IReindexOnServerRequest request);

		/// <inheritdoc/>
		Task<IReindexOnServerResponse> ReindexOnServerAsync(Func<ReindexOnServerDescriptor, IReindexOnServerRequest> selector);

		/// <inheritdoc/>
		Task<IReindexOnServerResponse> ReindexOnServerAsync(IReindexOnServerRequest request);

	}

	public partial class ElasticClient
	{
		/// <inheritdoc/>
		public IReindexOnServerResponse ReindexOnServer(Func<ReindexOnServerDescriptor, IReindexOnServerRequest> selector) =>
			this.ReindexOnServer(selector.InvokeOrDefault(new ReindexOnServerDescriptor()));

		/// <inheritdoc/>
		public IReindexOnServerResponse ReindexOnServer(IReindexOnServerRequest request) =>
			this.Dispatcher.Dispatch<IReindexOnServerRequest, ReindexOnServerRequestParameters, ReindexOnServerResponse>(
				this.ForceConfiguration<IReindexOnServerRequest, ReindexOnServerRequestParameters>(request, c => c.AllowedStatusCodes = new[] { -1 }),
				this.LowLevelDispatch.ReindexDispatch<ReindexOnServerResponse>
			);

		/// <inheritdoc/>
		public Task<IReindexOnServerResponse> ReindexOnServerAsync(Func<ReindexOnServerDescriptor, IReindexOnServerRequest> selector) =>
			this.ReindexOnServerAsync(selector.InvokeOrDefault(new ReindexOnServerDescriptor()));

		/// <inheritdoc/>
		public Task<IReindexOnServerResponse> ReindexOnServerAsync(IReindexOnServerRequest request) =>
			this.Dispatcher.DispatchAsync<IReindexOnServerRequest, ReindexOnServerRequestParameters, ReindexOnServerResponse, IReindexOnServerResponse>(
				this.ForceConfiguration<IReindexOnServerRequest, ReindexOnServerRequestParameters>(request, c => c.AllowedStatusCodes = new[] { -1 }),
				this.LowLevelDispatch.ReindexDispatchAsync<ReindexOnServerResponse>
			);
	}
}
