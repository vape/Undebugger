using System.Collections.Generic;

namespace Deszz.Undebugger
{
    public struct UndebuggerConfiguration
    {
        public static UndebuggerConfiguration CreateDefault()
        {
            return new UndebuggerConfiguration()
            {
                Triggers = new List<ActivationTrigger>()
                {
                    new SingleKeyActivationTrigger(UnityEngine.KeyCode.Escape, ActivationTriggerAction.Close),
                    new SingleKeyActivationTrigger(UnityEngine.KeyCode.F1),
                    new TouchActivationTrigger(4, ActivationTriggerAction.Open)
                }
            };
        }

        public List<ActivationTrigger> Triggers;
    }
}
