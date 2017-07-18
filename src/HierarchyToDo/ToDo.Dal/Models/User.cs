using Core.ElasticSearch.Domain;
using Core.ElasticSearch.Serialization;
using Nest;
using Newtonsoft.Json;
using ToDo.Dal.Interfaces;

namespace ToDo.Dal.Models
{
	public class User : BaseEntity, IWithVersion
	{
		[Text(Analyzer = "autocomplete", Fielddata = true)]
		public string Nick { get; set; }
		[Keyword]
		public string Email { get; set; }
		[Keyword]
		public string Password { get; set; }
		[Keyword]
		public EUserRole[] Roles { get; set; }
		public bool Deny { get; set; }
		[JsonIgnore]
		public int Version { get; set; }
	}
}