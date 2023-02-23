using System.Collections.Generic;
using UnityEngine;

namespace Undebugger.UI.Layout
{
    internal static class UListPool<T>
    {
        private static List<List<T>> pool = new List<List<T>>(capacity: 32);

        public static List<T> GetExact(int capacity)
        {
            for (int i = 0; i < pool.Count; i++)
            {
                if (pool[i].Capacity == capacity)
                {
                    return Remove(i);
                }
            }

            return new List<T>(capacity: capacity);
        }

        public static List<T> Get(int capacity)
        {
            int maxCapacity = 0;
            int maxIndex = -1;

            for (int i = 0; i < pool.Count; i++)
            {
                if (pool[i].Capacity >= capacity)
                {
                    return Remove(i);
                }

                if (pool[i].Capacity > maxCapacity)
                {
                    maxCapacity = pool[i].Capacity;
                    maxIndex = i;
                }
            }

            if (maxIndex != -1 && capacity / maxCapacity <= 2)
            {
                return Remove(maxIndex);
            }
            
            return new List<T>(capacity: Mathf.NextPowerOfTwo(capacity));
        }

        public static void Return(List<T> list)
        {
            list.Clear();
            pool.Add(list);
        }

        private static List<T> Remove(int index)
        {
            var list = pool[index];
            var last = pool.Count - 1;
            pool[index] = pool[last];
            pool.RemoveAt(last);

            return list;
        }
    }
}
