using System;
using Core.ElasticSearch.Domain;
using Expo3.Model;
using Expo3.Model.Embed;
using Expo3.Model.Models;
using Newtonsoft.Json;

namespace Expo3.ClientApp.Projections
{
	public class OnlyAddressEventProjections : BaseEntityWithVersion, IProjection<Event>, ISearchProjection
	{
		[JsonProperty]
		public Address Address { get; private set; }
	}
}