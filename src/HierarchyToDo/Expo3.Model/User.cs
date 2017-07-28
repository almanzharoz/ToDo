using System;
using Core.ElasticSearch.Domain;
using Nest;

namespace Expo3.Model
{
    public class User : BaseEntityWithVersion, IModel, IProjection
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
