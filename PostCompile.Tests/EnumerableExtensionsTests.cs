using PostCompile.Common;
using PostCompile.Extensions;
using PostCompile.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace PostCompile.Tests
{
    public class EnumerableExtensionsTests
    {
        [Fact]
        public void TopologicalSort_Tasks()
        {
            Type[] types = new[] { typeof(DummyTaskA), typeof(DummyTaskB), typeof(DummyTaskC), typeof(DummyTaskD), typeof(DummyTaskE), typeof(DummyTaskF) };
            Dictionary<Type, IPostCompileTask> tasks = types.ToDictionary(x => x, x => (IPostCompileTask)Activator.CreateInstance(x));

            IEnumerable<IPostCompileTask> sortedTasks = tasks.Values.TopologicalSort(x => x.DependsOn.Select(y => tasks[y]));
            string sortedTasksStr = sortedTasks.Aggregate(new StringBuilder(), (ag, n) => ag.Append(n.GetType().Name.Last()), ag => ag.ToString());

            Assert.Equal("ABCDEF", sortedTasksStr);
        }
    }
}