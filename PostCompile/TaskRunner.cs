﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using PostCompile.Common;
using PostCompile.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PostCompile
{
    public class TaskRunner : MarshalByRefObject
    {
        public TaskRunnerResult Execute(string assemblyPath, string solutionPath)
        {
            var assembly = Assembly.LoadFrom(assemblyPath);

            var tasks =
                (from type in assembly.GetTypes()
                 where typeof(IPostCompileTask).IsAssignableFrom(type)
                 where !type.IsAbstract
                 where type.HasDefaultConstructor()
                 select type).ToDictionary(x => x, x => (IPostCompileTask)Activator.CreateInstance(x));
            CheckTaskDependencies(tasks);

            var sortedTaskInstances = tasks.Values.TopologicalSort(x => x.DependsOn.Select(y => tasks[y])).ToList();

            Solution solution;
            using (var workspace = MSBuildWorkspace.Create())
            {
                solution = workspace.OpenSolutionAsync(solutionPath).Result;
            }
            var log = new ConcreteLog(solution, Console.Out);

            foreach (var taskInstance in sortedTaskInstances)
            {
                taskInstance.Log = log;
                taskInstance.RunAsync().Wait();
            }

            // Return task types that need to be removed
            return new TaskRunnerResult
            {
                TaskTypes = tasks.Select(x => x.Key.FullName).ToList()
            };
        }

        private static void CheckTaskDependencies(Dictionary<Type, IPostCompileTask> tasks)
        {
            List<InvalidDependency> invalidDependencies = (from task in tasks
                                                           from dependency in task.Value.DependsOn
                                                           where !typeof(IPostCompileTask).IsAssignableFrom(dependency)
                                                           select new InvalidDependency(dependency, task.Key))
                                                           .ToList();

            List<MissingDependency> missingDependencies = (from task in tasks
                                                           from dependency in task.Value.DependsOn
                                                           where !tasks.ContainsKey(task.Key)
                                                           where typeof(IPostCompileTask).IsAssignableFrom(dependency)
                                                           select new MissingDependency(dependency, task.Key))
                                                           .ToList();

            if (invalidDependencies.Count > 0 || missingDependencies.Count > 0)
            {
                invalidDependencies.ForEach(x => Error(x.ToString()));
                missingDependencies.ForEach(x => Error(x.ToString()));

                throw new TaksRunnerDependencyException("Aborted due to invalid or missing dependencies.",
                    missingDependencies,
                    invalidDependencies);
            }
        }

        private static void Error(string format, params object[] args)
        {
            Console.WriteLine("Error: " + format, args);
        }
    }
}