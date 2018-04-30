// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Generic;
using System.Threading.Tasks;
using RestSharp;
using Squidex.Domain.Users.Keycloak.Models;

namespace Squidex.Domain.Users.Keycloak
{
    public class KeycloakClient : IKeycloakClient
    {
        private readonly IRestClient _httpClient;

        public KeycloakClient(IRestClient httpClient)
        {
            this._httpClient = httpClient;
        }
        
        public Task<UserRepresentation> GetUserAsync(string userId)
        {
            var request = new RestRequest($"/master/users/{userId}", Method.GET);
            throw new System.NotImplementedException();
        }

        public Task<string> GetUserIdAsync(string username)
        {
            throw new System.NotImplementedException();
        }

        public Task<UserRepresentation> CreateUserAsync(UserRepresentation userRepresentation)
        {
            throw new System.NotImplementedException();
        }

        public Task<List<ClientRepresentation>> GetClientsAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<List<UserRepresentation>> GetUsersAsync(Dictionary<string, string> query = null)
        {
            throw new System.NotImplementedException();
        }
    }
}