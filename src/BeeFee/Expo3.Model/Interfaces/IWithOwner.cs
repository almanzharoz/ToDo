using Expo3.Model.Models;

namespace Expo3.Model.Interfaces
{
	public interface IWithOwner
	{
		BaseUserProjection Owner { get; }
	}
}