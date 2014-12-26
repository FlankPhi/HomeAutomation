using System.Collections;
using HomeAutomation.Etc.Delegates;

namespace HomeAutomation.Etc
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