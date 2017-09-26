using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Squidex.Domain.Apps.Read.Apps;
using Squidex.Extensibility;

namespace Squidex.Infrastructure.Composition
{
    /// <summary>
    /// Represents a custom operation for the content api.
    /// </summary>
    public interface IOperationService
    {
        Task<IContentOperation> FindOperationAsync(string operationName, string schemaNameOrId, IAppEntity app);
    }
}
