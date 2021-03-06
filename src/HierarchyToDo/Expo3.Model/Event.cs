﻿using System;
using Core.ElasticSearch.Domain;
using Expo3.Model.Helpers;
using Expo3.Model.Interfaces;
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
        public Price StartPrice { get; set; }
        public Price FinishPrice { get; set; }
        [Keyword]
        public DateTime StartDateTime { get; set; }
        [Keyword]
        public DateTime FinishDateTime { get; set; }
        [Keyword]
        public Address Address { get; set; }
        public EventPage EventPage { get; set; }
        public EEventType Type { get; set; }
        public string Email { get; set; }
    }
}