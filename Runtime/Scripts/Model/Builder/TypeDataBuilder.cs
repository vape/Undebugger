#if UNITY_EDITOR
#define UNDEBUGGER_MODEL_BUILDER_VALIDATION
#endif

using System;
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
                    if (!ValidateActionSignature(methods[i]))
                    {
                        continue;
                    }
#endif

                    EnsureMethodsCollection(ref data);

                    var methodData = new MethodData()
                    {
                        Info = methods[i],
                        Type = MethodType.Action,
                        NameAttribute = methods[i].GetCustomAttribute<UndebuggerNameAttribute>(),
                        PriorityAttribute = methods[i].GetCustomAttribute<UndebuggerPriorityAttribute>()
                    };

                    data.Methods.Add(methodData);
                    continue;
                }

                var handlerAttribute = methods[i].GetCustomAttribute<UndebuggerMenuHandlerAttribute>();
                if (handlerAttribute != null)
                {
#if UNDEBUGGER_MODEL_BUILDER_VALIDATION
                    if (!ValidateHandlerMethodSignature(methods[i]))
                    {
                        continue;
                    }
#endif

                    EnsureMethodsCollection(ref data);

                    var methodData = new MethodData()
                    {
                        Info = methods[i],
                        Type = MethodType.Handler,
                        NameAttribute = methods[i].GetCustomAttribute<UndebuggerNameAttribute>(),
                        PriorityAttribute = methods[i].GetCustomAttribute<UndebuggerPriorityAttribute>()
                    };

                    data.Methods.Add(methodData);
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
                    if (!ValidateToggleSignature(properties[i]))
                    {
                        continue;
                    }
#endif

                    EnsurePropertiesCollection(ref data);

                    var propertyData = new PropertyData()
                    {
                        Info = properties[i],
                        NameAttribute = properties[i].GetCustomAttribute<UndebuggerNameAttribute>(),
                        PriorityAttribute = properties[i].GetCustomAttribute<UndebuggerPriorityAttribute>(),
                        Type = PropertyType.Toggle
                    };

                    data.Properties.Add(propertyData);
                }

                var dropdownAttribute = properties[i].GetCustomAttribute<UndebuggerDropdownAttribute>();
                if (dropdownAttribute != null)
                {
#if UNDEBUGGER_MODEL_BUILDER_VALIDATION
                    if (!ValidateDropdownSignature(properties[i]))
                    {
                        continue;
                    }
#endif

                    EnsurePropertiesCollection(ref data);

                    var propertyData = new PropertyData()
                    {
                        Info = properties[i],
                        NameAttribute = properties[i].GetCustomAttribute<UndebuggerNameAttribute>(),
                        PriorityAttribute = properties[i].GetCustomAttribute<UndebuggerPriorityAttribute>(),
                        Type = PropertyType.Dropdown,
                        Values = dropdownAttribute.Values
                    };

                    data.Properties.Add(propertyData);
                }

                var carouselAttribute = properties[i].GetCustomAttribute<UndebuggerDropdownAttribute>();
                if (carouselAttribute != null)
                {
#if UNDEBUGGER_MODEL_BUILDER_VALIDATION
                    if (!ValidateDropdownSignature(properties[i]))
                    {
                        continue;
                    }
#endif

                    EnsurePropertiesCollection(ref data);

                    var propertyData = new PropertyData()
                    {
                        Info = properties[i],
                        NameAttribute = properties[i].GetCustomAttribute<UndebuggerNameAttribute>(),
                        PriorityAttribute = properties[i].GetCustomAttribute<UndebuggerPriorityAttribute>(),
                        Type = PropertyType.Carousel,
                        Values = carouselAttribute.Values
                    };

                    data.Properties.Add(propertyData);
                }
            }

            return data;
        }

        private static void EnsureMethodsCollection(ref TypeData typeData)
        {
            if (typeData.Methods == null)
            {
                typeData.Methods = new System.Collections.Generic.List<MethodData>();
                typeData.IsDebugTarget = true;
            }
        }

        private static void EnsurePropertiesCollection(ref TypeData typeData)
        {
            if (typeData.Properties == null)
            {
                typeData.Properties = new System.Collections.Generic.List<PropertyData>();
                typeData.IsDebugTarget = true;
            }
        }

#if UNDEBUGGER_MODEL_BUILDER_VALIDATION
        private static bool ValidateHandlerMethodSignature(MethodInfo method)
        {
            if (method.ReturnType != typeof(void))
            {
                Debug.LogError($"Invalid return type of {method}, must be void");
                return false;
            }

            var parameters = method.GetParameters();
            if (parameters.Length != 1 || parameters[0].ParameterType != typeof(MenuModel))
            {
                Debug.LogError($"Invalid number of parameters for {method}, must be single parameter of type {typeof(MenuModel)}");
                return false;
            }

            return true;
        }

        private static bool ValidateActionSignature(MethodInfo method)
        {
            if (method.ReturnType != typeof(void))
            {
                Debug.LogError($"Invalid return type of {method}, must be void");
                return false;
            }

            if (method.GetParameters().Length != 0)
            {
                Debug.LogError($"Invalid number of parameters for {method}, must be zero");
                return false;
            }

            return true;
        }

        private static bool ValidateToggleSignature(PropertyInfo property)
        {
            if (property.PropertyType != typeof(bool))
            {
                Debug.LogError($"Property {property} must be of type 'bool'");
                return false;
            }

            if (!property.CanWrite || !property.CanRead)
            {
                Debug.LogError($"Property {property} must be both readable and writable");
                return false;
            }

            return true;
        }

        private static bool ValidateDropdownSignature(PropertyInfo property)
        {
            if (!property.CanWrite || !property.CanRead)
            {
                Debug.LogError($"Property {property} must be both readable and writable");
                return false;
            }

            return true;
        }
#endif
    }
}
