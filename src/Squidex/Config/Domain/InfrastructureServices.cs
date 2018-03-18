﻿// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschränkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Squidex.Infrastructure.UsageTracking;

#pragma warning disable RECS0092 // Convert field to readonly

namespace Squidex.Config.Domain
{
    public static class InfrastructureServices
    {
        public static void AddMyInfrastructureServices(this IServiceCollection services)
        {
            services.AddSingletonAs(SystemClock.Instance)
                .As<IClock>();

            services.AddSingletonAs<BackgroundUsageTracker>()
                .As<IUsageTracker>();

            services.AddSingletonAs<HttpContextAccessor>()
                .As<IHttpContextAccessor>();

            services.AddSingletonAs<ActionContextAccessor>()
                .As<IActionContextAccessor>();
        }
    }
}
