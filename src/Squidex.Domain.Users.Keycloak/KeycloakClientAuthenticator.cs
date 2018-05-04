// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Net;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;
using Squidex.Domain.Users.Keycloak.Models;
using Squidex.Infrastructure;

namespace Squidex.Domain.Users.Keycloak
{
    /// <summary>
    /// Authenticates agains the Keycloak Admin API
    /// </summary>
    /// <remarks>
    /// https://www.keycloak.org/docs/latest/server_development/index.html#admin-rest-api
    /// </remarks>
    public class KeycloakClientAuthenticator : IAuthenticator
    {
        private readonly string _baseUrl;
        private readonly string _adminUserName;
        private readonly string _adminPassword;
        private readonly KeycloakClientToken _token;

        public KeycloakClientAuthenticator(IOptions<KeycloakOptions> options)
        {
            Guard.NotNull(options, nameof(options));
            this._baseUrl = options.Value?.RealmBaseUrl;
            this._adminUserName = options.Value?.AdminUserName;
            this._adminPassword = options.Value?.AdminPassword;
            this._token = new KeycloakClientToken();
        }

        public void Authenticate(IRestClient client, IRestRequest request)
        {
            if (IsTokenExpired())
            {
                UpdateToken();
            }

            request.Parameters.RemoveAll(x => x.Type == ParameterType.HttpHeader && x.Name == "Authorization");
            request.AddHeader("Authorization", $"Bearer {this._token.Token}");
        }

        private bool IsTokenExpired()
        {
            return this._token.ExpiresAt <= DateTime.UtcNow;
        }

        private void UpdateToken()
        {
            var tokenClient = new RestClient()
            {
                BaseUrl = new Uri(this._baseUrl)
            };

            var tokenRequest = new RestRequest("/protocol/openid-connect/token", Method.POST);
            tokenRequest.AddHeader("cache-control", "no-cache");
            tokenRequest.AddHeader("content-type", "application/x-www-form-urlencoded");
            tokenRequest.AddParameter("client_id", "admin-cli");

            if (string.IsNullOrEmpty(this._token.RefreshToken) || this._token.RefreshTokenExpiresAt <= DateTime.UtcNow)
            {
                tokenRequest.AddParameter("grant_type", "password");
                tokenRequest.AddParameter("username", this._adminUserName);
                tokenRequest.AddParameter("password", this._adminPassword);
            }
            else if (!string.IsNullOrEmpty(this._token.RefreshToken) &&
                     this._token.RefreshTokenExpiresAt >= DateTime.UtcNow)
            {
                tokenRequest.AddParameter("grant_type", "refresh_token");
                tokenRequest.AddParameter("refresh_token", this._token.RefreshToken);
            }

            var response = tokenClient.Execute(tokenRequest);
            if (response.IsSuccessful && response.StatusCode == HttpStatusCode.OK)
            {
                var ro = JsonConvert.DeserializeObject<JObject>(response.Content);
                this._token.Token = ro["access_token"].Value<string>();
                this._token.ExpiresAt = DateTime.UtcNow.AddSeconds(ro["expires_in"].Value<int>());
                this._token.RefreshToken = ro["refresh_token"].Value<string>();
                this._token.RefreshTokenExpiresAt = DateTime.UtcNow.AddSeconds(ro["refresh_expires_in"].Value<int>());
            }
            else
            {
                throw new Exception("Couldn't authenticate with keycloak.");
            }
        }
    }
}