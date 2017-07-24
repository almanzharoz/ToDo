using System;
using System.Collections.Generic;
using System.Text;
using Core.ElasticSearch;
using Nest;

namespace ToDo.Dal
{
    public class ElasticSettings : BaseElasticSettings
    {
	    public ElasticSettings(Uri url) : base(url)
	    {
	    }

		public readonly string IndexName = "todo";
    }
}
