using UnityEngine;

namespace Undebugger
{
    internal static class Preferences
    {
        private const string errorNotificationKey = "undebugger.ui.error_notification";
        private const int errorNotificationDefaultValue = 1; // enabled

        private const string autoCloseKey = "undebugger.commands.autoclose";
        private const int autoCloseDefaultValue = 1; // enabled

        public static bool AutoCloseEnabled
        {
            get
            {
                return PlayerPrefs.GetInt(autoCloseKey, autoCloseDefaultValue) > 0;
            }
            set
            {
                PlayerPrefs.SetInt(autoCloseKey, value ? 1 : 0);
            }
        }

        public static bool ErrorNotificationEnabled
        {
            get
            {
                return PlayerPrefs.GetInt(errorNotificationKey, errorNotificationDefaultValue) > 0;
            }
            set
            {
                PlayerPrefs.SetInt(errorNotificationKey, value ? 1 : 0);
            }
        }

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
