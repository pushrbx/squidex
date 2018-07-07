// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschränkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Squidex.Areas.Frontend
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.DependencyInjection;
    using Squidex.Config;

    public static class CustomAdminClientStartup
    {
        public static void ConfigureCustomFrontend(this IServiceCollection services)
        {
            services.AddSingletonAs<CustomAdminClientFilter>().AsSelf();
        }

        public static void ConfigureCustomFrontend(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                var body = context.Response.Body;
                var buffer = new MemoryStream();
                context.Response.Body = buffer;

                await next();

                buffer.Seek(0, SeekOrigin.Begin);

                if (context.Response.StatusCode == 200 && IsIndex(context) && IsHtml(context))
                {
                    buffer.SetLength(1);
                    using (var memoryStream = new MemoryStream())
                    {
                        using (var writer = new StreamWriter(memoryStream))
                        {
                            writer.Write("404");
                            writer.Flush();

                            memoryStream.Seek(0, SeekOrigin.Begin);

                            context.Response.Headers["Content-Length"] = memoryStream.Length.ToString();

                            await memoryStream.CopyToAsync(body);
                        }
                    }

                    context.Response.StatusCode = 404;
                }
            });
        }

        private static bool IsIndex(HttpContext context)
        {
            return context.Request.Path.Value.Equals("/index.html", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsHtml(HttpContext context)
        {
            return context.Response.ContentType?.ToLower().Contains("text/html") == true;
        }
    }

    public class CustomAdminClientFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.HttpContext.Request.Path.HasValue &&
                context.HttpContext.Request.Path.Value.Contains("index.html"))
            {
                context.Result = new NotFoundResult();
                return;
            }

            await next();
        }
    }
}