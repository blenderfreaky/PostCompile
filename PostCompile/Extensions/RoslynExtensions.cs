using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace PostCompile.Extensions
{
    public static class RoslynExtensions
    {
        public static IEnumerable<ITypeSymbol> GetTypeSymbols(this INamespaceSymbol namespaceSymbol)
        {
            if (namespaceSymbol == null)
                throw new ArgumentNullException(nameof(namespaceSymbol));

            return
                namespaceSymbol
                    .GetMembers()
                    .OfType<ITypeSymbol>()
                    .Union(namespaceSymbol.GetNamespaceMembers().SelectMany(x => x.GetTypeSymbols()))
                    .Union(namespaceSymbol.GetMembers().OfType<ITypeSymbol>().SelectMany(x => x.GetTypeSymbols()));
        }

        public static IEnumerable<ITypeSymbol> GetTypeSymbols(this ITypeSymbol typeSymbol)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            return
                typeSymbol
                    .GetTypeMembers()
                    .Union(typeSymbol.GetTypeMembers().SelectMany(x => x.GetTypeSymbols()));
        }

        public static ITypeSymbol GetSymbol(this Solution solution, Type type)
        {
            if (solution == null)
                throw new ArgumentNullException(nameof(solution));
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            foreach (var project in solution.Projects)
            {
                var compilation = project.GetCompilationAsync().Result;
                var matchingTypeSymbol = GetTypeSymbols(compilation.GlobalNamespace).FirstOrDefault(typeSymbol => typeSymbol.Is(type));
                if (matchingTypeSymbol != null)
                    return matchingTypeSymbol;
            }

            return null;
        }

        public static IMethodSymbol GetSymbol(this Solution solution, MethodInfo methodInfo)
        {
            if (solution == null)
                throw new ArgumentNullException(nameof(solution));
            if (methodInfo == null)
                throw new ArgumentNullException(nameof(methodInfo));

            var typeSymbol = solution.GetSymbol(methodInfo.DeclaringType);
            if (typeSymbol != null)
            {
                var matchingSymbol =
                    typeSymbol.GetMembers()
                        .OfType<IMethodSymbol>()
                        .SingleOrDefault(methodSymbol => methodSymbol.Is(methodInfo));
                return matchingSymbol;
            }

            return null;
        }

        public static IPropertySymbol GetSymbol(this Solution solution, PropertyInfo propertyInfo)
        {
            if (solution == null)
                throw new ArgumentNullException(nameof(solution));
            if (propertyInfo == null)
                throw new ArgumentNullException(nameof(propertyInfo));

            var typeSymbol = solution.GetSymbol(propertyInfo.DeclaringType);
            if (typeSymbol != null)
            {
                var matchingSymbol =
                    typeSymbol.GetMembers()
                        .OfType<IPropertySymbol>()
                        .SingleOrDefault(propertySymbol => propertySymbol.Is(propertyInfo));
                return matchingSymbol;
            }

            return null;
        }

        public static IMethodSymbol GetSymbol(this Solution solution, ConstructorInfo constructorInfo)
        {
            if (solution == null)
                throw new ArgumentNullException(nameof(solution));
            if (constructorInfo == null)
                throw new ArgumentNullException(nameof(constructorInfo));

            var typeSymbol = solution.GetSymbol(constructorInfo.DeclaringType);
            if (typeSymbol != null)
            {
                var matchingSymbol =
                    typeSymbol.GetMembers()
                        .OfType<IMethodSymbol>()
                        .SingleOrDefault(methodSymbol => methodSymbol.Is(constructorInfo));
                return matchingSymbol;
            }

            return null;
        }

        public static IFieldSymbol GetSymbol(this Solution solution, FieldInfo fieldInfo)
        {
            if (solution == null)
                throw new ArgumentNullException(nameof(solution));
            if (fieldInfo == null)
                throw new ArgumentNullException(nameof(fieldInfo));

            var typeSymbol = solution.GetSymbol(fieldInfo.DeclaringType);
            if (typeSymbol != null)
            {
                var matchingSymbol =
                    typeSymbol.GetMembers()
                        .OfType<IFieldSymbol>()
                        .SingleOrDefault(fieldSymbol => fieldSymbol.Is(fieldInfo));
                return matchingSymbol;
            }

            return null;
        }

        public static bool Is(this ITypeSymbol typeSymbol, Type type)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return typeSymbol.ToDisplayString() == type.ToDisplayString();
        }

        public static bool Is(this IMethodSymbol methodSymbol, MethodInfo methodInfo)
        {
            if (methodSymbol == null)
                throw new ArgumentNullException(nameof(methodSymbol));
            if (methodInfo == null)
                throw new ArgumentNullException(nameof(methodInfo));

            if (methodSymbol.Name != methodInfo.Name)
                return false;

            ImmutableArray<IParameterSymbol> symbolParameters = methodSymbol.Parameters;
            ParameterInfo[] methodInfoParameters = methodInfo.GetParameters();

            if (symbolParameters.Length != methodInfoParameters.Length)
                return false;

            for (var i = 0; i < symbolParameters.Length; i++)
            {
                if (!symbolParameters[i].Type.Is(methodInfoParameters[i].ParameterType))
                    return false;
            }

            return true;
        }

        public static bool Is(this IMethodSymbol methodSymbol, ConstructorInfo constructorInfo)
        {
            if (methodSymbol == null)
                throw new ArgumentNullException(nameof(methodSymbol));
            if (constructorInfo == null)
                throw new ArgumentNullException(nameof(constructorInfo));

            if (methodSymbol.Name != ".ctor")
                return false;

            ImmutableArray<IParameterSymbol> symbolParameters = methodSymbol.Parameters;
            ParameterInfo[] constructorInfoParameters = constructorInfo.GetParameters();

            if (symbolParameters.Length != constructorInfoParameters.Length)
                return false;

            for (var i = 0; i < symbolParameters.Length; i++)
            {
                if (!symbolParameters[i].Type.Is(constructorInfoParameters[i].ParameterType))
                    return false;
            }

            return true;
        }

        public static bool Is(this IPropertySymbol propertySymbol, PropertyInfo propertyInfo)
        {
            if (propertySymbol == null)
                throw new ArgumentNullException(nameof(propertySymbol));
            if (propertyInfo == null)
                throw new ArgumentNullException(nameof(propertyInfo));

            return propertySymbol.ContainingType.Is(propertyInfo.DeclaringType)
                   && propertySymbol.Name == propertyInfo.Name
                   && propertySymbol.Type.Is(propertyInfo.PropertyType);
        }

        public static bool Is(this IFieldSymbol fieldSymbol, FieldInfo fieldInfo)
        {
            if (fieldSymbol == null)
                throw new ArgumentNullException(nameof(fieldSymbol));
            if (fieldInfo == null)
                throw new ArgumentNullException(nameof(fieldInfo));

            return fieldSymbol.ContainingType.Is(fieldInfo.DeclaringType)
                   && fieldSymbol.Name == fieldInfo.Name
                   && fieldSymbol.Type.Is(fieldInfo.FieldType);
        }
    }
}