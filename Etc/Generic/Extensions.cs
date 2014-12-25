using System.Collections;
using HomeAutomation.Etc.Delegates;

namespace HomeAutomation.Etc.Generic
{
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
}