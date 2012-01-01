using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApplicationHost
{
    /// <summary>
    /// Specifies the available window styles. After the window has been created, these styles cannot be modified, except as noted.
    /// </summary>
    [Flags]
    public enum WindowStyle
    {
        /// <summary>
        /// The window has a thin-line border. (WS_BORDER)
        /// </summary>
        Border = 0x800000,
        /// <summary>
        /// The window has a title bar (includes the WS_BORDER style). (WS_CAPTION)
        /// </summary>
        Caption = 0xc00000,
        /// <summary>
        /// The window is a child window. A window with this style cannot have a menu bar. This style cannot be used with the WS_POPUP style. (WS_CHILD)
        /// </summary>
        Child = 0x40000000,
        /// <summary>
        /// Excludes the area occupied by child windows when drawing occurs within the parent window. This style is used when creating the parent window. (WS_CLIPCHILDREN)
        /// </summary>
        ClipChildren = 0x2000000,
        /// <summary>
        /// Clips child windows relative to each other; that is, when a particular child window receives a WM_PAINT message, the WS_CLIPSIBLINGS style clips all other overlapping child windows out of the region of the child window to be updated.
        /// If WS_CLIPSIBLINGS is not specified and child windows overlap, it is possible, when drawing within the client area of a child window, to draw within the client area of a neighboring child window. (WS_CLIPSIBLINGS)
        /// </summary>
        ClipSiblings = 0x4000000,
        /// <summary>
        /// The window is initially disabled. A disabled window cannot receive input from the user. To change this after a window has been created, use the EnableWindow function. (WS_DISABLED)
        /// </summary>
        Disabled = 0x8000000,
        /// <summary>
        /// The window has a border of a style typically used with dialog boxes. A window with this style cannot have a title bar. (WS_DLGFRAME)
        /// </summary>
        DialogFrame = 0x400000,
        /// <summary>
        /// The window is the first control of a group of controls. The group consists of this first control and all controls defined after it, up to the next control with the WS_GROUP style.
        /// The first control in each group usually has the WS_TABSTOP style so that the user can move from group to group. The user can subsequently change the keyboard focus from one control in the group to the next control in the group by using the direction keys.
        /// You can turn this style on and off to change dialog box navigation. To change this style after a window has been created, use the SetWindowLong function. (WS_GROUP)
        /// </summary>
        Group = 0x20000,
        /// <summary>
        /// The window has a horizontal scroll bar. (WS_HSCROLL)
        /// </summary>
        HorizontalScroll = 0x100000,
        /// <summary>
        /// The window is initially maximized. (WS_MAXIMIZE)
        /// </summary> 
        Maximize = 0x1000000,
        /// <summary>
        /// The window has a maximize button. Cannot be combined with the WS_EX_CONTEXTHELP style. The WS_SYSMENU style must also be specified. (WS_MAXIMIZEBOX)
        /// </summary> 
        MaximizeBox = 0x10000,
        /// <summary>
        /// The window is initially minimized. (WS_MINIMIZE)
        /// </summary>
        Minimize = 0x20000000,
        /// <summary>
        /// The window has a minimize button. Cannot be combined with the WS_EX_CONTEXTHELP style. The WS_SYSMENU style must also be specified. (WS_MINIMIZEBOX)
        /// </summary>
        MinimizeBox = 0x20000,
        /// <summary>
        /// The window is an overlapped window. An overlapped window has a title bar and a border. (WS_OVERLAPPED)
        /// </summary>
        Overlapped = 0x0,
        /// <summary>
        /// The window is an overlapped window. (WS_OVERLAPPEDWINDOW)
        /// </summary>
        OverlappedWindow = Overlapped | Caption | SystemMenu | SizeFrame | MinimizeBox | MaximizeBox,
        /// <summary>
        /// The window is a pop-up window. This style cannot be used with the WS_CHILD style. (WS_POPUP)
        /// </summary>
        Popup = unchecked((int)0x80000000L),
        /// <summary>
        /// The window is a pop-up window. The WS_CAPTION and WS_POPUPWINDOW styles must be combined to make the window menu visible. (WS_POPUPWINDOW)
        /// </summary>
        PopupWindow = Popup | Border | SystemMenu,
        /// <summary>
        /// The window has a sizing border. (WS_SIZEFRAME)
        /// </summary>
        SizeFrame = 0x40000,
        /// <summary>
        /// The window has a sizing border. Same as the WS_SIZEBOX style. (WS_THICKFRAME)
        /// </summary>
        ThickFrame = 0x40000,
        /// <summary>
        /// The window has a window menu on its title bar. The WS_CAPTION style must also be specified. (WS_SYSMENU)
        /// </summary>
        SystemMenu = 0x80000,
        /// <summary>
        /// The window is a control that can receive the keyboard focus when the user presses the TAB key.
        /// Pressing the TAB key changes the keyboard focus to the next control with the WS_TABSTOP style.  
        /// You can turn this style on and off to change dialog box navigation. To change this style after a window has been created, use the SetWindowLong function.
        /// For user-created windows and modeless dialogs to work with tab stops, alter the message loop to call the IsDialogMessage function. (WS_TABSTOP)
        /// </summary>
        TabStop = 0x10000,
        /// <summary>
        /// The window is initially visible. This style can be turned on and off by using the ShowWindow or SetWindowPos function. (WS_VISIBLE)
        /// </summary>
        Visible = 0x10000000,
        /// <summary>
        /// The window has a vertical scroll bar. (WS_VSCROLL)
        /// </summary>
        VerticalScroll = 0x200000
    }
}
