// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestSharp.Authenticators;
using Squidex.Domain.Users;
using Squidex.Domain.Users.Keycloak;
using Squidex.Domain.Users.Keycloak.Models;
using Squidex.Infrastructure;
using Squidex.Shared.Users;

namespace Squidex.Config.Authentication.Keycloak
{
    public static class KeycloakServices
    {
        public static void AddKeycloakAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var keycloakSection = configuration.GetSection("keycloak");
            var keycloakOptions = keycloakSection.Get<KeycloakOptions>();
            if (!keycloakOptions.Enabled)
                return;
            
            if (string.IsNullOrEmpty(keycloakOptions.AdminUserName))
                throw new ConfigurationException("Invalid keycloak configuration: Keycloak authentication is enabled, but the admin user name was empty or null.");
            
            if (string.IsNullOrEmpty(keycloakOptions.AdminPassword))
                throw new ConfigurationException("Invalid keycloak configuration: Keycloak authentication is enabled, but the admin password was empty or null.");
            
            if (string.IsNullOrEmpty(keycloakOptions.ClientNameForAdminFrontend))
                throw new ConfigurationException("Invalid keycloak configuration: Keycloak authentication is enabled, but the client name for the squidex admin frontend was empty or null.");

            services.Configure<KeycloakOptions>(keycloakSection);
            services.AddSingletonAs<KeycloakUserResolver>()
                .As<IUserResolver>();
            services.AddSingletonAs<KeycloakUserFactory>()
                .As<IUserFactory>();
            services.AddSingletonAs<KeycloakClient>()
                .As<IKeycloakClient>();
            services.AddSingletonAs<KeycloakClientAuthenticator>()
                .As<IAuthenticator>();
        }
    }
}