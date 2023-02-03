using Undebugger.Model.Commands;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Undebugger.UI.Menu.Commands
{
    public class CommandViewFactory
    {
        private Dictionary<Type, CommandView> templates;

        public CommandViewFactory(CommandView[] templates)
        {
            this.templates = new Dictionary<Type, CommandView>(templates.Length);

            for (int i = 0; i < templates.Length; ++i)
            {
                var type = templates[i].GetType();
                var attribute = Attribute.GetCustomAttribute(type, typeof(CommandViewAttribute)) as CommandViewAttribute;
                if (attribute != null)
                {
                    this.templates[attribute.ModelType] = templates[i];
                }
                else
                {
                    Debug.LogWarning($"No model is assigned to {type.Name} view");
                }
            }
        }

        public CommandView FindTemplate<TModel>()
            where TModel : CommandModel
        {
            return FindTemplate(typeof(TModel));
        }

        public CommandView FindTemplate(Type modelType)
        {
            while (modelType != null && modelType != typeof(CommandModel))
            {
                if (templates.TryGetValue(modelType, out var template))
                {
                    return template;
                }
                else
                {
                    modelType = modelType.BaseType;
                }
            }

            return null;
        }
    }
}
