using System;
using System.Collections;
using HomeAutomation.Etc.Delegates;

namespace HomeAutomation.Etc
{
    sealed class ConditionalEnumerator : IEnumerator, IDisposable
    {
        IEnumerator e;
        Predicate p;

        internal ConditionalEnumerator(IEnumerator e, Predicate p)
        {
            this.e = e;
            this.p = p;
        }

        object IEnumerator.Current
        {
            get { return e.Current; }
        }

        void IEnumerator.Reset()
        {
            e.Reset();
        }

        bool IEnumerator.MoveNext()
        {
            var b = e.MoveNext();
            while (b && !p(e.Current))
            {
                b = e.MoveNext();
            }
            return b;
        }

        public void Dispose()
        {
            var d = e as IDisposable;
            if (null != d)
                d.Dispose();
        }
    }
}