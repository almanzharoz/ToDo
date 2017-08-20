using System;
using System.Collections.Generic;
using System.Linq;
using Core.ElasticSearch;
using Expo3.ClientApp.Projections;
using Expo3.ClientApp.Projections.Event;
using Expo3.Model;
using Expo3.Model.Embed;
using Expo3.Model.Models;
using Microsoft.Extensions.Logging;
using Nest;
using SharpFuncExt;

namespace Expo3.ClientApp.Services
{
    public class CategoryService : BaseExpo3Service
    {
        public CategoryService(ILoggerFactory loggerFactory, Expo3ElasticConnection settings,
            ElasticScopeFactory<Expo3ElasticConnection> factory, UserName user) : base(loggerFactory, settings, factory, user)
        {
        }

        public BaseCategoryProjection GetCategoryById(string id)
            => Get<BaseCategoryProjection>(id.HasNotNullArg("category id"));

        public IReadOnlyCollection<BaseCategoryProjection> SearchCategories(string name = null)
        {
            if (!string.IsNullOrEmpty(name))
                return Search<Category, BaseCategoryProjection>(q => q
                    .Match(m => m
                        .Field(x => x.Name)
                        .Query(name)));
            return Search<Category, BaseCategoryProjection>(q => q);
        }
    }
}