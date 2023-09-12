using System;
using System.Reflection;

using UnityEngine;

using UnityEditor;
using UnityEditor.ShortcutManagement;

namespace Chimp.Editor
{
    public static class ToggleProjectBrowser
    {
        // editor pref prefix
        private const string _prefix = "Chimp_ProjectBrowser";

        // editor prefs
        private const string _positionX = _prefix + "PositionX";
        private const string _positionY = _prefix + "PositionY";
        private const string _width = _prefix + "Width";
        private const string _height = _prefix + "Height";

        private static Rect s_defaultPosition = new(0.0f, 0.0f, 200.0f, 200.0f);
        private static bool s_toggled;

        [Shortcut(
            id: "Chimp_ToggleProjectWindow",
            context: null,
            defaultKeyCode: KeyCode.Space,
            defaultShortcutModifiers: ShortcutModifiers.Control,
            displayName = "Toggle Project Window"
        )]
        public static void ToggleProjectWindow()
        {
            // "UnityEditor.ProjectBrowser" is internal class, reflection required
            Type type = Assembly.GetAssembly(typeof(UnityEditor.Editor)).GetType("UnityEditor.ProjectBrowser");
            EditorWindow projectBrowser = EditorWindow.GetWindow(type, false, "Project", false);

            if (s_toggled)
            {
                SaveProjectBrowserPosition(projectBrowser);
                projectBrowser.Close();
            }
            else
            {
                projectBrowser.position = GetProjectBrowserPosition();
                projectBrowser.Show();
                projectBrowser.Repaint();
            }

            s_toggled = !s_toggled;
        }

        private static void SaveProjectBrowserPosition(EditorWindow projectBrowser)
        {
            EditorPrefs.SetFloat(_positionX, projectBrowser.position.x);
            EditorPrefs.SetFloat(_positionY, projectBrowser.position.y);
            EditorPrefs.SetFloat(_width, projectBrowser.position.width);
            EditorPrefs.SetFloat(_height, projectBrowser.position.height);
        }

        private static Rect GetProjectBrowserPosition()
        {
            float x = !EditorPrefs.HasKey(_positionX) ? s_defaultPosition.x : EditorPrefs.GetFloat(_positionX);
            float y = !EditorPrefs.HasKey(_positionY) ? s_defaultPosition.y : EditorPrefs.GetFloat(_positionY);
            float width = !EditorPrefs.HasKey(_width) ? s_defaultPosition.width : EditorPrefs.GetFloat(_width);
            float height = !EditorPrefs.HasKey(_height) ? s_defaultPosition.height : EditorPrefs.GetFloat(_height);

            return new(x, y, width, height);
        }
    }
}
