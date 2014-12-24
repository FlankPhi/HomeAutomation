using System.Collections;

namespace HomeAutomation
{
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
}