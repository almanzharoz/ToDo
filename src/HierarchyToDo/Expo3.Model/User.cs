using System;
using Core.ElasticSearch.Domain;
using Expo3.Model.Embed;
using Nest;

namespace Expo3.Model
{
    public class User : BaseEntityWithVersion, IModel, IProjection, IGetProjection, IInsertProjection
    {
        public string Email { get; set; }
        [Keyword]
        public string Nickname { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        [Keyword]
        public EUserRole Role { get; set; }
    }
}
