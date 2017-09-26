using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Squidex.Controllers.ContentApi.Models;
using Squidex.Domain.Apps.Read.Contents;
using Squidex.Infrastructure.Composition;
using Squidex.Infrastructure.CQRS.Commands;
using Squidex.Infrastructure.Reflection;
using Squidex.Pipeline;

namespace Squidex.Controllers.ContentApi
{
    [ApiExceptionFilter]
    [AppApi]
    [SwaggerIgnore]
    public class ContentOperationsController : ControllerBase
    {
        private readonly IOperationService operationService;
        private readonly IContentQueryService contentQuery;

        public ContentOperationsController(ICommandBus commandBus, IOperationService operationService, IContentQueryService contentQuery)
            : base(commandBus)
        {
            this.operationService = operationService;
            this.contentQuery = contentQuery;
        }

        [MustBeAppReader]
        [HttpGet]
        [Route("content/{app}/{name}/operation/{operationName}")]
        [ApiCosts(2)]
        public async Task<IActionResult> ExecuteOperation(string name, string operationName)
        {
            var operation = await operationService.FindOperationAsync(operationName, name, App);

            if (operation == null)
            {
                return NotFound();
            }

            var isFrontendClient = User.IsFrontendClient();
            var contents = await operation.ExecuteAsync(contentQuery, App, User);

            var response = new AssetsDto
            {
                Total = contents.Total,
                Items = contents.Items.Take(200).Select(item =>
                {
                    var itemModel = SimpleMapper.Map(item, new ContentDto());

                    if (item.Data != null)
                    {
                        itemModel.Data = item.Data.ToApiModel(contents.Schema.SchemaDef, App.LanguagesConfig, !isFrontendClient);
                    }

                    return itemModel;
                }).ToArray()
            };

            return Ok(response);
        }
    }
}
