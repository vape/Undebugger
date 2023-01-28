using UnityEngine;

namespace Undebugger
{
    internal static class Preferences
    {
        public static void SetStatusSegmentFoldout(string id, bool value)
        {
            PlayerPrefs.SetInt($"undebugger.status.segment.{id}.foldout", value ? 1 : 0);
        }

        public static bool GetStatusSegmentFoldout(string id, bool defaultValue = false)
        {
            return PlayerPrefs.GetInt($"undebugger.status.segment.{id}.foldout", defaultValue: defaultValue ? 1 : 0) > 0;
        }
    }
}
