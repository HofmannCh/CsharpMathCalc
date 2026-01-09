using System;
using System.Collections.Generic;
using System.Text;

namespace MathCalc.Logic.Internal
{
    internal static class Extensions
    {
        public static bool TryGetValue<T>(this IEnumerable<T> src, Func<T, bool> match, out T result)
        {
            var enumerator = src.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (match(enumerator.Current))
                {
                    result = enumerator.Current;
                    return true;
                }
            }
            result = default;
            return false;
        }
        public static bool SameValue<T1, T2>(this IEnumerable<T1> src1, IEnumerable<T2> src2, Func<T1, T2, bool> match)
        {
            var enumerator1 = src1.GetEnumerator();
            var enumerator2 = src2.GetEnumerator();

            while (true)
            {
                var next1 = enumerator1.MoveNext();
                var next2 = enumerator2.MoveNext();

                if (next1 != next2)
                    return false;
                else if (!next1 && !next2)
                    return true;

                if (!match(enumerator1.Current, enumerator2.Current))
                    return false;
            }
        }
    }
}
