using System.Runtime.Serialization;
using Core.ElasticSearch.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ToDo.Dal.Models
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum EUserRole
	{
		[EnumMember(Value = "anonym")]
		Anonym = 1,
		[EnumMember(Value = "user")]
		User = 2,
		[EnumMember(Value = "manager")]
		Manager,
		[EnumMember(Value = "admin")]
		Admin
	}
}