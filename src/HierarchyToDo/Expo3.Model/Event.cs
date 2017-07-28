using System;
using Core.ElasticSearch.Domain;
using Expo3.Model.Interfaces;
using ToDo.Dal.Projections;
using Nest;

namespace Expo3.Model
{
    public class Event : BaseEntityWithVersion, IModel, IProjection, IWithName, IWithCreated, IWithUser
    {
        [Keyword]
        public string Name { get; set; }
        [Keyword]
        public string Caption { get; set; }
        public DateTime Created { get; set; }
        public User User { get; set; }
        public Price Price { get; set; }
        [Keyword]
        public DateTime StartDateTime { get; set; }
        [Keyword]
        public DateTime FinishDateTime { get; set; }
        [Keyword]
        public Address Address { get; set; }
        public string Description { get; set; }
    }
}