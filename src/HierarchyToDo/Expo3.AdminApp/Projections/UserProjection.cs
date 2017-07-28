using Core.ElasticSearch.Domain;
using Expo3.Model;
using Newtonsoft.Json;

namespace Expo3.AdminApp.Projections
{
    public class UserProjection : BaseEntityWithVersion, IProjection<User>, IGetProjection
    {
        [JsonProperty]
        public string Email { get; private set; }
        [JsonProperty]
        public string Nickname { get; private set; }
        [JsonProperty]
        public string Password { get; private set; }
        [JsonProperty]
        public string Salt { get; private set; }
        [JsonProperty]
        public EUserRole Role { get; private set; }
    }

    public class UserName
    {
        public string Id { get; }

        public UserName(string id)
        {
            Id = id;
        }

        public UserName(User user)
        {
            Id = user.Id;
        }
    }
}