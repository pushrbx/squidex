// ==========================================================================
//  CompositionModule.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using Autofac;
using Microsoft.Extensions.Configuration;
using Squidex.Infrastructure.Composition;

namespace Squidex.Config.Domain
{
    public class CompositionModule : Module
    {
        private IConfiguration Configuration { get; }

        public CompositionModule(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var enabled = Configuration.GetValue<bool>("plugins:enabled");

            if (!enabled)
                return;

            var pluginsFolder = Configuration.GetValue<string>("plugins:folder");

            builder.RegisterType<PluginManager>()
                .As<IPluginManager>()
                .SingleInstance()
                .OnActivated(x => x.Instance.ComposePlugins(pluginsFolder));

            builder.RegisterType<ContentOperationService>()
                .As<IOperationService>()
                .SingleInstance();
        }
    }
}
