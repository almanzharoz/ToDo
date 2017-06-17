using ToDo.Dal.Projections;

namespace ToDo.WebApp.Model
{
	public struct SimpleUserModel
	{
		public string Id { get; set; }
		public string Nick { get; set; }

		public SimpleUserModel(User user)
		{
			Id = user.Id;
			Nick = user.Nick;
		}
	}
}