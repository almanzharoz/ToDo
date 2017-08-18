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
	public class TestsEventService: BaseExpo3Service
	{
		public TestsEventService(ILoggerFactory loggerFactory, Expo3ElasticConnection settings, ElasticScopeFactory<Expo3ElasticConnection> factory, UserName user) : base(loggerFactory, settings, factory, user)
		{
		}

		public string AddUser(string name)
			=> new NewUserProjection {Name = name}.Fluent(Insert).Id;

		public string AddEvent(string name, EventDateTime dateTime, Address address, EEventType type, string categoryId)
		{
			var category = Get<BaseCategoryProjection>(categoryId.HasNotNullArg(nameof(categoryId))).HasNotNullArg("category");

			return new NewEventProjection(Get<BaseUserProjection>(User.Id).HasNotNullArg("owner"))
				{
					Name = name,
					DateTime = dateTime,
					Address = address,
					Type = type,
					Category = category,
					Page = new EventPage() {Address = address, Category = category.Name, Caption = name, Date = dateTime.ToString()}
				}
				.Fluent(Insert)
				.Id;
		}

		public string AddCategory(string name)
			=> new NewCategoryProjection { Name = name }.Fluent(Insert).Id;
	}
}