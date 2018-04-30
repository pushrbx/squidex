// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using RestSharp;
using Squidex.Domain.Users.Keycloak.Models;
using Squidex.Infrastructure;

namespace Squidex.Domain.Users.Keycloak
{
    public class KeycloakClient : IKeycloakClient
    {
        private readonly IRestClient _httpClient;

        public KeycloakClient(IRestClient httpClient)
        {
            this._httpClient = httpClient;
        }

        public async Task<UserRepresentation> GetUserAsync(string userId)
        {
            Guard.NotNullOrEmpty(userId, nameof(userId));
            var request = new RestRequest($"/users/{userId}", Method.GET);
            var response = await this._httpClient.ExecuteTaskAsync<UserRepresentation>(request);
            if (response.IsSuccessful && response.StatusCode == HttpStatusCode.OK)
            {
                return response.Data;
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                // todo: log it
            }

            return null;
        }

        public async Task<string> GetUserIdAsync(string username)
        {
            string result;
            var request = new RestRequest("/users", Method.GET);
            request.AddQueryParameter("username", username);
            var response = await this._httpClient.ExecuteTaskAsync<List<UserRepresentation>>(request);
            if (response.IsSuccessful && response.StatusCode == HttpStatusCode.OK && response.Data.Count > 0)
            {
                var elem = response.Data.FirstOrDefault(x => x.Username == username);
                result = elem?.Id;
            }
            else
            {
                result = null;
            }

            return result;
        }

        public Task<UserRepresentation> CreateUserAsync(UserRepresentation userRepresentation)
        {
            throw new System.NotImplementedException();
        }

        public Task<List<ClientRepresentation>> GetClientsAsync()
        {
            return GetClientsAsyncCore();
        }

        public Task<List<ClientRepresentation>> GetClientsAsync(string clientId)
        {
            var query = new Dictionary<string, string>()
            {
                {"clientId", clientId}
            };

            return GetClientsAsyncCore(query);
        }

        private async Task<List<ClientRepresentation>> GetClientsAsyncCore(Dictionary<string, string> query = null)
        {
            var request = new RestRequest("/clients", Method.GET);

            AddQueryParamsToRequest(request, query);
            
            var response = await this._httpClient.ExecuteTaskAsync<List<ClientRepresentation>>(request);
            if (response.IsSuccessful && response.StatusCode == HttpStatusCode.OK)
            {
                return response.Data;
            }
            
            return new List<ClientRepresentation>();
        }

        public async Task<List<UserRepresentation>> GetUsersAsync(Dictionary<string, string> query = null)
        {
            var request = new RestRequest("/users", Method.GET);

            AddQueryParamsToRequest(request, query);
            
            var response = await this._httpClient.ExecuteTaskAsync<List<UserRepresentation>>(request);
            if (response.IsSuccessful && response.StatusCode == HttpStatusCode.OK)
            {
                return response.Data;
            }

            return new List<UserRepresentation>();
        }

        public async Task<List<RoleRepresentation>> GetClientLevelRoleMappingsForUser(string userId, string clientId)
        {
            Guard.NotNullOrEmpty(userId, nameof(userId));
            Guard.NotNullOrEmpty(clientId, nameof(clientId));
            
            var request = new RestRequest($"/users/{userId}/role-mappings/clients/{clientId}", Method.GET);
            var response = await this._httpClient.ExecuteTaskAsync<List<RoleRepresentation>>(request);
            
            if (response.IsSuccessful && response.StatusCode == HttpStatusCode.OK)
            {
                return response.Data;
            }

            return new List<RoleRepresentation>();
        }

        public async Task<List<RoleRepresentation>> GetRealmLevelRoleMappingsForUser(string userId)
        {
            Guard.NotNullOrEmpty(userId, nameof(userId));
            var request = new RestRequest($"/users/{userId}/role-mappings/realm", Method.GET);
            var response = await this._httpClient.ExecuteTaskAsync<List<RoleRepresentation>>(request);
            
            if (response.IsSuccessful && response.StatusCode == HttpStatusCode.OK)
            {
                return response.Data;
            }
            
            return new List<RoleRepresentation>();
        }

        private void AddQueryParamsToRequest(RestRequest request, Dictionary<string, string> query)
        {
            if (query != null)
            {
                foreach (var param in query)
                {
                    request.AddQueryParameter(param.Key, param.Value);
                }
            }
        }
    }
}