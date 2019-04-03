using System;
using System.Collections.Generic;
using System.Text;

namespace FluentCreativity.Core
{
    public static class CollectionExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> @this, Action<T> action)
        {
            if (@this == null) throw new ArgumentNullException("source");
            if (action == null) throw new ArgumentNullException("action");

            foreach (T item in @this)
            {
                action(item);
                //yield return item;
            }
        }
    }
}
