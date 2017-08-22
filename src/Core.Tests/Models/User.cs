using Core.ElasticSearch.Domain;
using Nest;

namespace Core.Tests.Models
{
    public class User : BaseEntity, IModel, IProjection<User>, IGetProjection
    {
        public string Login { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
    }

	public class NewUser : BaseNewEntity, IProjection<User>
	{
		public string Login { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
		public string Salt { get; set; }
	}
}