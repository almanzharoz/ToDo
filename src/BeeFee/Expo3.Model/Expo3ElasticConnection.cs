using System;
using Core.ElasticSearch;

namespace Expo3.Model
{
	public class Expo3ElasticConnection : BaseElasticConnection
	{
		public Expo3ElasticConnection(Uri url) : base(url)
		{
		}

		public virtual string EventIndexName => "event_index";
		public virtual string UserIndexName => "user_index";
	}
}