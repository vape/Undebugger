using System;

namespace Undebugger.Model.Commands.Builtin
{
    public class ActionCommandModel : CommandModel
    {
        public NameTag Name
        { get; private set; }

        private Action action;

        public ActionCommandModel(NameTag name, Action action)
        {
            Name = name;

            this.action = action;
        }

        public void Activate()
        {
            action?.Invoke();
        }
    }
}
