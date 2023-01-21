using System;

namespace Undebugger.Model
{
    public delegate T ValueReferenceGetter<T>();
    public delegate void ValueReferenceSetter<T>(T value);

    public struct ValueRef<T>
    {
        public T Value
        {
            get
            {
                return Get();
            }
            set
            {
                Set(value);
            }
        }

        public bool SupportSettingValue => setter != null;

        private ValueReferenceGetter<T> getter;
        private ValueReferenceSetter<T> setter;

        public ValueRef(ValueReferenceGetter<T> getter)
        {
            this.getter = getter;
            this.setter = null;
        }

        public ValueRef(ValueReferenceGetter<T> getter, ValueReferenceSetter<T> setter)
        {
            this.getter = getter;
            this.setter = setter;
        }

        public void Set(T value)
        {
            if (!SupportSettingValue)
            {
                throw new InvalidOperationException("Set operation is not supported.");
            }

            setter.Invoke(value);
        }

        public T Get()
        {
            return getter.Invoke();
        }
    }
}
