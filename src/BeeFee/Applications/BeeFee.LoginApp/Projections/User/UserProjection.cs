using BeeFee.Model.Embed;
using BeeFee.Model.Helpers;
using Core.ElasticSearch.Domain;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;

namespace BeeFee.LoginApp.Projections.User
{
	public class UserProjection : BaseEntity, IProjection<Model.Models.User>, IGetProjection, ISearchProjection
	{
		public string Email { get; private set; }
		public string Name { get; private set; }
		[JsonProperty]
		private string Password { get; set; }
		[JsonProperty]
		private string Salt { get; set; }
		public EUserRole[] Roles { get; private set; }

		internal bool CheckPassword(string password)
			=> HashPasswordHelper.GetHash(password, Base64UrlTextEncoder.Decode(Salt)) == Password;
	}
}