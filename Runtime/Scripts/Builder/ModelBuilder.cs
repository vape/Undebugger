using Deszz.Undebugger.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Deszz.Undebugger.Builder
{
    public static class ModelBuilder
    {
        private static readonly string[] ignoredAssemblies = new string[]
        {
            "UnityEngine",
            "System",
            "UnityEditor",
            "Unity",
            "netstandart",
            "Microsoft",
            "mscorelib",
            "Mono",
        };

        private static List<MethodInfo> staticHandlers;

        public static MenuModel Build()
        {
            if (staticHandlers == null)
            {
                CacheStaticHandlers();
            }

            var model = new MenuModel();

            foreach (var handler in GameObject.FindObjectsOfType<MonoBehaviour>().OfType<IDebugMenuHandler>())
            {
                handler.OnBuildingModel(model);
            }

            var param = new object[] { model };
            for (int i = 0; i < staticHandlers.Count; i++)
            {
                staticHandlers[i].Invoke(null, param);
            }

            return model;
        }

        private static void CacheStaticHandlers()
        {
            const TypeAttributes staticClassAttributes = TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit;

            staticHandlers = new List<MethodInfo>();

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

                    var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

                    for (int k = 0; k < methods.Length; ++k)
                    {
                        if (methods[k].GetCustomAttribute<UndebuggerHandlerMethodAttribute>() == null)
                        {
                            continue;
                        }

                        var parameters = methods[k].GetParameters();
                        if (parameters.Length != 1 || parameters[0].ParameterType != typeof(MenuModel))
                        {
                            Debug.LogError($"Method {methods[k].Name} has wrong signature, expected {nameof(MenuModel)} as single parameter");
                            continue;
                        }

                        staticHandlers.Add(methods[k]);
                    }
                }
            }
        }
    }
}
