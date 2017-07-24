using System;
using Core.ElasticSearch;

namespace Core.Tests
{
	public class ElasticSettings : BaseElasticSettings
	{
		public ElasticSettings() : base(new Uri("http://localhost:9200"))
		{
		}

		public readonly string FirstIndex = "first_test_index";
		public readonly string SecondIndex = "second_test_index";
	}
}