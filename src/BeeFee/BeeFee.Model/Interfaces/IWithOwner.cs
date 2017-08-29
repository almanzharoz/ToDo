using BeeFee.Model.Models;

namespace BeeFee.Model.Interfaces
{
	public interface IWithOwner
	{
		BaseUserProjection Owner { get; }
	}
}