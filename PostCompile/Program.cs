using Mono.Cecil;
using System;
using System.IO;
using System.Linq;

namespace PostCompile
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                if (args.Length != 2)
                    throw new ArgumentException("Invalid of amount arguments passed.", nameof(args));

                string assemblyPath = args[0];
                string projectPath = args[1];

                if (!File.Exists(assemblyPath))
                    throw new FileNotFoundException("Failed to locate assembly.");

                if (!File.Exists(projectPath))
                    throw new FileNotFoundException("Failed to locate project.");

                TaskRunnerResult result = TaskRunner.Execute(assemblyPath, projectPath);

                var module = ModuleDefinition.ReadModule(assemblyPath);
                foreach (var t in module.Types.Where(x => result.TaskTypes.Contains(x.FullName)))
                {
                    module.Types.Remove(t);
                }

                module.Write(assemblyPath);
                File.Delete(Path.Combine(Path.GetDirectoryName(assemblyPath), "PostCompile.Common.dll"));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Critical error: " + ex.Message);
                throw;
            }
        }
    }
}