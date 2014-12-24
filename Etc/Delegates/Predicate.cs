using System;
using System.Collections;

namespace HomeAutomation
{
    public delegate bool Predicate(object obj);

    public static class Extensions
    {
        public static IEnumerable Where(this IEnumerable e, Predicate p)
        {
            return new ConditionalEnumerable(e, p);
        }
        public static object FirstOrDefault(this IEnumerable e)
        {
            return FirstOrDefault(e, o => true);
        }
        public static object FirstOrDefault(this IEnumerable e, Predicate p)
        {
            foreach (var o in e)
                if (p(o))
                    return o;

            return null;
        }


    }

    sealed class ConditionalEnumerable : IEnumerable
    {
        IEnumerable e;
        Predicate p;

        internal ConditionalEnumerable(IEnumerable e, Predicate p)
        {
            this.e = e;
            this.p = p;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new ConditionalEnumerator(e.GetEnumerator(), p);
        }
    }
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