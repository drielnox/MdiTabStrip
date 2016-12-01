using System;
using System.Runtime.InteropServices;

namespace MdiTabStrip
{
    internal static class NativeMethods
    {
        [DllImport("User32.dll", EntryPoint = "LoadCursorFromFileW", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
        internal static extern IntPtr LoadCursorFromFile(string filename);
    }
}
