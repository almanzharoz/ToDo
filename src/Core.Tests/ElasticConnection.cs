using System;
using Expo3.Model;

namespace Core.Tests
{
	public class ElasticConnection : Expo3ElasticConnection
	{
		public ElasticConnection() : base(new Uri("http://localhost:9200"))
		{
		}
	}
}