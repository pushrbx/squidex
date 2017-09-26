using System;
using System.Collections.Generic;
using System.Text;
using Squidex.Extensibility;

namespace Squidex.Infrastructure.Composition
{
    public interface IPluginManager
    {
        IEnumerable<ISquidexPlugin> Plugins { get; }
    }
}
