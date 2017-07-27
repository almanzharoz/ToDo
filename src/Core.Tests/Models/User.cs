using Core.ElasticSearch.Domain;
using Nest;

namespace Core.Tests.Models
{
    public class User : BaseEntityWithVersion, IModel, IProjection<User>, IGetProjection, IInsertProjection
    {
        public string Login { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
    }
}