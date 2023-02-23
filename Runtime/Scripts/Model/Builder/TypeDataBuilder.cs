#if UNITY_EDITOR
#define UNDEBUGGER_MODEL_BUILDER_VALIDATION
#endif

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Undebugger.Model.Builder
{
    internal static class TypeDataBuilder
    {
        public const TypeAttributes StaticTypeAttributes = TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit;

        public static TypeData CreateForType(Type type)
        {
            var targetAttribute = type.GetCustomAttribute<UndebuggerTargetAttribute>();
            if (targetAttribute == null)
            {
                return new TypeData()
                {
                    IsDebugTarget = false
                };
            }

            var data = new TypeData()
            {
                Type = type,
                NameAttribute = type.GetCustomAttribute<UndebuggerNameAttribute>(),
                PriorityAttribute = type.GetCustomAttribute<UndebuggerPriorityAttribute>()
            };

            var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            for (int i = 0; i < methods.Length; ++i)
            {
                var actionAttribute = methods[i].GetCustomAttribute<UndebuggerActionAttribute>();
                if (actionAttribute != null)
                {
#if UNDEBUGGER_MODEL_BUILDER_VALIDATION
                    if (!Validate(methods[i], typeof(void)))
                    {
                        continue;
                    }
#endif
                    EnsureMethodsCollection(ref data);
                    AddMethod(data.Methods, methods[i], MethodType.Action);
                    continue;
                }

                var textActionAttribute = methods[i].GetCustomAttribute<UndebuggerTextActionAttribute>();
                if (textActionAttribute != null)
                {
#if UNDEBUGGER_MODEL_BUILDER_VALIDATION
                    if (!Validate(methods[i], typeof(void), typeof(string)))
                    {
                        continue;
                    }
#endif
                    EnsureMethodsCollection(ref data);
                    AddMethod(data.Methods, methods[i], MethodType.TextAction);
                    continue;
                }

                var intActionAttribute = methods[i].GetCustomAttribute<UndebuggerIntActionAttribute>();
                if (intActionAttribute != null)
                {
#if UNDEBUGGER_MODEL_BUILDER_VALIDATION
                    if (!Validate(methods[i], typeof(void), typeof(int)))
                    {
                        continue;
                    }
#endif
                    EnsureMethodsCollection(ref data);
                    AddMethod(data.Methods, methods[i], MethodType.IntAction);
                    continue;
                }

                var handlerAttribute = methods[i].GetCustomAttribute<UndebuggerMenuHandlerAttribute>();
                if (handlerAttribute != null)
                {
#if UNDEBUGGER_MODEL_BUILDER_VALIDATION
                    if (!Validate(methods[i], typeof(void), typeof(MenuModel)))
                    {
                        continue;
                    }
#endif

                    EnsureMethodsCollection(ref data);
                    AddMethod(data.Methods, methods[i], MethodType.Handler);
                    continue;
                }
            }

            var properties = type.GetProperties(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            for (int i = 0; i < properties.Length; ++i)
            {
                var toggleAttribute = properties[i].GetCustomAttribute<UndebuggerToggleAttribute>();
                if (toggleAttribute != null)
                {
#if UNDEBUGGER_MODEL_BUILDER_VALIDATION
                    if (!Validate(properties[i], typeof(bool), readable: true, writable: true))
                    {
                        continue;
                    }
#endif

                    EnsurePropertiesCollection(ref data);
                    AddProperty(data.Properties, properties[i], PropertyType.Toggle, null);
                }

                var dropdownAttribute = properties[i].GetCustomAttribute<UndebuggerDropdownAttribute>();
                if (dropdownAttribute != null)
                {
#if UNDEBUGGER_MODEL_BUILDER_VALIDATION
                    if (!Validate(properties[i], readable: true, writable: true))
                    {
                        continue;
                    }
#endif

                    EnsurePropertiesCollection(ref data);
                    AddProperty(data.Properties, properties[i], PropertyType.Dropdown, dropdownAttribute.Values);
                    continue;
                }

                var carouselAttribute = properties[i].GetCustomAttribute<UndebuggerCarouselAttribute>();
                if (carouselAttribute != null)
                {
#if UNDEBUGGER_MODEL_BUILDER_VALIDATION
                    if (!Validate(properties[i], readable: true, writable: true))
                    {
                        continue;
                    }
#endif

                    EnsurePropertiesCollection(ref data);
                    AddProperty(data.Properties, properties[i], PropertyType.Carousel, carouselAttribute.Values);
                    continue;
                }
            }

            return data;
        }

        private static void EnsureMethodsCollection(ref TypeData typeData)
        {
            if (typeData.Methods == null)
            {
                typeData.Methods = new List<MethodData>();
                typeData.IsDebugTarget = true;
            }
        }

        private static void EnsurePropertiesCollection(ref TypeData typeData)
        {
            if (typeData.Properties == null)
            {
                typeData.Properties = new List<PropertyData>();
                typeData.IsDebugTarget = true;
            }
        }

        private static void AddProperty(List<PropertyData> properties, PropertyInfo info, PropertyType type, object[] values)
        {
            var property = new PropertyData()
            {
                Info = info,
                NameAttribute = info.GetCustomAttribute<UndebuggerNameAttribute>(),
                PriorityAttribute = info.GetCustomAttribute<UndebuggerPriorityAttribute>(),
                DefaultValueAttribute = info.GetCustomAttribute<UndebuggerDefaultValueAttribute>(),
                Type = type,
                Values = values
            };

            properties.Add(property);
        }

        private static void AddMethod(List<MethodData> methods, MethodInfo info, MethodType type)
        {
            var data = new MethodData()
            {
                Info = info,
                Type = type,
                NameAttribute = info.GetCustomAttribute<UndebuggerNameAttribute>(),
                PriorityAttribute = info.GetCustomAttribute<UndebuggerPriorityAttribute>(),
                DefaultValueAttribute = info.GetCustomAttribute<UndebuggerDefaultValueAttribute>()
            };

            methods.Add(data);
        }

#if UNDEBUGGER_MODEL_BUILDER_VALIDATION
        private static bool Validate(PropertyInfo property, bool? readable, bool? writable)
        {
            if (readable != null && property.CanRead != readable)
            {
                Debug.LogError($"Property {property} must be readable");
                return false;
            }

            if (writable != null && property.CanWrite != writable)
            {
                Debug.LogError($"Property {property} must be writable");
                return false;
            }

            return true;
        }

        private static bool Validate(PropertyInfo property, Type type, bool? readable, bool? writable)
        {
            if (property.PropertyType != type)
            {
                Debug.LogError($"Invalid property {property} type, must be {type}, got {property.PropertyType} instead");
                return false;
            }

            return Validate(property, readable, writable);
        }

        private static bool Validate(MethodInfo method, Type returnType, params Type[] paramTypes)
        {
            if (method.ReturnType != returnType)
            {
                Debug.LogError($"Invalid return type of {method}, must be {returnType}");
                return false;
            }

            var parameters = method.GetParameters();
            if (parameters.Length != paramTypes.Length)
            {
                Debug.LogError($"Invalid number of parameters for {method}, must be {paramTypes.Length}, got {parameters.Length} instead");
                return false;
            }

            for (int i = 0; i < paramTypes.Length; ++i)
            {
                var type = parameters[i].ParameterType;
                if (type != paramTypes[i])
                {
                    Debug.LogError($"Invalid parameter type at {i}, must be {paramTypes[i]}, got {type} instead");
                    return false;
                }
            }

            return true;
        }
#endif
    }
}
