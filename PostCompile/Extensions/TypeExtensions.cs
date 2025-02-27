﻿using System;
using System.Linq;
using System.Reflection;

namespace PostCompile.Extensions
{
    public static class TypeExtensions
    {
        public static bool HasDefaultConstructor(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return type.GetConstructor(Type.EmptyTypes) != null;
        }

        public static string ToDisplayString(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (type.FullName == null)
                return string.Empty;

            if (type.IsArray)
            {
                return $"{type.GetElementType().ToDisplayString()}[]";
            }

            if (type.IsGenericType)
            {
                return
                    $"{type.FullName.Split('`')[0].Replace("+", ".")}<{string.Join(",", type.GetGenericArguments().Select(ToDisplayString))}>";
            }

            switch (type.FullName)
            {
                case "System.Byte": return "byte";
                case "System.Int16": return "short";
                case "System.Int32": return "int";
                case "System.Int64": return "long";
                case "System.SByte": return "sbyte";
                case "System.UInt16": return "ushort";
                case "System.UInt32": return "uint";
                case "System.UInt64": return "ulong";
                case "System.Boolean": return "bool";
                case "System.Single": return "float";
                case "System.Double": return "double";
                case "System.Decimal": return "decimal";
                case "System.String": return "string";
                case "System.Object": return "object";
                default: return type.FullName.Replace("+", ".");
            }
        }

        public static string ToDisplayString(this MethodInfo methodInfo)
        {
            if (methodInfo == null)
                throw new ArgumentNullException(nameof(methodInfo));

            return
                $"{methodInfo.DeclaringType.ToDisplayString()}.{methodInfo.Name}({string.Join(",", methodInfo.GetParameters().Select(x => x.ParameterType.ToDisplayString()))})";
        }

        public static string ToDisplayString(this PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException(nameof(propertyInfo));

            return $"{propertyInfo.DeclaringType.ToDisplayString()}.{propertyInfo.Name}";
        }

        public static string ToDisplayString(this ConstructorInfo constructorInfo)
        {
            if (constructorInfo == null)
                throw new ArgumentNullException(nameof(constructorInfo));

            return
                $"{constructorInfo.DeclaringType.ToDisplayString()}({string.Join(",", constructorInfo.GetParameters().Select(x => x.ParameterType.ToDisplayString()))})";
        }

        public static string ToDisplayString(this FieldInfo fieldInfo)
        {
            if (fieldInfo == null)
                throw new ArgumentNullException(nameof(fieldInfo));

            return $"{fieldInfo.DeclaringType.ToDisplayString()}.{fieldInfo.Name}";
        }

        public static string ToDisplayString(this TypeInfo typeInfo)
        {
            if (typeInfo == null)
                throw new ArgumentNullException(nameof(typeInfo));

            return typeInfo.AsType().ToDisplayString();
        }
    }
}