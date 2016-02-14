// Permission is hereby granted, free of charge, to any person obtaining 
// a copy of this software and associated documentation files (the 
// "Software"), to deal in the Software without restriction, including 
// without limitation the rights to use, copy, modify, merge, publish, 
// distribute, sublicense, and/or sell copies of the Software, and to 
// permit persons to whom the Software is furnished to do so, subject to 
// the following conditions: 
//  
// The above copyright notice and this permission notice shall be 
// included in all copies or substantial portions of the Software. 
//  
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE 
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION 
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
// 
// Copyright (c) 2009 Novell, Inc. (http://www.novell.com) 
// 
// Authors: 
//  Matt Guo <matt@mattguo.com>
// 

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Mono.UIAutomation.Services;

namespace System.Windows.Automation
{
	internal class NativeMethods
	{
		static NativeMethods()
		{
			string [] args = new string [0];
			Gdk.Global.InitCheck (ref args);
		}

		private static bool IsWindowsSys {
			get {
				PlatformID os = Environment.OSVersion.Platform;
				return (os == PlatformID.Win32Windows || os == PlatformID.Win32NT ||
					os == PlatformID.Win32S || os == PlatformID.WinCE);
			}
		}

		private static IntPtr [] GetChildWindowHandles (IntPtr rootWin) {
			IntPtr rootReturn;
			IntPtr parentReturn;
			IntPtr childrenReturn;
			int childrenReturnCount;
			try {
				IntPtr dpy = gdk_x11_display_get_xdisplay (Gdk.Display.Default.Handle);
				if (XQueryTree (dpy, rootWin, out rootReturn, out parentReturn,
				                out childrenReturn, out childrenReturnCount) == 0)
					return new IntPtr [0];
				IntPtr [] children = new IntPtr [childrenReturnCount];
				if (childrenReturnCount > 0)
					Marshal.Copy (childrenReturn, children, 0, childrenReturnCount);
				return children;
			} catch (Exception ex) {
				Log.Error ("Error, probably due to calling GDK or XQueryTree: {0}", ex.Message);
				return new IntPtr [0];
			}
		}

		private static IntPtr ChildWindowAtPosition (IntPtr root,
								int px, int py, bool checkRoot)
		{
			if (checkRoot) {
				IntPtr dpy = gdk_x11_display_get_xdisplay (Gdk.Display.Default.Handle);
				var rootWin = gdk_x11_window_foreign_new_for_display (dpy, root);
				if (gdk_window_is_viewable (rootWin) == 0)
					return IntPtr.Zero;
				int x, y, w, h;
				gdk_window_get_origin (rootWin, out x, out y);
				gdk_window_get_size (rootWin, out w, out h);
				if (x > px || x + w < px || y > py || y + h < py)
					return IntPtr.Zero;
			}
			foreach (var child in GetChildWindowHandles (root)) {
				var ret = ChildWindowAtPosition (child, px, py, true);
				if (ret != IntPtr.Zero)
					return ret;
			}
			return root;
		}

		public static void GetScreenBound (out int width, out int height)
		{
			var screen = Gdk.Screen.Default;
			width = screen.Width;
			height = screen.Height;
		}

		public static IntPtr WindowAtPosition (int px, int py)
		{
			var startHandle = IntPtr.Zero;
			var topWin = TopWindowAtPosition (px, py);
			if (topWin != null)
				startHandle = GetWindowHandle (topWin);
			if (startHandle == IntPtr.Zero)
				startHandle = RootWindowHandle;
			return ChildWindowAtPosition (startHandle, px, py, false);
		}

		public static Gdk.Window TopWindowAtPosition (int px, int py)
		{
			var windowStack = Gdk.Screen.Default.WindowStack;
			Gdk.Window ret = null;
			foreach (Gdk.Window win in windowStack) {
				//The latter item in "windowStack" has higher z-order
				if (win.IsViewable && win.FrameExtents.Contains (px, py))
					ret = win;
			}
			return ret;
		}

		public static IntPtr GetWindowHandle (Gdk.Window win)
		{
			if (win == null)
				return IntPtr.Zero;
			try {
				if (IsWindowsSys)
					return gdk_win32_window_get_handle (win.Handle);
				else
					return gdk_x11_window_get_xid (win.Handle);
			} catch (Exception ex) {
				Log.Error ("Error calling libgdk: {0}", ex.Message);
				return IntPtr.Zero;
			}
		}

		public static IntPtr RootWindowHandle
		{
			get {
				return GetWindowHandle (Gdk.Screen.Default.RootWindow);
			}
		}

		[DllImport ("libgdk-3.so.0")]
		internal static extern IntPtr gdk_x11_window_get_xid (IntPtr handle);

		[DllImport ("libgdk-3.0-dll")]
		internal static extern IntPtr gdk_win32_window_get_handle (IntPtr handle);

		[DllImport ("libgdk-3.so.0")]
		internal static extern IntPtr gdk_x11_display_get_xdisplay (IntPtr handle);

		[DllImport ("libgdk-3.so.0")]
		internal static extern IntPtr gdk_x11_window_foreign_new_for_display (IntPtr display, IntPtr window);

		[DllImport ("libgdk-3.so.0")]
		internal static extern int gdk_window_is_viewable (IntPtr window);

		[DllImport ("libgdk-3.so.0")]
		internal static extern int gdk_window_get_origin (IntPtr window, out int x, out int y);

		[DllImport ("libgdk-3.so.0")]
		internal static extern int gdk_window_get_size (IntPtr window, out int x, out int y);

		[DllImport("libX11.so")]
		private static extern int XQueryTree(IntPtr display, IntPtr w, out IntPtr rootReturn, out IntPtr parentReturn, out IntPtr childrenReturn, out int childrenReturnCount);
	}
}
