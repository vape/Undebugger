#if UNITY_EDITOR
#define UNDEBUGGER_MODEL_BUILDER_VALIDATION
#endif

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Undebugger.Model.Commands;
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
                    data = TypeDataBuilder.CreateForType(type);
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
            PageModel page = null;
            SegmentModel segment = null;

            void EnsureSegment()
            {
                if (page == null)
                {
                    page = model.Commands.GetGlobalPage();
                }

                if (segment == null)
                {
                    var name = type.NameAttribute?.Name ?? type.Type.Name;
                    var priority = type.PriorityAttribute?.Priority ?? 0;

                    segment = page.FindOrCreateSegment(name, priority);
                }
            }

            if (type.Methods?.Count > 0)
            {
                var handlerParam = new object[1] { model };

                foreach (var method in type.Methods)
                {
                    switch (method.Type)
                    {
                        case MethodType.Action:
                            EnsureSegment();
                            segment.Commands.Add(CreateAction(method, instance));
                            break;

                        case MethodType.TextAction:
                            EnsureSegment();
                            segment.Commands.Add(CreateTextActionCommand(method, instance, method.DefaultValueAttribute?.Value?.ToString()));
                            break;

                        case MethodType.IntAction:
                            EnsureSegment();
                            segment.Commands.Add(CreateIntActionCommand(method, instance, method.DefaultValueAttribute?.Value == null ? 0 : (int)method.DefaultValueAttribute.Value));
                            break;

                        case MethodType.Handler:
                            method.Info?.Invoke(instance, handlerParam);
                            break;
                    }
                }
            }

            if (type.Properties?.Count > 0)
            {
                foreach (var property in type.Properties)
                {
                    switch (property.Type)
                    {
                        case PropertyType.Toggle:
                            EnsureSegment();
                            segment.Commands.Add(CreateToggle(property, type.Type, instance));
                            break;

                        case PropertyType.Dropdown:
                            EnsureSegment();
                            segment.Commands.Add(CreateDropdown(property, instance));
                            break;

                        case PropertyType.Carousel:
                            EnsureSegment();
                            segment.Commands.Add(CreateCarousel(property, instance));
                            break;
                    }
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

                    if ((type.Attributes & staticClassAttributes) != staticClassAttributes)
                    {
                        continue;
                    }

                    var typeInfo = TypeDataBuilder.CreateForType(type);
                    if (typeInfo.IsDebugTarget)
                    {
                        data.Add(typeInfo);
                    }
                }
            }

            return data;
        }

        private static ToggleCommandModel CreateToggle(PropertyData property, Type type, object instance)
        {
            if (instance == null)
            {
                return ToggleCommandModel.Create(property.NameAttribute?.Name ?? property.Info.Name, type, property.Info.Name);
            }
            else
            {
                return ToggleCommandModel.Create(property.NameAttribute?.Name ?? property.Info.Name, instance, property.Info.Name);
            }
        }

        private static ActionCommandModel CreateAction(MethodData method, object instance)
        {
            return new ActionCommandModel(method.NameAttribute?.Name ?? method.Info.Name, () => { method.Info.Invoke(instance, null); });
        }

        private static TextCommandModel CreateTextActionCommand(MethodData method, object instance, string defaultValue)
        {
            return new TextCommandModel(method.NameAttribute?.Name ?? method.Info.Name, (str) => { method.Info.Invoke(instance, new object[] { str }); }, defaultValue);
        }

        private static IntTextCommandModel CreateIntActionCommand(MethodData method, object instance, int defaultValue)
        {
            return new IntTextCommandModel(method.NameAttribute?.Name ?? method.Info.Name, (str) => { method.Info.Invoke(instance, new object[] { str }); }, defaultValue);
        }

        private static DropdownCommandModel CreateDropdown(PropertyData property, object instance)
        {
            var current = property.Info.GetValue(instance);

            return DropdownCommandModel.Create(property.NameAttribute?.Name ?? property.Info.Name, property.Values, current, (newValue) =>
            {
                property.Info.SetValue(instance, newValue);
            });
        }

        private static CarouselCommandModel CreateCarousel(PropertyData property, object instance)
        {
            var current = property.Info.GetValue(instance);

            return CarouselCommandModel.Create(property.NameAttribute?.Name ?? property.Info.Name, property.Values, current, (newValue) =>
            {
                property.Info.SetValue(instance, newValue);
            });
        }
    }
}
