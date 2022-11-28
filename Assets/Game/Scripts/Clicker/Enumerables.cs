using System;
using System.Collections.Generic;

namespace Clicker
{
    public static class Enumerables
    {
        public static void ForEach<T>(this IEnumerable<T> @this, Action<T> action)
        {
            foreach (var item in @this)
            {
                action(item);
            }
        }
    }
}