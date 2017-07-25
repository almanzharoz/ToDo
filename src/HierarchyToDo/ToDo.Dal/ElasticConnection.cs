using System;
using System.Collections.Generic;
using System.Text;
using Core.ElasticSearch;
using Nest;

namespace ToDo.Dal
{
    public class ElasticConnection : BaseElasticConnection
    {
	    public ElasticConnection(Uri url) : base(url)
	    {
	    }

		public readonly string IndexName = "todo";
    }
}
