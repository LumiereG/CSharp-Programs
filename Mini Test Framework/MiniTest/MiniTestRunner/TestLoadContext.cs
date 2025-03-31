using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace MiniTestRunner
{
    public class TestLoadContext : AssemblyLoadContext
    {
        private readonly AssemblyDependencyResolver _resolver;

        public TestLoadContext(string pluginPath, bool collectible = true) : base(name: pluginPath, isCollectible: collectible)
        {
            _resolver = new AssemblyDependencyResolver(pluginPath);
        }

        // Loads an assembly from the provided path
        protected override Assembly? Load(AssemblyName assemblyName)
        {
            Assembly? assemblyInDefaultContext = Default.Assemblies.FirstOrDefault(assembly => assembly.FullName == assemblyName.FullName);

            if (assemblyInDefaultContext is not null)
            {
                return assemblyInDefaultContext;
            }

            string? assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
            if (assemblyPath != null)
            {
                return LoadFromAssemblyPath(assemblyPath);
            }
            return null;
        }
        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            string? libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            if (libraryPath != null)
            {
                return LoadUnmanagedDllFromPath(libraryPath);
            }

            return IntPtr.Zero;
        }
    }
}
