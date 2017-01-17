﻿using System;
using System.Collections.Generic;
using Elasticsearch252.Net;

namespace Nest252
{
	public interface IBulkAllRequest<T> where T : class
	{
		/// <summary> In case of a 429 (too busy) how long should we wait before retrying</summary>
		Time BackOffTime { get; set; }

		/// <summary> In case of a 429 (too busy) how many times should we automatically back off before failing</summary>
		int? BackOffRetries { get; set; }

		/// <summary> The number of documents to send per bulk</summary>
		int? Size { get; set; }

		///<summary>
		/// The documents to send to elasticsearch, try to keep the IEnumerable lazy.
		/// The bulk observable will ToList() each partitioned page in a lazy fashion, to keep memory consumption to a minimum.
		/// It makes no sense to set this to an list of 1 million records because all of those million records need to be in memory
		/// Make use of c#'s lazy generator!
		///</summary>
		IEnumerable<T> Documents { get; }

		///<summary>The maximum number of bulk operations we want to have in flight at a time</summary>
		int? MaxDegreeOfParallelism { get; set; }

		///<summary>Default index for items which don't provide one</summary>
		IndexName Index { get; set; }

		///<summary>Default document type for items which don't provide one</summary>
		TypeName Type { get; set; }

		///<summary>Explicit write consistency setting for the operation</summary>
		Consistency? Consistency { get; set; }

		///<summary>Refresh the index after performing each operation (elasticsearch will refresh locally)</summary>
		bool? Refresh { get; set; }

		///<summary>Refresh the index after performing ALL the bulk operations (NOTE this is an additional request)</summary>
		bool RefreshOnCompleted { get; set; }

		///<summary>Specific per bulk operation routing value</summary>
		string Routing { get; set; }

		///<summary>Explicit per operation timeout</summary>
		Time Timeout { get; set; }

		///<summary>The pipeline id to preprocess all the incoming documents with</summary>
		string Pipeline { get; set; }

		/// <summary>
		/// By default the bulkall helper simply calls <see cref="BulkDescriptor.IndexMany"/> on the buffer.
		/// There might be case where you'd like more control over this. By setting this callback you are in complete control
		/// of describing how the buffer should be translated to a bulk operation. Maybe you want to enforce all documents are newly created using
		/// <see cref="BulkDescriptor.CreateMany"/> or maybe a piece of metadata on <typeparamref name="T"/> controls where you need to call
		/// <see cref="BulkDescriptor.Update{T, TPartialDocument}(Func{BulkUpdateDescriptor{T, TPartialDocument}, IBulkUpdateOperation{T, TPartialDocument}})"/>
		/// or <see cref="BulkDescriptor.Index{T}(Func{BulkIndexDescriptor{T}, IIndexOperation{T}})"/>
		/// </summary>
		Action<BulkDescriptor, IList<T>> BufferToBulk { get; set; }
	}

	public class BulkAllRequest<T>  : IBulkAllRequest<T>
		where T : class
	{
		/// <inheritdoc />
		public Time BackOffTime { get; set; }
		/// <inheritdoc />
		public int? Size { get; set; }
		/// <inheritdoc />
		public int? MaxDegreeOfParallelism { get; set; }
		/// <inheritdoc />
		public int? BackOffRetries { get; set; }
		/// <inheritdoc />
		public IEnumerable<T> Documents { get; private set; }

		/// <inheritdoc />
		public IndexName Index { get; set; }
		/// <inheritdoc />
		public TypeName Type { get; set; }
		/// <inheritdoc />
		public Consistency? Consistency { get; set; }
		/// <inheritdoc />
		public bool? Refresh { get; set; }
		/// <inheritdoc />
		public bool RefreshOnCompleted { get; set; }
		/// <inheritdoc />
		public string Routing { get; set; }
		/// <inheritdoc />
		public Time Timeout { get; set; }
		/// <inheritdoc />
		public string Pipeline { get; set; }
		/// <inheritdoc />
		public Action<BulkDescriptor, IList<T>> BufferToBulk { get; set; }

		public BulkAllRequest(IEnumerable<T> documents)
		{
			this.Documents = documents;
			this.Index = typeof(T);
			this.Type = typeof(T);
		}
	}

	public class BulkAllDescriptor<T> : DescriptorBase<BulkAllDescriptor<T>, IBulkAllRequest<T>>, IBulkAllRequest<T>
		where T : class
	{
		private readonly IEnumerable<T> _documents;

		Time IBulkAllRequest<T>.BackOffTime { get; set; }
		int? IBulkAllRequest<T>.Size { get; set; }
		int? IBulkAllRequest<T>.BackOffRetries { get; set; }
		int? IBulkAllRequest<T>.MaxDegreeOfParallelism { get; set; }
		IEnumerable<T> IBulkAllRequest<T>.Documents => this._documents;

		IndexName IBulkAllRequest<T>.Index { get; set; }
		TypeName IBulkAllRequest<T>.Type { get; set; }
		Consistency? IBulkAllRequest<T>.Consistency { get; set; }
		bool? IBulkAllRequest<T>.Refresh { get; set; }
		bool IBulkAllRequest<T>.RefreshOnCompleted { get; set; }
		string IBulkAllRequest<T>.Routing { get; set; }
		Time IBulkAllRequest<T>.Timeout { get; set; }
		string IBulkAllRequest<T>.Pipeline { get; set; }
		Action<BulkDescriptor, IList<T>>  IBulkAllRequest<T>.BufferToBulk { get; set; }

		public BulkAllDescriptor(IEnumerable<T> documents)
		{
			this._documents = documents;
			((IBulkAllRequest<T>)this).Index = typeof(T);
			((IBulkAllRequest<T>)this).Type = typeof(T);
		}

		/// <inheritdoc />
		public BulkAllDescriptor<T> MaxDegreeOfParallelism(int? parallism) =>
			Assign(a => a.MaxDegreeOfParallelism = parallism);

		/// <inheritdoc />
		public BulkAllDescriptor<T> Size(int? size) => Assign(a => a.Size = size);

		/// <inheritdoc />
		public BulkAllDescriptor<T> BackOffRetries(int? backoffs) =>
			Assign(a => a.BackOffRetries = backoffs);

		/// <inheritdoc />
		public BulkAllDescriptor<T> BackOffTime(Time time) => Assign(a => a.BackOffTime = time);

		/// <inheritdoc />
		public BulkAllDescriptor<T> Index(IndexName index) => Assign(a => a.Index = index);

		/// <inheritdoc />
		public BulkAllDescriptor<T> Index<TOther>() where TOther : class => Assign(a => a.Index = typeof(TOther));

		/// <inheritdoc />
		public BulkAllDescriptor<T> Type(TypeName type) => Assign(a => a.Type = type);

		/// <inheritdoc />
		public BulkAllDescriptor<T> Type<TOther>() where TOther : class => Assign(a => a.Type = typeof(TOther));

		/// <inheritdoc />
		public BulkAllDescriptor<T> RefreshOnCompleted(bool refresh = true) => Assign(p => p.RefreshOnCompleted = refresh);

		/// <inheritdoc />
		public BulkAllDescriptor<T> Refresh(bool refresh = true) => Assign(p => p.Refresh = refresh);

		/// <inheritdoc />
		public BulkAllDescriptor<T> Routing(string routing) => Assign(p => p.Routing = routing);

		/// <inheritdoc />
		public BulkAllDescriptor<T> Timeout(Time timeout) => Assign(p => p.Timeout = timeout);

		/// <inheritdoc />
		public BulkAllDescriptor<T> Pipeline(string pipeline) => Assign(p => p.Pipeline = pipeline);

		/// <inheritdoc />
		public BulkAllDescriptor<T> BufferToBulk(Action<BulkDescriptor, IList<T>> modifier) => Assign(p => p.BufferToBulk = modifier);

	}
}
