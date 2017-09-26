using Squidex.Domain.Apps.Read.Schemas;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Squidex.Domain.Apps.Read.Apps;
using Squidex.Domain.Apps.Read.Contents;

namespace Squidex.Extensibility
{
    public interface IContentOperation
    {
        string Name { get; }

        Task<(ISchemaEntity Schema, long Total, IReadOnlyList<IContentEntity> Items)> ExecuteAsync(
            IContentQueryService queryService, IAppEntity app, ClaimsPrincipal user);
    }
}
