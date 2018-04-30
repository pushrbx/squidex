// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Generic;
using System.Threading.Tasks;
using Squidex.Domain.Users.Keycloak.Models;

namespace Squidex.Domain.Users.Keycloak
{
    public interface IKeycloakClient
    {
        Task<UserRepresentation> GetUserAsync(string userId);

        Task<string> GetUserIdAsync(string username);

        Task<UserRepresentation> CreateUserAsync(UserRepresentation userRepresentation);

        Task<List<ClientRepresentation>> GetClientsAsync();

        Task<List<UserRepresentation>> GetUsersAsync(Dictionary<string, string> query = null);
    }
}