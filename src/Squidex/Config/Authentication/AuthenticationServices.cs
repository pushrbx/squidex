// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschränkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
                    o.RequireHttpsMetadata = false; // if dev, for prod this is true
                    o.Events = new JwtBearerEvents()
                    {
//                        OnAuthenticationFailed = c =>
//                        {
//                            c.NoResult();
//
//                            // c.Response.StatusCode = 500;
//                            c.Response.ContentType = "text/plain";
//                            return c.Response.WriteAsync("An error occured processing your authentication.");
//                        }
                    };
                });
            }
        }
    }
}