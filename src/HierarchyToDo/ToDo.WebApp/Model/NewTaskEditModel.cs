using System;
using System.ComponentModel.DataAnnotations;
using ToDo.Dal.Projections;

namespace ToDo.WebApp.Model
{
	public class NewTaskEditModel
	{
		[Required]
		public string ProjectId { get; set; }
		[Required]
		public string Name { get; set; }

		public string ParentTaskId { get; set; }
		public string ParentTaskName { get; set; }
		public string Note { get; set; }
		public DateTime Deadline { get; set; }
		public string Assigned { get; set; }
		public int EstimatedTime { get; set; }

		public NewTaskEditModel() { }

		public NewTaskEditModel(string projectId, Task parentTask)
		{
			ProjectId = projectId;
			Init(parentTask);
		}

		public NewTaskEditModel Init(Task parentTask)
		{
			if (parentTask != null)
			{
				ParentTaskId = parentTask.Id;
				ParentTaskName = parentTask.Name;
			}
			return this;
		}
	}
}