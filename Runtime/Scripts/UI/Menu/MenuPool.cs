using System;
using System.Collections.Generic;
using UnityEngine;

namespace Undebugger.UI.Menu
{
    internal interface IPoolHandler
    {
        void UsePool(MenuPool pool);
    }

    internal interface IPoolable
    {
        void AddingToPool();
    }

    internal static class MenuPoolExtensions
    {
        public static T GetOrInstantiate<T>(this MenuPool pool, T template, Transform container)
            where T: Component
        {
            T result;

            if (pool == null || !pool.TryGet(template.GetType(), out result, container))
            {
                result = GameObject.Instantiate(template, container);

                var handler = result as IPoolHandler;
                if (handler != null)
                {
                    handler.UsePool(pool);
                }
            }

            return result;
        }

        public static void AddOrDestroy(this MenuPool pool, Component obj)
        {
            if (obj == null)
            {
                return;
            }

            if (pool == null)
            {
                GameObject.DestroyImmediate(obj);
            }
            else
            {
                pool.Add(obj);
            }
        }
    }

#if !UNDEBUGGER_INTERNAL
    [AddComponentMenu("")]
#endif
    internal class MenuPool : MonoBehaviour
    {
        public bool IsDestroyed
        { get; private set; }

        private Dictionary<Type, List<object>> pool = new Dictionary<Type, List<object>>(capacity: 16);

        private void OnDestroy()
        {
            IsDestroyed = true;
        }

        public void Reserve(Type type, int capacity)
        {
            if (!pool.TryGetValue(type, out var list))
            {
                list = new List<object>(capacity: capacity);
                pool.Add(type, list);
            }
            else if (capacity / list.Capacity >= 2)
            {
                var newList = new List<object>(capacity);
                newList.AddRange(list);
                pool[type] = newList;
            }
        }

        public void Add(Component obj)
        {
            if (IsDestroyed)
            {
                return;
            }

            var type = obj.GetType();

            if (!pool.TryGetValue(type, out var list))
            {
                list = new List<object>(8);
                pool[type] = list;
            }

            var poolable = obj as IPoolable;
            if (poolable != null)
            {
                poolable.AddingToPool();
            }

            obj.gameObject.SetActive(false);
            obj.transform.SetParent(transform);
            list.Add(obj);
        }

        public bool TryGet<T>(out T obj, Transform container)
            where T : Component
        {
            if (pool == null || !pool.TryGetValue(typeof(T), out var list) || list.Count == 0)
            {
                obj = default;
                return false;
            }

            obj = list[list.Count - 1] as T;
            list.RemoveAt(list.Count - 1);

            obj.transform.SetParent(container);
            obj.gameObject.SetActive(true);

            return true;
        }

        public bool TryGet<T>(Type type, out T obj, Transform container)
            where T: Component
        {
            if (pool == null || !pool.TryGetValue(type, out var list) || list.Count == 0)
            {
                obj = default;
                return false;
            }

            obj = list[list.Count - 1] as T;
            list.RemoveAt(list.Count - 1);

            obj.transform.SetParent(container);
            obj.gameObject.SetActive(true);

            return true;
        }
    }
}
