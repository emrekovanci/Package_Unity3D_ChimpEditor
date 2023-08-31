using UnityEditor;

namespace Chimp.Editor.Terminal
{
    internal static class TerminalSettings
    {
        private static class EditorPrefsKeys
        {
            private const string PREFIX = "Chimp.UnityTerminalLauncher.";
            internal const string TERMINAL_TYPE = PREFIX + "TerminalType";
        }

        internal static TerminalType TerminalType
        {
            get
            {
                if (!EditorPrefs.HasKey(EditorPrefsKeys.TERMINAL_TYPE))
                {
                    return TerminalType.Auto;
                }
                int value = EditorPrefs.GetInt(EditorPrefsKeys.TERMINAL_TYPE);
                return (TerminalType)System.Enum.ToObject(typeof(TerminalType), value);
            }
            set => EditorPrefs.SetInt(EditorPrefsKeys.TERMINAL_TYPE, (int)value);
        }

        internal static int TerminalTypeInt
        {
            get => (int)TerminalType;
            set => TerminalType = (TerminalType)System.Enum.ToObject(typeof(TerminalType), value);
        }
    }

    internal enum TerminalType
    {
        Auto,
        WindowsTerminal,
        PowerShell,
        Cmd,
        MacTerminal,
        GitBash
    }
}