using System;
using System.Linq;

namespace Undebugger.Model.Commands.Builtin
{
    public class DropdownCommandModel : CommandModel
    {
        public static DropdownCommandModel Create(NameTag title, object[] values, object current, Action<object> onChanged)
        {
            var index = Array.IndexOf(values, current);
            if (index == -1)
            {
                index = 0;
            }

            ValueReferenceGetter<int> getIndex = () => index;
            ValueReferenceSetter<int> setIndex = (i) =>
            {
                index = i;
                onChanged?.Invoke(values[index]);
            };

            return new DropdownCommandModel(title, new ValueRef<int>(getIndex, setIndex), values);
        }

        public static DropdownCommandModel Create(object[] values, object current, Action<object> onChanged)
        {
            return Create(default, values, current, onChanged);
        }

        public static DropdownCommandModel Create<T>(NameTag title, T[] values, T current, Action<T> onChanged)
        {
            var objectValues = values.Cast<object>().ToArray();

            return Create(title, objectValues, current, (value) =>
            {
                onChanged?.Invoke((T)value);
            });
        }

        public static DropdownCommandModel Create<T>(T[] values, T current, Action<T> onChanged)
        {
            return Create(default, values, current, onChanged);
        }

        public NameTag Title
        { get; private set; }

        public object Value => values[Index];
        public object[] Values => values;
        public int Index => index.Value;

        private ValueRef<int> index;
        private object[] values;

        public DropdownCommandModel(NameTag title, ValueRef<int> index, object[] values)
            : this(index, values)
        {
            Title = title;
        }

        public DropdownCommandModel(ValueRef<int> index, object[] values)
        {
            this.index = index;
            this.values = values;
        }

        public void Set(int value)
        {
            index.Value = Wrap(value, values.Length);
        }

        private static int Wrap(int index, int count)
        {
            return ((index % count) + count) % count;
        }
    }
}
