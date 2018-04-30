// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Threading.Tasks;

namespace Squidex.Domain.Users.Keycloak
{
    public interface IKeycloakUserClaimMapper
    {
        Task<KeycloakUser> MapClaimsAsync(KeycloakUser user);
    }
}