using System;
using System.Collections.Generic;
using System.Linq;

namespace PostCompile
{
    [Serializable]
#pragma warning disable CA2229 // Implement serialization constructors
#pragma warning disable RCS1194 // Implement exception constructors.
    public class TaksRunnerDependencyException : Exception
#pragma warning restore RCS1194 // Implement exception constructors.
#pragma warning restore CA2229 // Implement serialization constructors
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

        public TaksRunnerDependencyException() : base("Aborted due to invalid or missing dependencies.")
        {
            MissingDependencies = Enumerable.Empty<MissingDependency>().ToList();
            InvalidDependencies = Enumerable.Empty<InvalidDependency>().ToList();
        }

        public TaksRunnerDependencyException(string message) : base(message)
        {
            MissingDependencies = Enumerable.Empty<MissingDependency>().ToList();
            InvalidDependencies = Enumerable.Empty<InvalidDependency>().ToList();
        }

        public TaksRunnerDependencyException(string message, Exception innerException) : base(message, innerException)
        {
            MissingDependencies = Enumerable.Empty<MissingDependency>().ToList();
            InvalidDependencies = Enumerable.Empty<InvalidDependency>().ToList();
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