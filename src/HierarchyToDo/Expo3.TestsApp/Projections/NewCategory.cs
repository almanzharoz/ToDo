using Core.ElasticSearch.Domain;
using Expo3.Model.Helpers;
using Expo3.Model.Models;
using SharpFuncExt;

namespace Expo3.TestsApp.Projections
{
	internal class NewCategory : BaseNewEntity, IProjection<Category>
	{
		public string Url { get; }
		public string Name { get; }

		public NewCategory() { } //Hack for where new()

		public NewCategory(string url, string name)
		{
			Name = name.HasNotNullArg(nameof(name));
			Url = url.IfNull(() => CommonHelper.UriTranslit(name));
		}

	}
}