﻿using System;
using System.Runtime.InteropServices;

namespace PhotoFolder.Wpf.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct CWPRETSTRUCT
    {
        public IntPtr lResult;
        public IntPtr lParam;
        public IntPtr wParam;
        public uint message;
        public IntPtr hwnd;
    }
}
