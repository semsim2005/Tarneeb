using System;
using System.Collections.Generic;

namespace Tarneeb.Engine
{
    public static class Extensions
    {
        public static void Swap<T>(this IList<T> list, int firstIndex, int secondIndex)
        {
            var temp = list[firstIndex];
            list[firstIndex] = list[secondIndex];
            list[secondIndex] = temp;
        }

        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            if (action == null)
                return;
            var enumerator = enumerable.GetEnumerator();
            while (enumerator.MoveNext())
            {
                action(enumerator.Current);
            }
        }

        public static void SafelyInvoke<T>(this Delegate dDelegate, object sender, T eventArgs)
            where T : EventArgs
        {
            var eventHandler = dDelegate as EventHandler<T>;
            if (eventHandler != null)
            {
                eventHandler(sender, eventArgs);
            }
        }
    }
}