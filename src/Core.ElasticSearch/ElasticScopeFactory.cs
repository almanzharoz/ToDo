using System;
using Core.ElasticSearch.Mapping;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Core.ElasticSearch
{
	public class ElasticScopeFactory<TSettings>
		where TSettings : BaseElasticSettings
	{
		internal ElasticClient<TSettings> Client { get; }
		internal RequestContainer<TSettings> Container { get; }
		internal ElasticMapping<TSettings> Mapping { get; }

		public ElasticScopeFactory(IServiceProvider servise, TSettings settings)
		{
			Mapping = servise.GetService<ElasticMapping<TSettings>>();
			Container = new RequestContainer<TSettings>(Mapping);
			Client = new ElasticClient<TSettings>(settings, Mapping, Container);
		}
	}
}