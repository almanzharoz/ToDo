using System;
using System.Collections.Concurrent;

namespace Core.ElasticQueryBuilder
{
	public static class QueryFactory
	{
		private static readonly ConcurrentDictionary<string, BaseCommand> Commands = new ConcurrentDictionary<string, BaseCommand>();

		public static int Count => Commands.Count;

		public static string GetOrAdd<TCommand>(Func<TCommand, TCommand> query) where TCommand : BaseCommand, new()
		{
			var q = query(new TCommand());
			return ((TCommand) Commands.GetOrAdd(q.Hash, k =>
			{
				q.Build();
				return q;
			})).GetJson(q);
		}
	}
}