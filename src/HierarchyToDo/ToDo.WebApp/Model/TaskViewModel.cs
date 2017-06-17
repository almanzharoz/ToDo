using System;
using SharpFuncExt;
using ToDo.Dal.Projections;

namespace ToDo.WebApp.Model
{
	public class TaskViewModel
	{
		public string Id { get; }
		public string Name { get; }

		public SimpleUserModel User { get; }
		public SimpleUserModel Assign { get; }
		public DateTime Created { get; }

		public DateTime Deadline { get; }
		public int EstimatedTime { get; }

		public int ChildrenCount { get; }

		public TaskViewModel(Task task, int childrenCount)
		{
			Id = task.Id;
			User = new SimpleUserModel(task.User);
			Name = task.Name;
			Assign = task.Assign.IfNotNullOrDefault(x => new SimpleUserModel(x));
			Created = task.Created;
			Deadline = task.Deadline;
			EstimatedTime = task.EstimatedTime;
			ChildrenCount = childrenCount;
		}
	}
}