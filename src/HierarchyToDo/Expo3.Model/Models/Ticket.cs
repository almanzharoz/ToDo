﻿using Core.ElasticSearch.Domain;
using Nest;

namespace Expo3.Model.Models
{
	public class Ticket : BaseEntity
	{
		[Keyword]
		public Event Event { get; set; }
		public Visitor Visitor { get; set; }
	}
}