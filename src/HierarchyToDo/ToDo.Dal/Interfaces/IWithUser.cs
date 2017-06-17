using ToDo.Dal.Projections;

namespace ToDo.Dal.Interfaces
{
	public interface IWithUser
	{
		User User { get; set; }
	}
}