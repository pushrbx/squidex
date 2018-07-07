// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschränkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Squidex.Config.Authentication
{
    public static class AuthenticationServices
    {
        public static void AddMyAuthentication(this IServiceCollection services, IConfiguration config)
        {
            var identityOptions = config.GetSection("identity").Get<MyIdentityOptions>();

            if (!identityOptions.UseExternalIdentityProvider)
            {
                services.AddAuthentication()
                    .AddMyGoogleAuthentication(identityOptions)
                    .AddMyMicrosoftAuthentication(identityOptions)
                    .AddMyIdentityServerAuthentication(identityOptions, config)
                    .AddCookie();
            }
            else
            {
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(o =>
                {
                    o.Authority = identityOptions.AuthorityUrl;
                    o.Audience = Constants.Audiance;
                    o.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateIssuerSigningKey = true,
                        // todo: check if the issuer only the domain name, or the full url of the realm in keycloak
                        ValidIssuer = identityOptions.AuthorityUrl,
                        ValidateAudience = true,
                        ValidAudience = Constants.Audiance,
                        ValidateLifetime = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["keycloak:signingKey"]))
                    };
                    #if DEBUG
                    o.RequireHttpsMetadata = false; // if dev, for prod this is true
                    #else
                    o.RequireHttpsMetadata = true;
                    #endif
                });
            }
        }
    }
}