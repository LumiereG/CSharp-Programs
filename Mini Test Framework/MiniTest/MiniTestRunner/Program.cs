using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using MiniTest;


namespace MiniTestRunner
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Check if the user provided assembly paths
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: MiniTestRunner <assembly paths>");
                return;
            }

            // Iterate through all provided assembly paths and load tests
            foreach (var path in args)
            {
                var context = new TestLoadContext(path);

                try
                {
                    var assembly = context.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(path)));
                    TestRunner.RunTestsInAssembly(assembly);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Failed to load assembly '{path}': {ex.Message} : WARNING");
                    Console.ResetColor();
                }
                finally
                {
                    context.Unload();
                }
            }
        }
    }
}
