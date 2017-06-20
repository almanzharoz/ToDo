using System;
using Core.ElasticSearch;

namespace Core.Tests
{
	public class ElasticSettings : BaseElasticSettings
	{
		public ElasticSettings() : base(new Uri("http://localhost:9200"), "test_index")
		{
		}
	}
}