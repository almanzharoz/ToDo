using BeeFee.Model.Embed;
using BeeFee.Model.Helpers;
using Core.ElasticSearch.Domain;
using Microsoft.AspNetCore.WebUtilities;

namespace BeeFee.LoginApp.Projections.User
{
	internal class UpdatePasswordProjection : BaseEntity, IProjection<Model.Models.User>, IUpdateProjection, IGetProjection
	{
		public string Password { get; private set; }
		public string Salt { get; private set; }
		public string Name { get; private set; }
		public EUserRole[] Roles { get; private set; }

		internal UpdatePasswordProjection ChangePassword(/*string oldPassword, */string newPassword)
		{
			Password = HashPasswordHelper.GetHash(newPassword, Base64UrlTextEncoder.Decode(Salt));
			return this;
		}

	}
}