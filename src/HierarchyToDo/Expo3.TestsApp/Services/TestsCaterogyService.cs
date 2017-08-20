using Core.ElasticSearch;
using Expo3.Model;
using Expo3.Model.Embed;
using Expo3.Model.Exceptions;
using Expo3.Model.Models;
using Expo3.TestsApp.Projections;
using Microsoft.Extensions.Logging;
using SharpFuncExt;

namespace Expo3.TestsApp.Services
{
	public class TestsCaterogyService : BaseExpo3Service
	{
		public TestsCaterogyService(ILoggerFactory loggerFactory, Expo3ElasticConnection settings, ElasticScopeFactory<Expo3ElasticConnection> factory, UserName user) : base(loggerFactory, settings, factory, user)
		{
		}
        
		public string AddCategory(string name)
			=> new NewCategoryProjection { Name = name }.Fluent(Insert).Id;
	}
}