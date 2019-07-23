using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using PostCompile.Common;
using PostCompile.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PostCompile
{

    [Serializable]
    public class TaksRunnerDependencyException : Exception
    {
        public ICollection<MissingDependency> MissingDependencies { get; set; }
        public ICollection<InvalidDependency> InvalidDependencies { get; set; }

        public TaksRunnerDependencyException(string message,
                                             ICollection<MissingDependency> missingDependencies,
                                             ICollection<InvalidDependency> invalidDependencies) : base(message)
        {
            MissingDependencies = missingDependencies;
            InvalidDependencies = invalidDependencies;
        }

        public TaksRunnerDependencyException(string message, Exception innerException,
                                             ICollection<MissingDependency> missingDependencies,
                                             ICollection<InvalidDependency> invalidDependencies) : base(message, innerException)
        {
            MissingDependencies = missingDependencies;
            InvalidDependencies = invalidDependencies;
        }
    }

    public class MissingDependency
    {
        public MissingDependency(Type dependency, Type dependent)
        {
            Dependency = dependency;
            Dependent = dependent;
        }

        public Type Dependency { get; }

        public Type Dependent { get; }

        public override string ToString() => $"Missing dependency '{Dependency}' for task '{Dependent}'.";
    }

    public class InvalidDependency
    {
        public InvalidDependency(Type dependency, Type dependent)
        {
            Dependency = dependency;
            Dependent = dependent;
        }

        public Type Dependency { get; }

        public Type Dependent { get; }

        public override string ToString() => $"Invalid dependency '{Dependency}' for task '{Dependent}'.";
    }
}