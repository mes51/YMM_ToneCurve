using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YMM_ToneCurve.Extensions
{
    static class ObservableCollectionExtension
    {
        public static void Sort<T>(this ObservableCollection<T> collection, Comparison<T> comparison)
        {
            ArrayList.Adapter(collection).Sort(new ComparisonComparer<T>(comparison));
        }

        public static int FindLastIndex<T>(this ObservableCollection<T> collection, Predicate<T> predicate)
        {
            for (var i = collection.Count - 1; i > -1; i--)
            {
                if (predicate(collection[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        private class ComparisonComparer<T>(Comparison<T> Comparison) : IComparer
        {
            public int Compare(object? x, object? y)
            {
                if (x is T a && y is T b)
                {
                    return Comparison(a, b);
                }

                return 0;
            }
        }
    }
}
