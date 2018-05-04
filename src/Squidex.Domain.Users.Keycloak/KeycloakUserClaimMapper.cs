// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Squidex.Domain.Users.Keycloak.Models;
using Squidex.Infrastructure;
using Squidex.Shared.Identity;

namespace Squidex.Domain.Users.Keycloak
{
    public class KeycloakUserClaimMapper : IKeycloakUserClaimMapper
    {
        private readonly IKeycloakClient _client;
        private readonly KeycloakOptions _options;
        // the client id of the admin frontend in keycloak
        private string _cachedClientId;

        public KeycloakUserClaimMapper(IKeycloakClient client, IOptions<KeycloakOptions> options)
        {
            Guard.NotNull(client, nameof(client));
            Guard.NotNull(options, nameof(options));
            
            _client = client;
            _options = options.Value;
        }
        
        public async Task<KeycloakUser> MapClaimsAsync(KeycloakUser user)
        {
            var clientId = await GetClientId();
            if (clientId == null)
                return user;

            var clientRoles = await _client.GetClientLevelRoleMappingsForUser(user.Id, clientId);
            var realmRoles = await _client.GetRealmLevelRoleMappingsForUser(user.Id);

            if (user.Claims.All(x => x.Type != "realm_access"))
            {
                var realmAccessClaim = new Claim("realm_access",
                    JsonConvert.SerializeObject(new {roles = realmRoles.Select(x => x.Name).ToList()}));
                
                user.AddClaim(realmAccessClaim);
            }

            if (user.Claims.All(x => x.Type != "resource_access"))
            {
                var resourceAccessClaim = new Claim("resource_access",
                    JsonConvert.SerializeObject(new Dictionary<string, object>()
                    {
                        {
                            _options.ClientNameForAdminFrontend, new
                            {
                                roles = clientRoles.Select(x => x.Name).ToList()
                            }
                        }
                    }));
                
                user.AddClaim(resourceAccessClaim);
            }

            if (user.Claims.All(x => x.Type != SquidexClaimTypes.SquidexDisplayName))
            {
                user.AddClaim(new Claim(SquidexClaimTypes.SquidexDisplayName, $"{user.Representation.FirstName} {user.Representation.LastName}"));
            }
            
            if (user.Claims.All(x => x.Type != "name"))
            {
                user.AddClaim(new Claim("name", $"{user.Representation.FirstName} {user.Representation.LastName}"));
            }

            if (user.Claims.All(x => x.Type != "preferred_username"))
            {
                user.AddClaim(new Claim("preferred_username", user.Representation.Username));
            }

            if (user.Claims.All(x => x.Type != "email"))
            {
                user.AddClaim(new Claim("email", user.Email));
            }

            return user;
        }

        private async Task<string> GetClientId()
        {
            if (!string.IsNullOrEmpty(_cachedClientId)) return _cachedClientId;
            var clients = await _client.GetClientsAsync(_options.ClientNameForAdminFrontend);
            if (clients == null || clients.Count == 0)
                return null;

            var client = clients.FirstOrDefault(x => x.ClientId == _options.ClientNameForAdminFrontend);
            if (client == null)
                return null;

            _cachedClientId = client.Id;

            return _cachedClientId;
        }
    }
}