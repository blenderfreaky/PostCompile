using System;

namespace PostCompile
{
    public class Isolated<T> : IDisposable
        where T : MarshalByRefObject
    {
        private AppDomain _domain;

        public Isolated()
        {
            _domain = AppDomain.CreateDomain("Isolated:" + Guid.NewGuid(), null, AppDomain.CurrentDomain.SetupInformation);

            var type = typeof(T);

            Value = (T)_domain.CreateInstanceAndUnwrap(type.Assembly.FullName, type.FullName);
        }

        public T Value { get; }

        public void Dispose()
        {
            if (_domain != null)
            {
                AppDomain.Unload(_domain);
                _domain = null;
            }
        }
    }
}