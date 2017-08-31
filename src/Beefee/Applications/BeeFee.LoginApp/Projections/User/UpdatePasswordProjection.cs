using BeeFee.Model.Embed;
using BeeFee.Model.Helpers;
using Core.ElasticSearch.Domain;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;

namespace BeeFee.LoginApp.Projections.User
{
	internal class UpdatePasswordProjection : BaseEntity, IProjection<Model.Models.User>, IUpdateProjection, IGetProjection
	{
		[JsonProperty]
		public string Password { get; private set; }
		[JsonProperty]
		public string Salt { get; private set; }
		[JsonProperty]
		public string Name { get; private set; }
		[JsonProperty]
		public EUserRole[] Roles { get; private set; }

		internal UpdatePasswordProjection ChangePassword(/*string oldPassword, */string newPassword)
		{
			Password = HashPasswordHelper.GetHash(newPassword, Base64UrlTextEncoder.Decode(Salt));
			return this;
		}

	}
}