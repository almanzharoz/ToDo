using System;
using Core.ElasticSearch.Domain;
using Expo3.Model;
using Expo3.Model.Embed;
using Expo3.Model.Interfaces;
using Expo3.Model.Models;
using Nest;
using Newtonsoft.Json;

namespace Expo3.AdminApp.Projections
{
	public class EventProjection : BaseEntityWithVersion, IProjection<Event>, IGetProjection, ISearchProjection, IRemoveProjection, IUpdateProjection, IWithName, IWithOwner
	{
		[Keyword]
		[JsonProperty]
		public BaseCategoryProjection Category { get; private set; }
		[JsonProperty]
		public string Name { get; private set; }
		[JsonProperty]
		public EventDateTime DateTime { get; private set; }
		[JsonProperty]
		public Address Address { get; private set; }
		[JsonProperty]
		public EEventType Type { get; private set; }

		[Keyword]
		[JsonProperty]
		public BaseUserProjection Owner { get; private set; }

		internal EventProjection ChangeCategory(BaseCategoryProjection newCategory)
		{
			Category = newCategory;
			return this;
		}
	}
}