using System;
using Core.ElasticQueryBuilder.QueryDsl;
using Newtonsoft.Json;

namespace Core.ElasticQueryBuilder.Commands
{
	public class SearchCommand<T> : BaseCommand
	{
		private string _url = "_search";

		public SearchCommand<T> Query(Func<Query<T>, Query<T>> func)
		{
			AddItem(func(new Query<T>()));
			return this;
		}

		public SearchCommand() : base("Search")
		{
		}

	}
}