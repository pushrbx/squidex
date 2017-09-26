// ==========================================================================
//  PluginManager.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using System.Collections.Generic;
using System.Composition;
using System.Composition.Convention;
using System.Composition.Hosting;
using Squidex.Extensibility;

namespace Squidex.Infrastructure.Composition
{
    public class PluginManager : IPluginManager
    {
        [ImportMany]
        public IEnumerable<ISquidexPlugin> Plugins { get; private set; }

        public void ComposePlugins(string pluginsFolder)
        {
            var conventions = new ConventionBuilder();
            conventions
                .ForTypesDerivedFrom<ISquidexPlugin>()
                .Export<ISquidexPlugin>()
                .Shared();

            var configuration = new ContainerConfiguration()
                .WithAssembliesInPath(pluginsFolder, conventions); // todo: if its a relative path, change it to absolute path

            using (var container = configuration.CreateContainer())
            {
                this.Plugins = container.GetExports<ISquidexPlugin>();
            }
        }
    }
}
