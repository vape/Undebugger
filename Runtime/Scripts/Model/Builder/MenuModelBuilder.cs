#if UNITY_EDITOR
#define UNDEBUGGER_MODEL_BUILDER_VALIDATION
#endif

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Undebugger.Model.Commands.Builtin;
using UnityEngine;

namespace Undebugger.Model.Builder
{
    internal class MenuModelBuilder
    {
        private static readonly string[] ignoredAssemblies = new string[]
        {
            "System",
            "Unity",
            "netstandart",
            "Microsoft",
            "mscorelib",
            "Mono",
        };

        private object preloadingLock = new object();
        private bool preloading;
        private List<TypeData> staticTypesCache;
        private Dictionary<Type, TypeData> typeCache = new Dictionary<Type, TypeData>(capacity: 128);

        public async void PreloadAsync()
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            preloading = true;

            await Task.Run(() =>
            {
                staticTypesCache = FindStaticTargets();

                lock (preloadingLock)
                {
                    preloading = false;
                }
            });
        }

        public MenuModel Build()
        {
            var model = new MenuModel();

            model.Status.Segments.Add(Status.Builtin.ApplicationInfoStatusSegment.Instance);
            model.Status.Segments.Add(Status.Builtin.SystemInfoStatusSegment.Instance);
            model.Status.Segments.Add(Status.Builtin.DeviceInfoStatusSegment.Instance);

            AddBehaviourTypesOptions(model);

            lock (preloadingLock)
            {
                if (!preloading)
                {
                    if (staticTypesCache == null)
                    {
                        staticTypesCache = FindStaticTargets();
                    }

                    AddStaticTypesOptions(model);
                }
            }
            
            return model;
        }

        private void AddStaticTypesOptions(MenuModel model)
        {
            foreach (var type in staticTypesCache)
            {
                AddTypeOptions(model, type, null);
            }
        }

        private void AddBehaviourTypesOptions(MenuModel model)
        {
            foreach (var behaviour in GameObject.FindObjectsOfType<MonoBehaviour>())
            {
                var handler = behaviour as IDebugMenuHandler;
                if (handler != null)
                {
                    handler.OnBuildingModel(model);
                }

                var type = behaviour.GetType();
                if (!typeCache.TryGetValue(type, out var data))
                {
                    data = TypeDataBuilder.CreateForType(type, isStatic: false);
                    typeCache.Add(type, data);
                }

                if (!data.IsDebugTarget)
                {
                    continue;
                }

                AddTypeOptions(model, data, behaviour);
            }
        }

        private static void AddTypeOptions(MenuModel model, TypeData type, object instance)
        {
            var page =
                type.PageOverride.Valid ?
                model.Commands.FindOrCreatePage(type.PageOverride.Name, type.PageOverride.Priority) :
                model.Commands.GetGlobalPage();

            var segment = page.FindOrCreateSegment(type.Name, priority: 0);

            if (type.ActionMethods?.Count > 0)
            {
                foreach (var action in type.ActionMethods)
                {
                    segment.Commands.Add(CreateAction(action, instance));
                }
            }

            if (type.ToggleProperties?.Count > 0)
            {
                foreach (var toggle in type.ToggleProperties)
                {
                    segment.Commands.Add(CreateToggle(toggle, instance));
                }
            }

            if (type.HandlerMethods?.Count > 0)
            {
                var param = new object[1] { model };

                foreach (var handler in type.HandlerMethods)
                {
                    handler.Invoke(instance, param);
                }
            }
        }

        private static List<TypeData> FindStaticTargets()
        {
            var data = new List<TypeData>(capacity: 16);

            const TypeAttributes staticClassAttributes = TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit;

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var ignore = false;

            for (int i = 0; i < assemblies.Length; ++i)
            {
                ignore = false;

                for (int k = 0; k < ignoredAssemblies.Length; ++k)
                {
                    if (assemblies[i].FullName.StartsWith(ignoredAssemblies[k]))
                    {
                        ignore = true;
                        break;
                    }
                }

                if (ignore)
                {
                    continue;
                }

                var types = assemblies[i].GetTypes();

                for (int j = 0; j < types.Length; ++j)
                {
                    var type = types[j];

                    if ((type.Attributes & staticClassAttributes) == 0)
                    {
                        continue;
                    }

                    var typeInfo = TypeDataBuilder.CreateForType(type, isStatic: true);
                    if (typeInfo.IsDebugTarget)
                    {
                        data.Add(typeInfo);
                    }
                }
            }

            return data;
        }

        private static ToggleCommandModel CreateToggle(PropertyInfo property, object instance)
        {
            var valueRef = new ValueRef<bool>(() => (bool)property.GetValue(instance), (value) => property.SetValue(instance, value));
            return new ToggleCommandModel(property.Name, valueRef);
        }

        private static ActionCommandModel CreateAction(MethodInfo method, object instance)
        {
            return new ActionCommandModel(method.Name, () => { method.Invoke(instance, null); });
        }
    }
}
