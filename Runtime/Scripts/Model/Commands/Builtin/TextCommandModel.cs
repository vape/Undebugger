using System;
using UnityEngine.UI;

namespace Undebugger.Model.Commands.Builtin
{
    public abstract class BaseTextCommandModel : CommandModel
    {
        public NameTag Title 
        { get; private set; }
        public InputField.ContentType ContentType
        { get; protected set; }

        public BaseTextCommandModel(NameTag title, InputField.ContentType contentType)
        {
            Title = title;
            ContentType = contentType;
        }

        public abstract void Apply(string data);
    }

    public abstract class BaseTextCommandModel<T> : BaseTextCommandModel
    {
        private Action<T> onApply;

        public BaseTextCommandModel(NameTag name, Action<T> onApply, InputField.ContentType contentType)
            : base(name, contentType)
        {
            this.onApply = onApply;
        }

        protected abstract bool TryConvert(string data, out T value);

        public override void Apply(string data)
        {
            if (TryConvert(data, out var value))
            {
                onApply?.Invoke(value);
            }
        }
    }

    public class TextCommandModel : BaseTextCommandModel<string>
    {
        public TextCommandModel(Action<string> onApply)
            : base(default, onApply, InputField.ContentType.Standard)
        { }

        public TextCommandModel(NameTag name, Action<string> onApply) 
            : base(name, onApply, InputField.ContentType.Standard)
        { }

        protected override bool TryConvert(string data, out string value)
        {
            value = data;
            return true;
        }
    }

    public class IntTextCommandModel : BaseTextCommandModel<int>
    {
        public IntTextCommandModel(Action<int> onApply)
            : base(default, onApply, InputField.ContentType.IntegerNumber)
        { }

        public IntTextCommandModel(NameTag name, Action<int> onApply)
            : base(name, onApply, InputField.ContentType.IntegerNumber)
        { }

        protected override bool TryConvert(string data, out int value)
        {
            return int.TryParse(data, out value);
        }
    }
}
