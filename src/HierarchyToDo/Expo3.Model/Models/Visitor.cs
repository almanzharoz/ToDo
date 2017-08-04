using Core.ElasticSearch.Domain;

namespace Expo3.Model.Models
{
    public class Visitor : BaseEntityWithParent<Event>, IModel
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }
}