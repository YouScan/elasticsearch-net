﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Elasticsearch252.Net;
using FluentAssertions;
using Tests.Framework;

namespace Tests.ClientConcepts.ConnectionPooling.RoundRobin
{
	public class RoundRobin
	{
		/** == Round Robin
		 * <<sniffing-connection-pool, Sniffing>> and <<static-connection-pool, Static>> connection pools
		 * round robin over the `live` nodes to evenly distribute request load over all known nodes.
		 */

		/** === GetNext
		* `GetNext` is implemented in a lock free thread safe fashion, meaning each callee gets returned its own cursor to advance
		* over the internal list of nodes. This to guarantee each request that needs to fall over tries all the nodes without
		* suffering from noisy neighbours advancing a global cursor.
		*/
		protected int NumberOfNodes = 10;

		[U] public void EachViewStartsAtNexPositionAndWrapsOver()
		{
			var uris = Enumerable.Range(9200, NumberOfNodes).Select(p => new Uri("http://localhost:" + p));
			var staticPool = new StaticConnectionPool(uris, randomize: false);
			var sniffingPool = new SniffingConnectionPool(uris, randomize: false);

			this.AssertCreateView(staticPool);
			this.AssertCreateView(sniffingPool);
		}

		public void AssertCreateView(IConnectionPool pool)
		{
			/**
			* Here we have setup a static connection pool seeded with 10 nodes. We force randomization OnStartup to false
			* so that we can test the nodes being returned are int the order we expect them to.
			* So what order we expect? Imagine the following:
			*
			* Thread A calls GetNext first without a local cursor and takes the current from the internal global cursor which is 0.
			* Thread B calls GetNext() second without a local cursor and therefor starts at 1.
			* After this each thread should walk the nodes in successive order using their local cursor
			* e.g Thread A might get 0,1,2,3,5 and thread B will get 1,2,3,4,0.
			*/
			var startingPositions = Enumerable.Range(0, NumberOfNodes)
				.Select(i => pool.CreateView().First())
				.Select(n => n.Uri.Port)
				.ToList();

			var expectedOrder = Enumerable.Range(9200, NumberOfNodes);
			startingPositions.Should().ContainInOrder(expectedOrder);

			/**
			* What the above code just proved is that each call to GetNext(null) gets assigned the next available node.
			*
			* Lets up the ante:
			* - call get next over `NumberOfNodes * 2` threads
			* - on each thread call getnext `NumberOfNodes * 10` times using a local cursor.
			* We'll validate that each thread sees all the nodes and they they wrap over e.g after node 9209
			* comes 9200 again
			*/
			var threadedStartPositions = new ConcurrentBag<int>();
			var threads = Enumerable.Range(0, 20)
				.Select(i => CreateThreadCallingGetNext(pool, threadedStartPositions))
				.ToList();

			foreach (var t in threads) t.Start();
			foreach (var t in threads) t.Join();

			/**
			* Each thread reported the first node it started off lets make sure we see each node twice as the first node
			* because we started `NumberOfNodes * 2` threads
			*/
			var grouped = threadedStartPositions.GroupBy(p => p).ToList();
			grouped.Count().Should().Be(NumberOfNodes);
			grouped.Select(p => p.Count()).Should().OnlyContain(p => p == 2);
		}

		public Thread CreateThreadCallingGetNext(IConnectionPool pool, ConcurrentBag<int> startingPositions) => new Thread(() =>
		{
			/** CallGetNext is a generator that calls GetNext() indefinitely using a local cursor */
			var seenPorts = CallGetNext(pool).Take(NumberOfNodes * 10).ToList();
			var startPosition = seenPorts.First();
			startingPositions.Add(startPosition);
			//first seenNode is e.g 9202 then start counting at 2;
			var i = (startPosition - 9200) % NumberOfNodes;
			foreach (var port in seenPorts)
				port.Should().Be(9200 + (i++ % NumberOfNodes));
		});

		//hide
		private IEnumerable<int> CallGetNext(IConnectionPool pool)
		{
			foreach(var n in pool.CreateView()) yield return n.Uri.Port;
		}
	}
}
