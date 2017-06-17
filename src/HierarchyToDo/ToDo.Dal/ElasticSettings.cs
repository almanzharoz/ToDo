using System;
using System.Collections.Generic;
using System.Text;
using Core.ElasticSearch;

namespace ToDo.Dal
{
    public class ElasticSettings : BaseElasticSettings
    {
	    public ElasticSettings(Uri url, string indexName) : base(url, indexName)
	    {
	    }
    }
}
