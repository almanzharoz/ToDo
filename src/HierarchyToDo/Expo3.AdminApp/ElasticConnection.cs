using System;
using Core.ElasticSearch;

namespace Expo3.AdminApp
{
    public class ElasticConnection : BaseElasticConnection
    {
        public ElasticConnection(Uri url) : base(url)
        {
        }

        public readonly string IndexName = "expo3";
    }
}