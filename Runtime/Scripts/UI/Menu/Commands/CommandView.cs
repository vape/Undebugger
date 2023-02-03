using Undebugger.Model.Commands;
using System;
using UnityEngine;

namespace Undebugger.UI.Menu.Commands
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class CommandViewAttribute : Attribute
    {
        public Type ModelType
        { get; set; }

        public CommandViewAttribute(Type modelType)
        {
            ModelType = modelType;
        }
    }

    public abstract class CommandView : MonoBehaviour
    {
        public virtual void Setup(CommandModel model)
        { }
    }

    public abstract class CommandView<TModel> : CommandView
        where TModel : CommandModel
    {
        protected TModel model;

        public sealed override void Setup(CommandModel model)
        {
            base.Setup(model);

            this.model = model as TModel;
            Setup(this.model);
        }

        protected virtual void Setup(TModel model)
        { }
    }
}
