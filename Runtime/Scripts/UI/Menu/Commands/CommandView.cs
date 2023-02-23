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
        protected ICommandsGroupContext context;
        protected CommandModel model;

        public virtual void Setup(ICommandsGroupContext context, CommandModel model)
        {
            this.context = context;
            this.model = model;
        }

        protected virtual void OnAfterAction()
        {
            if (model?.CloseMenuOnAction ?? false)
            {
                context.TryCloseOnAction();
            }
        }
    }

    public abstract class CommandView<TModel> : CommandView
        where TModel : CommandModel
    {
        protected new TModel model;

        public sealed override void Setup(ICommandsGroupContext context, CommandModel model)
        {
            base.Setup(context, model);

            this.model = model as TModel;
            Setup(this.model);
        }

        protected virtual void Setup(TModel model)
        { }
    }
}
