﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Elasticsearch252.Net;

namespace Nest252
{
	using GetWarmerConverter = Func<IApiCallDetails, Stream, GetWarmerResponse>;
	using CrazyWarmerResponse = Dictionary<string, Dictionary<string, IWarmers>>;

	public partial interface IElasticClient
	{
		/// <summary>
		/// Allows to put a warmup search request on a specific index (or indices), with the body composing of a regular 
		/// search request. Types can be provided as part of the URI if the search request is designed to be run only 
		/// against the specific types.
		/// <para> </para>http://www.elasticsearch.org/guide/en/elasticsearch/reference/current/indices-warmers.html#warmer-adding
		/// </summary>
		/// <param name="name">The name for the warmer that you want to register</param>
		/// <param name="selector">A descriptor that further describes what the warmer should look like</param>
		IPutWarmerResponse PutWarmer(Name name, Func<PutWarmerDescriptor, IPutWarmerRequest> selector);

		/// <inheritdoc/>
		IPutWarmerResponse PutWarmer(IPutWarmerRequest request);

		/// <inheritdoc/>
		Task<IPutWarmerResponse> PutWarmerAsync(Name name, Func<PutWarmerDescriptor, IPutWarmerRequest> selector);

		/// <inheritdoc/>
		Task<IPutWarmerResponse> PutWarmerAsync(IPutWarmerRequest request);

	}

	public partial class ElasticClient
	{
		//TODO AllIndices() seems odd here

		/// <inheritdoc/>
		public IPutWarmerResponse PutWarmer(Name name, Func<PutWarmerDescriptor, IPutWarmerRequest> selector) =>
			this.PutWarmer(selector?.Invoke(new PutWarmerDescriptor(name)));

		/// <inheritdoc/>
		public IPutWarmerResponse PutWarmer(IPutWarmerRequest request) => 
			this.Dispatcher.Dispatch<IPutWarmerRequest, PutWarmerRequestParameters, PutWarmerResponse>(
				request,
				this.LowLevelDispatch.IndicesPutWarmerDispatch<PutWarmerResponse>
			);

		/// <inheritdoc/>
		public Task<IPutWarmerResponse> PutWarmerAsync(Name name, Func<PutWarmerDescriptor, IPutWarmerRequest> selector) => 
			this.PutWarmerAsync(selector?.Invoke(new PutWarmerDescriptor(name)));

		/// <inheritdoc/>
		public Task<IPutWarmerResponse> PutWarmerAsync(IPutWarmerRequest request) => 
			this.Dispatcher.DispatchAsync<IPutWarmerRequest, PutWarmerRequestParameters, PutWarmerResponse, IPutWarmerResponse>(
				request,
				this.LowLevelDispatch.IndicesPutWarmerDispatchAsync<PutWarmerResponse>
			);
	}
}