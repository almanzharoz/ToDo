using System;
using Nest;
using ToDo.Dal.Interfaces;

namespace ToDo.Dal.Models
{
	public struct TaskState : IWithFiles
	{
		public DateTime Created { get; set; }
		[Keyword]
		public User User { get; set; }
		public ERecordState State { get; set; }

		public string Note { get; set; }
		public string[] Files { get; set; }
	}
}