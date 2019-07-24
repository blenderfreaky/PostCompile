using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FindSymbols;
using PostCompile.Common;
using PostCompile.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TypeInfo = System.Reflection.TypeInfo;

namespace PostCompile
{
    public class ConcreteLog : ILog
    {
        private readonly Solution _solution;
        private readonly TextWriter _writer;

        public ConcreteLog(Solution solution, TextWriter writer)
        {
            _solution = solution ?? throw new ArgumentNullException(nameof(solution));
            _writer = writer ?? throw new ArgumentNullException(nameof(writer));
        }

        #region Error Interface

        public void Error(string message)
        {
            _writer.WriteLine($"PostCompile: error: {message}");
        }

        public void Error(string file, string message)
        {
            _writer.WriteLine($"{file}: error: {message}");
        }

        public void Error(string file, int line, string message)
        {
            _writer.WriteLine($"{file}({line}): error: {message}");
        }

        public void Error(string file, int line, int column, string message)
        {
            _writer.WriteLine($"{file}({line},{column}): error: {message}");
        }

        public void Error(MethodInfo methodInfo, string message)
        {
            var symbol = _solution.GetSymbol(methodInfo);
            if (symbol == null)
            {
                Error($"{methodInfo.ToDisplayString()}: Method {message}");
                Warning($"Failed to locate symbol for method '{methodInfo}'.");
            }
            else
            {
                Error(symbol, message);
            }
        }

        public void Error(PropertyInfo propertyInfo, string message)
        {
            var symbol = _solution.GetSymbol(propertyInfo);
            if (symbol == null)
            {
                Error($"{propertyInfo.ToDisplayString()}: Property {message}");
                Warning($"Failed to locate symbol for property '{propertyInfo}'.");
            }
            else
            {
                Error(symbol, message);
            }
        }

        public void Error(ConstructorInfo constructorInfo, string message)
        {
            var symbol = _solution.GetSymbol(constructorInfo);
            if (symbol == null)
            {
                Error($"{constructorInfo.ToDisplayString()}: Constructor {message}");
                Warning($"Failed to locate symbol for constructor '{constructorInfo}'.");
            }
            else
            {
                Error(symbol, message);
            }
        }

        public void Error(FieldInfo fieldInfo, string message)
        {
            var symbol = _solution.GetSymbol(fieldInfo);
            if (symbol == null)
            {
                Error($"{fieldInfo.ToDisplayString()}: Field {message}");
                Warning($"Failed to locate symbol for field '{fieldInfo}'.");
            }
            else
            {
                Error(symbol, message);
            }
        }

        public void Error(TypeInfo typeInfo, string message)
        {
            Error(typeInfo.AsType(), message);
        }

        public void Error(Type type, string message)
        {
            var symbol = _solution.GetSymbol(type);
            if (symbol == null)
            {
                Error($"{type.ToDisplayString()}: Type {message}");
                Warning($"Failed to locate symbol for type '{type}'.");
            }
            else
            {
                Error(symbol, message);
            }
        }

        #endregion Error Interface

        #region Error Private

        private void Error(ISymbol symbol, string message)
        {
            var location = symbol.Locations.FirstOrDefault();
            if (location == null)
                throw new Exception("Unexpected: Symbol contains no location.");

            Error(location.GetLineSpan(), message);
        }

        private void Error(SymbolCallerInfo callerInfo, string message)
        {
            foreach (var location in callerInfo.Locations.Reverse())
            {
                Error(location.GetLineSpan(), message);
            }
        }

        private void Error(FileLinePositionSpan lineSpan, string message)
        {
            Error(lineSpan.Path, lineSpan.StartLinePosition.Line + 1, lineSpan.StartLinePosition.Character + 1, message);
        }

        #endregion Error Private

        #region Warning Interface

        public void Warning(string message)
        {
            _writer.WriteLine("PostCompile: warning: {0}", message);
        }

        public void Warning(string file, string message)
        {
            _writer.WriteLine("{0}: warning: {1}", file, message);
        }

        public void Warning(string file, int line, string message)
        {
            _writer.WriteLine("{0}({1}): warning: {2}", file, line, message);
        }

        public void Warning(string file, int line, int column, string message)
        {
            _writer.WriteLine("{0}({1},{2}): warning: {3}", file, line, column, message);
        }

        public void Warning(MethodInfo methodInfo, string message)
        {
            var symbol = _solution.GetSymbol(methodInfo);
            if (symbol == null)
            {
                Warning($"{methodInfo.ToDisplayString()}: Method {message}");
                Warning($"Failed to locate symbol for method '{methodInfo}'.");
            }
            else
            {
                Warning(symbol, message);
            }
        }

        public void Warning(PropertyInfo propertyInfo, string message)
        {
            var symbol = _solution.GetSymbol(propertyInfo);
            if (symbol == null)
            {
                Warning($"{propertyInfo.ToDisplayString()}: Property {message}");
                Warning($"Failed to locate symbol for property '{propertyInfo}'.");
            }
            else
            {
                Warning(symbol, message);
            }
        }

        public void Warning(ConstructorInfo constructorInfo, string message)
        {
            var symbol = _solution.GetSymbol(constructorInfo);
            if (symbol == null)
            {
                Warning($"{constructorInfo.ToDisplayString()}: Constructor {message}");
                Warning($"Failed to locate symbol for constructor '{constructorInfo}'.");
            }
            else
            {
                Warning(symbol, message);
            }
        }

        public void Warning(FieldInfo fieldInfo, string message)
        {
            var symbol = _solution.GetSymbol(fieldInfo);
            if (symbol == null)
            {
                Warning($"{fieldInfo.ToDisplayString()}: Field {message}");
                Warning($"Failed to locate symbol for field '{fieldInfo}'.");
            }
            else
            {
                Warning(symbol, message);
            }
        }

        public void Warning(TypeInfo typeInfo, string message)
        {
            Warning(typeInfo.AsType(), message);
        }

        public void Warning(Type type, string message)
        {
            var symbol = _solution.GetSymbol(type);
            if (symbol == null)
            {
                Warning($"{type.ToDisplayString()}: Type {message}");
                Warning($"Failed to locate symbol for type '{type}'.");
            }
            else
            {
                Warning(symbol, message);
            }
        }

        #endregion Warning Interface

        #region Warning Private

        private void Warning(ISymbol symbol, string message)
        {
            var location = symbol.Locations.FirstOrDefault();
            if (location == null)
                throw new Exception("Unexpected: Symbol contains no location.");

            Warning(location.GetLineSpan(), message);
        }

        private void Warning(SymbolCallerInfo callerInfo, string message)
        {
            foreach (var location in callerInfo.Locations.Reverse())
            {
                Warning(location.GetLineSpan(), message);
            }
        }

        private void Warning(FileLinePositionSpan lineSpan, string message)
        {
            Warning(lineSpan.Path, lineSpan.StartLinePosition.Line + 1, lineSpan.StartLinePosition.Character + 1, message);
        }

        #endregion Warning Private

        #region Usage Error Interface

        public void UsageError(MethodInfo methodInfo, string message)
        {
            var symbol = _solution.GetSymbol(methodInfo);
            if (symbol == null)
            {
                Warning($"Failed to locate symbol for method '{methodInfo}'.");
            }
            else
            {
                IEnumerable<SymbolCallerInfo> callers = SymbolFinder.FindCallersAsync(symbol, _solution).Result;
                foreach (var caller in callers)
                {
                    Error(caller, message);
                }
            }
        }

        public void UsageError(PropertyInfo propertyInfo, string message)
        {
            var symbol = _solution.GetSymbol(propertyInfo);
            if (symbol == null)
            {
                Warning($"Failed to locate symbol for property '{propertyInfo}'.");
            }
            else
            {
                IEnumerable<SymbolCallerInfo> callers = SymbolFinder.FindCallersAsync(symbol, _solution).Result;
                foreach (var caller in callers)
                {
                    Error(caller, message);
                }
            }
        }

        public void UsageError(ConstructorInfo constructorInfo, string message)
        {
            var symbol = _solution.GetSymbol(constructorInfo);
            if (symbol == null)
            {
                Warning($"Failed to locate symbol for constructor '{constructorInfo}'.");
            }
            else
            {
                IEnumerable<SymbolCallerInfo> callers = SymbolFinder.FindCallersAsync(symbol, _solution).Result;
                foreach (var caller in callers)
                {
                    Error(caller, message);
                }
            }
        }

        #endregion Usage Error Interface

        #region Usage Warning Interface

        public void UsageWarning(MethodInfo methodInfo, string message)
        {
            var symbol = _solution.GetSymbol(methodInfo);
            if (symbol == null)
            {
                Warning($"Failed to locate symbol for method '{methodInfo}'.");
            }
            else
            {
                IEnumerable<SymbolCallerInfo> callers = SymbolFinder.FindCallersAsync(symbol, _solution).Result;
                foreach (var caller in callers)
                {
                    Warning(caller, message);
                }
            }
        }

        public void UsageWarning(PropertyInfo propertyInfo, string message)
        {
            var symbol = _solution.GetSymbol(propertyInfo);
            if (symbol == null)
            {
                Warning($"Failed to locate symbol for property '{propertyInfo}'.");
            }
            else
            {
                IEnumerable<SymbolCallerInfo> callers = SymbolFinder.FindCallersAsync(symbol, _solution).Result;
                foreach (var caller in callers)
                {
                    Warning(caller, message);
                }
            }
        }

        public void UsageWarning(ConstructorInfo constructorInfo, string message)
        {
            var symbol = _solution.GetSymbol(constructorInfo);
            if (symbol == null)
            {
                Warning($"Failed to locate symbol for constructor '{constructorInfo}'.");
            }
            else
            {
                IEnumerable<SymbolCallerInfo> callers = SymbolFinder.FindCallersAsync(symbol, _solution).Result;
                foreach (var caller in callers)
                {
                    Warning(caller, message);
                }
            }
        }

        #endregion Usage Warning Interface
    }
}