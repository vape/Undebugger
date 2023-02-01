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
        public static TypeData CreateForType(Type type, bool isStatic)
        {
            var targetAttribute = type.GetCustomAttribute<DebugTargetAttribute>();
            if (targetAttribute == null)
            {
                return new TypeData()
                {
                    IsDebugTarget = false
                };
            }

            var info = new TypeData()
            {
                Name = type.Name
            };

            if (targetAttribute.CommandsPage != null)
            {
                info.PageOverride = new TypeData.CommandsPageOverride()
                {
                    Valid = true,
                    Name = targetAttribute.CommandsPage,
                    Priority = targetAttribute.CommandsPagePriority
                };
            }

            var methods = type.GetMethods((isStatic ? BindingFlags.Static : BindingFlags.Instance) | BindingFlags.Public | BindingFlags.NonPublic);
            for (int i = 0; i < methods.Length; ++i)
            {
                var actionAttribute = methods[i].GetCustomAttribute<DebugActionAttribute>();
                if (actionAttribute != null)
                {
#if UNDEBUGGER_MODEL_BUILDER_VALIDATION
                    if (!ValidateActionSignature(methods[i]))
                    {
                        continue;
                    }
#endif

                    if (info.ActionMethods == null)
                    {
                        info.IsDebugTarget = true;
                        info.ActionMethods = new System.Collections.Generic.List<MethodInfo>();
                    }

                    info.ActionMethods.Add(methods[i]);
                    continue;
                }

                if (isStatic)
                {
                    var handlerAttribute = methods[i].GetCustomAttribute<DebugMenuHandlerAttribute>();
                    if (handlerAttribute != null)
                    {
#if UNDEBUGGER_MODEL_BUILDER_VALIDATION
                        if (!ValidateHandlerMethodSignature(methods[i]))
                        {
                            continue;
                        }
#endif

                        if (info.HandlerMethods == null)
                        {
                            info.IsDebugTarget = true;
                            info.HandlerMethods = new System.Collections.Generic.List<MethodInfo>();
                        }

                        info.HandlerMethods.Add(methods[i]);
                        continue;
                    }
                }
            }

            var properties = type.GetProperties((isStatic ? BindingFlags.Static : BindingFlags.Instance) | BindingFlags.Public | BindingFlags.NonPublic);
            for (int i = 0; i < properties.Length; ++i)
            {
                var toggleAttribute = properties[i].GetCustomAttribute<DebugToggleAttribute>();
                if (toggleAttribute != null)
                {
#if UNDEBUGGER_MODEL_BUILDER_VALIDATION
                    if (!ValidateToggleSignature(properties[i]))
                    {
                        continue;
                    }
#endif

                    if (info.ToggleProperties == null)
                    {
                        info.IsDebugTarget = true;
                        info.ToggleProperties = new System.Collections.Generic.List<PropertyInfo>();
                    }

                    info.ToggleProperties.Add(properties[i]);
                }
            }

            return info;
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
#endif
    }
}
