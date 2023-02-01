using UnityEngine;

namespace Undebugger.Model.Status.Builtin
{
    internal class ApplicationInfoStatusSegment : StaticStatusSegmentDriver
    {
        private enum ScriptingBackend
        {
            Unknown,
            IL2CPP,
            Mono
        }

        private const string LabelColor = "#A2A2A2";

#if ENABLE_IL2CPP
        private const ScriptingBackend Backend = ScriptingBackend.IL2CPP;
#elif ENABLE_MONO
        private const ScriptingBackend Backend = ScriptingBackend.Mono;
#else
        private const ScriptingBackend Backend = ScriptingBackend.Unknown;
#endif

#if DEBUG
        private const bool IsDebugBuild = true;
#else
        private const bool IsDebugBuild = false;
#endif

        public static ApplicationInfoStatusSegment Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ApplicationInfoStatusSegment();
                }

                return instance;
            }
        }

        private static ApplicationInfoStatusSegment instance;

        public override int Priority
        { get { return -9001; } }

        public ApplicationInfoStatusSegment()
            : base("app_info", "Application", GenerateString())
        { }

        private static string GenerateString()
        {
            return
$@"<color={LabelColor}>Identifier:</color> {Application.identifier}
<color={LabelColor}>Version:</color> {Application.version}
<color={LabelColor}>System Language:</color> {Application.systemLanguage}
<color={LabelColor}>Unity Version:</color> {Application.unityVersion}
<color={LabelColor}>Platform:</color> {Application.platform}
<color={LabelColor}>Backend:</color> {Backend}
<color={LabelColor}>Debug:</color> {IsDebugBuild}";
        }
    }
}
