using Newtonsoft.Json;

namespace Core.ElasticSearch.Domain
{
	public interface IEntity
	{
		[JsonIgnore]
		string Id { get; set; }
	}
}