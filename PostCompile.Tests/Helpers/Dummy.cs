using System.Collections.Generic;

namespace PostCompile.Tests.Helpers
{
    public class Dummy
    {
        public static void Method()
        {
        }

#pragma warning disable RCS1163 // Unused parameter.
#pragma warning disable IDE0060 // Remove unused parameter

        public static void Method(int i)
        {
        }

        public static void Method(string s, int i)
        {
        }

        public static void Method(IEnumerable<string> s)
        {
        }

        public static void Method(bool[] b)
        {
        }

        public static void Method(params object[] o)
        {
        }

        public static void Method(Dummy d)
        {
        }

        public static void Method(NestedDummy nd)
        {
        }

        public int PropertyInt { get; set; }

        public int PropertyString { get; set; }

        public int _fieldInt;

        public string _fieldString;

#pragma warning disable CA1034 // Nested types should not be visible

        public class NestedDummy
        {
            public int PropertyInt { get; set; }

            public int PropertyString { get; set; }

            public int _fieldInt;

            public string _fieldString;

            public class DeeplyNested
            {
            }
        }
    }
}