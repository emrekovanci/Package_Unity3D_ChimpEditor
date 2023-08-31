﻿using System.Diagnostics;

namespace Chimp.Editor.Terminal
{
    internal class WindowsTerminalLauncher : TerminalLauncher
    {
        internal override string LauncherName => "wt.exe";

        internal override void Launch(string targetFolder)
        {
            base.Launch(targetFolder);

            Process.Start(LauncherName, $"-d \"{targetFolder}\"");
        }
    }
}