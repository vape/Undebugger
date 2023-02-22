using System;
using UnityEngine.UI;

namespace Undebugger.Model.Commands.Builtin
{
    public abstract class BaseTextCommandModel : CommandModel
    {
        public abstract string DefaultStringValue
        { get; }

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
        public override string DefaultStringValue => DefaultValue?.ToString();

        public T DefaultValue => defaultValue;

        private Action<T> onApply;
        private T defaultValue;

        public BaseTextCommandModel(NameTag name, Action<T> onApply, InputField.ContentType contentType, T defaultValue = default)
            : base(name, contentType)
        {
            this.onApply = onApply;
            this.defaultValue = defaultValue;
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

        public TextCommandModel(NameTag name, Action<string> onApply, string defaultValue = default) 
            : base(name, onApply, InputField.ContentType.Standard, defaultValue)
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

        public IntTextCommandModel(NameTag name, Action<int> onApply, int defaultValue = default)
            : base(name, onApply, InputField.ContentType.IntegerNumber, defaultValue)
        { }

        protected override bool TryConvert(string data, out int value)
        {
            return int.TryParse(data, out value);
        }
    }
}
