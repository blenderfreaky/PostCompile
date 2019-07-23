namespace PostCompile.Tests.Helpers
{
    public class DummyWithMixedConstructors
    {
        public DummyWithMixedConstructors()
        {
        }

#pragma warning disable RCS1163 // Unused parameter.
#pragma warning disable IDE0060 // Remove unused parameter

        public DummyWithMixedConstructors(int i)
        {
        }

        public DummyWithMixedConstructors(string s)
        {
        }
    }
}