using System.ComponentModel.DataAnnotations;
using ToDo.Dal.Models;
using User = ToDo.Dal.Projections.User;

namespace ToDo.WebApp.Model
{
	public class ProjectEditModel : BaseEditModel<Dal.Models.Project>
	{
		[Required]
		public string Name { get; set; }
		public SimpleUserModel User { get; private set; }

		public ProjectEditModel() { }

		public ProjectEditModel(Dal.Models.Project project) : base(project)
		{
			Name = project.Name;
			User = new SimpleUserModel(project.User);
		}

		public override Project Update(Project project)
		{
			project.Name = Name;
			return project;
		}
	}
}