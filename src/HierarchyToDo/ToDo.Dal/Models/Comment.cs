using System;
using Core.ElasticSearch.Domain;
using ToDo.Dal.Interfaces;
using Nest;
using Newtonsoft.Json;

namespace ToDo.Dal.Models
{
	public class Comment : BaseEntityWithVersion, IWithUser, IWithCreated, IWithFiles
	{
		[Keyword]
		public Projections.User User { get; set; }
		public DateTime Created { get; set; }
		public bool Deleted { get; set; }

		[Keyword]
		public Projections.Task Task { get; set; }
		[Keyword]
		public Comment Parent { get; set; }

		public string Text { get; set; }
		public string[] Files { get; set; }

	}
}