using System;
using System.Threading.Tasks;
using Nest252;

namespace Profiling.Async
{
	public class GetScriptAsync : IAsyncProfiledOperation
	{
		public Task RunAsync(IElasticClient client, ColoredConsoleWriter output)
		{
			throw new NotImplementedException();
		}
	}
}