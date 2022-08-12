using System;
using UnityEngine;

namespace Deszz.Undebugger
{
    [Flags]
    public enum ActivationTriggerAction : uint
    {
        None = 0,
        Open = 1,
        Close = 2
    }

    public abstract class ActivationTrigger
    {
        public virtual ActivationTriggerAction Action => ActivationTriggerAction.Open | ActivationTriggerAction.Close;

        public abstract bool IsTriggered();
    }

    public class SingleKeyActivationTrigger : ActivationTrigger
    {
        public override ActivationTriggerAction Action => action;

        public readonly KeyCode Key;

        private readonly ActivationTriggerAction action;

        public SingleKeyActivationTrigger(KeyCode key)
        {
            Key = key;
            action = base.Action;
        }

        public SingleKeyActivationTrigger(KeyCode key, ActivationTriggerAction action)
        {
            Key = key;
            this.action = action;
        }

        public override bool IsTriggered()
        {
            return Input.GetKeyUp(Key);
        }
    }

    public class TouchActivationTrigger : ActivationTrigger
    {
        public override ActivationTriggerAction Action => action;

        public readonly int TouchesCount;

        private readonly ActivationTriggerAction action;

        public TouchActivationTrigger(int touchesCount)
        {
            TouchesCount = touchesCount;
            action = base.Action;
        }

        public TouchActivationTrigger(int touchesCount, ActivationTriggerAction action)
        {
            TouchesCount = touchesCount;
            this.action = action;
        }

        public override bool IsTriggered()
        {
            return Input.touchCount == TouchesCount;
        }
    }
}
