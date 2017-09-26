using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Squidex.Domain.Apps.Read.Apps;
using Squidex.Domain.Apps.Read.Contents;
using Squidex.Extensibility;

namespace Squidex.Infrastructure.Composition
{
    public class ContentOperationService : IOperationService
    {
        private readonly IPluginManager pluginManager;
        private readonly IContentQueryService contentQuery;

        public ContentOperationService(IPluginManager pluginManager, IContentQueryService contentQuery)
        {
            this.pluginManager = pluginManager;
            this.contentQuery = contentQuery;
        }

        public async Task<IContentOperation> FindOperationAsync(string operationName, string schemaNameOrId, IAppEntity app)
        {
            IContentOperation result = null;
            var schema = await this.contentQuery.FindSchemaAsync(app, schemaNameOrId);

            if (schema != null)
            {
                foreach (var plugin in this.pluginManager.Plugins)
                {
                    if (plugin.TargetSchema == schema.Name && plugin.TargetAppName == app.Name)
                    {
                        result = plugin.Operation;
                    }
                }
            }

            return result;
        }
    }
}
