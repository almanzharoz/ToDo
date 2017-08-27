using Core.ElasticSearch.Domain;
using Expo3.Model.Helpers;
using Expo3.Model.Models;
using Newtonsoft.Json;
using SharpFuncExt;

namespace Expo3.AdminApp.Projections
{
	public class CategoryProjection : BaseEntityWithVersion, IProjection<Category>, IRemoveProjection,
		IGetProjection, IUpdateProjection, ISearchProjection
	{
		[JsonProperty]
		public string Url { get; private set; }
		[JsonProperty]
		public string Name { get; private set; }

		internal CategoryProjection Rename(string name, string url)
		{
			Url = url.IfNull(name, CommonHelper.UriTranslit);
			Name = name;
			return this;
		}
	}

	internal class NewCategory : BaseNewEntity, IProjection<Category>
	{
		public string Url { get; }
		public string Name { get; }

		public NewCategory() { } //Hack for where new()

		public NewCategory(string url, string name)
		{
			Url = url.IfNull(name, CommonHelper.UriTranslit);
			Name = name;
		}

	}
}