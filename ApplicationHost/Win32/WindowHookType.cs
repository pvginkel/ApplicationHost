using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApplicationHost.Win32
{
    internal enum WindowHookType
    {
        /// <summary>
        /// WH_JOURNALRECORD
        /// </summary>
        JournalRecord = 0,
        /// <summary>
        /// WH_JOURNALPLAYBACK
        /// </summary>
        JournalPlayback = 1,
        /// <summary>
        /// WH_KEYBOARD
        /// </summary>
        Keyboard = 2,
        /// <summary>
        /// WH_GETMESSAGE
        /// </summary>
        GetMessage = 3,
        /// <summary>
        /// WH_CALLWNDPROC
        /// </summary>
        CallWndProc = 4,
        /// <summary>
        /// WH_CBT
        /// </summary>
        ComputerBasedTraining = 5,
        /// <summary>
        /// WH_SYSMSGFILTER
        /// </summary>
        SystemMessageFilter = 6,
        /// <summary>
        /// WH_MOUSE
        /// </summary>
        Mouse = 7,
        /// <summary>
        /// WH_HARDWARE
        /// </summary>
        Hardware = 8,
        /// <summary>
        /// WH_DEBUG
        /// </summary>
        Debug = 9,
        /// <summary>
        /// WH_SHELL
        /// </summary>
        Shell = 10,
        /// <summary>
        /// WH_FOREGROUNDIDLE
        /// </summary>
        ForegroundIdle = 11,
        /// <summary>
        /// WH_CALLWNDPROCRET
        /// </summary>
        CallWndProcReturn = 12,
        /// <summary>
        /// WH_KEYBOARD_LL
        /// </summary>
        KeyboardLowLevel = 13,
        /// <summary>
        /// WH_MOUSE_LL
        /// </summary>
        MouseLowLevel = 14
    }
}
