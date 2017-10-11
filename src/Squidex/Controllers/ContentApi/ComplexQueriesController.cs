﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Squidex.Controllers.ContentApi.Models;
using Squidex.Domain.Apps.Read.Assets.Repositories;
using Squidex.Domain.Apps.Read.Contents;
using Squidex.Domain.Apps.Read.Contents.CustomQueries;
using Squidex.Infrastructure;
using Squidex.Infrastructure.CQRS.Commands;
using Squidex.Infrastructure.Reflection;
using Squidex.Pipeline;

namespace Squidex.Controllers.ContentApi
{
    [ApiExceptionFilter]
    [AppApi]
    [SwaggerIgnore]
    public class ComplexQueriesController : ControllerBase
    {
        private readonly IContentQueryService contentQuery;
        private readonly IAssetRepository assetsRepository;
        private IList<IQueryModule> plugins;

        public ComplexQueriesController(ICommandBus commandBus, IContentQueryService contentQuery, IAssetRepository assetsRepository, IEnumerable<IQueryModule> plugins)
            : base(commandBus)
        {
            this.contentQuery = contentQuery;
            this.assetsRepository = assetsRepository;
            this.plugins = plugins.ToList();
        }

        [MustBeAppReader]
        [HttpGet]
        [Route("content/{app}/{name}/queries/{queryName}")]
        [ApiCosts(2)]
        public async Task<IActionResult> GetContents(string name, string queryName, [FromQuery] bool archived = false)
        {
            var schema = await contentQuery.FindSchemaAsync(App, name);

            if (schema == null)
            {
                return NotFound();
            }

            IQuery query = null;

            foreach (var squidexPlugin in plugins)
            {
                query = squidexPlugin.GetQueries(App.Name, schema).FirstOrDefault(x => x.Name == queryName);
                if (query != null)
                {
                    break;
                }
            }

            if (query == null)
            {
                return NotFound();
            }

            var context = new QueryContext(App, assetsRepository, contentQuery, User);
            var contents = await query.Execute(schema, context, HttpContext.Request.Query.ToDictionary(x => x.Key, x => (object)x.Value));
            if (contents.Schema != schema)
            {
                throw new ComplexQuerySchemaValidationException("The query has returned an invalid schema.", query.Name);
            }

            var response = new AssetsDto
            {
                Total = contents.Total,
                Items = contents.Items.Take(200).Select(item =>
                {
                    var itemModel = SimpleMapper.Map(item, new ContentDto());

                    if (item.Data != null)
                    {
                        itemModel.Data = item.Data.ToApiModel(contents.Schema.SchemaDef, App.LanguagesConfig, !User.IsFrontendClient());
                    }

                    return itemModel;
                }).ToArray()
            };

            return Ok(response);
        }
    }
}
