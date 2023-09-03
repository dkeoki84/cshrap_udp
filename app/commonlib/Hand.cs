using System;
using System.Runtime.InteropServices;
using System.Collections;

namespace CommonLib
{
    [StructLayout(LayoutKind.Sequential)]
    public class FingerInfo
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public Vector3 accel;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public Vector3 force;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public Vector6 temp;
    }

    public class HandInfo
    {
        public const int FINGER_NUM = 5;
        public int seq;
        public FingerInfo[] fingerInfos = new FingerInfo[FINGER_NUM];
    }
}