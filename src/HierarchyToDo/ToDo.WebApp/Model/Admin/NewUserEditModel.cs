using System.ComponentModel.DataAnnotations;
using ToDo.Dal.Models;

namespace ToDo.WebApp.Model.Admin
{
	public class NewUserEditModel
	{
		[Required]
		public string Name { get; set; }
		[Required]
		public string Email { get; set; }
		[Required]
		public string Password { get; set; }
		[Compare("Password")]
		public string ConfirmPassword { get; set; }
		[Required]
		public EUserRole[] Roles { get; set; }
	}
}