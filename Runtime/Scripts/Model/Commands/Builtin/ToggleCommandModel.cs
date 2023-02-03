using System;
using System.Reflection;

namespace Undebugger.Model.Commands.Builtin
{
    public class ToggleCommandModel : CommandModel
    {
        public static ToggleCommandModel Create(string name, object obj, string propertyName)
        {
            var type = obj.GetType();

            PropertyInfo property = null;
            FieldInfo field = null;

            property = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (property == null)
            {
                field = type.GetField(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            }

            ValueReferenceGetter<bool> getter = () =>
            {
                if (property != null)
                {
                    return (bool)property.GetValue(obj);
                }

                return (bool)field.GetValue(obj);
            };
            ValueReferenceSetter<bool> setter = (value) =>
            {
                if (property != null)
                {
                    property.SetValue(obj, value);
                }
                else
                {
                    field.SetValue(obj, value);
                }
            };

            return new ToggleCommandModel(name, new ValueRef<bool>(getter, setter));
        }

        public static ToggleCommandModel Create(string name, Type type, string propertyName)
        {
            PropertyInfo property = null;
            FieldInfo field = null;

            property = type.GetProperty(propertyName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            if (property == null)
            {
                field = type.GetField(propertyName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            }

            ValueReferenceGetter<bool> getter = () =>
            {
                if (property != null)
                {
                    return (bool)property.GetValue(null);
                }

                return (bool)field.GetValue(null);
            };
            ValueReferenceSetter<bool> setter = (value) =>
            {
                if (property != null)
                {
                    property.SetValue(null, value);
                }
                else
                {
                    field.SetValue(null, value);
                }
            };

            return new ToggleCommandModel(name, new ValueRef<bool>(getter, setter));
        }

        public NameTag Name
        { get; private set; }
        public ValueRef<bool> Reference
        { get; private set; }

        public ToggleCommandModel(NameTag name, ValueRef<bool> value)
        {
            Name = name;
            Reference = value;
        }
    }
}
