﻿using Core.ElasticSearch.Domain;
using Newtonsoft.Json;

namespace Core.Tests.Projections
{
    public class UserUpdateProjection: BaseEntityWithVersion, IProjection<Models.User>, IGetProjection, IUpdateProjection
    {
        [JsonProperty]
        public string Login { get; private set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}