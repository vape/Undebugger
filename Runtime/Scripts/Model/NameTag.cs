using System;

namespace Undebugger.Model
{
    public struct NameTag
    {
        public string Value
        {
            get
            {
                if (isConstant)
                {
                    return constValue;
                }

                return getter?.Invoke();
            }
        }

        private bool isConstant;
        private readonly Func<string> getter;
        private readonly string constValue;

        public NameTag(string name)
        {
            getter = null;
            constValue = name;
            isConstant = true;
        }

        public NameTag(Func<string> func)
        {
            constValue = null;
            getter = func;
            isConstant = false;
        }

        public static implicit operator NameTag(string constantString)
        {
            return new NameTag(constantString);
        }

        public static implicit operator NameTag(Func<string> stringGetter)
        {
            return new NameTag(stringGetter);
        }

        public static implicit operator string(NameTag tag)
        {
            return tag.Value;
        }
    }
}
