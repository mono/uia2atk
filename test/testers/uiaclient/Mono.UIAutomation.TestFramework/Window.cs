// Window.cs: Window control class wrapper.
//
// This program is free software; you can redistribute it and/or modify it under
// the terms of the GNU General Public License version 2 as published by the
// Free Software Foundation.
//
// This program is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
// FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more
// details.
//
// You should have received a copy of the GNU General Public License along with
// this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
//
// Copyright (c) 2009 Novell, Inc (http://www.novell.com)
//
// Authors:
//	Ray Wang  (rawang@novell.com)
//	Felicia Mu  (fxmu@novell.com)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Automation;

namespace Mono.UIAutomation.TestFramework
{
	// The wrapper class of Window class.
	public class Window : Element
	{
		public static readonly ControlType UIAType = ControlType.Window;

		public Window (AutomationElement elm)
			: base (elm)
		{
		}

		// The methods and properties of TransformPattern
		public void Move (double x, double y)
		{
			Move (x, y, true);
		}

		public void Move (double x, double y, bool log)
		{
			if (log == true)
				procedureLogger.Action (string.Format ("Move window to ({0}, {1}).", x, y));

			TransformPattern tp = (TransformPattern) element.GetCurrentPattern (TransformPattern.Pattern);
			tp.Move (x, y);
		}

		public void Resize (double width, double height)
		{
			Resize (width, height, true);
		}

		public void Resize (double width, double height, bool log)
		{
			if (log == true)
				procedureLogger.Action (string.Format ("Resize window to {0} width, {1} height.", width, height));

			TransformPattern tp = (TransformPattern) element.GetCurrentPattern (TransformPattern.Pattern);
			tp.Resize (width, height);
		}

		public void Rotate (double degree)
		{
			Rotate (degree, true);
		}

		public void Rotate (double degree, bool log)
		{
			if (log == true)
				procedureLogger.Action (string.Format ("Rotate {0} {1} degree(s).", this.Name, degree));

			TransformPattern tp = (TransformPattern) element.GetCurrentPattern (TransformPattern.Pattern);
			tp.Rotate (degree);
		}

		public bool CanMove {
			get { return (bool) element.GetCurrentPropertyValue (TransformPattern.CanMoveProperty); }
		}

		public bool CanResize {
			get { return (bool) element.GetCurrentPropertyValue (TransformPattern.CanResizeProperty); }
		}

		public bool CanRotate {
			get { return (bool) element.GetCurrentPropertyValue (TransformPattern.CanRotateProperty); }
		}

		// The methods and properties of WindowPattern
		public void SetWindowVisualState (WindowVisualState state)
		{
			SetWindowVisualState (state, true);
		}

		public void SetWindowVisualState (WindowVisualState state, bool log)
		{
			if (log == true)
				procedureLogger.Action (string.Format ("Set {0} to be {1}.", this.Name, state));

			WindowPattern wp = (WindowPattern) element.GetCurrentPattern (WindowPattern.Pattern);
			wp.SetWindowVisualState (state);
		}

		public void Close ()
		{
			Close (true);
		}

		public void Close (bool log)
		{
			if (log == true)
				procedureLogger.Action (string.Format ("Close {0}.", this.Name));

			WindowPattern wp = (WindowPattern) element.GetCurrentPattern (WindowPattern.Pattern);
			wp.Close ();
		}

		public bool CanMaximize {
			get { return (bool) element.GetCurrentPropertyValue (WindowPattern.CanMaximizeProperty); }
		}

		public bool CanMinimize {
			get { return (bool) element.GetCurrentPropertyValue (WindowPattern.CanMinimizeProperty); }
		}

		public bool IsModal {
			get { return (bool) element.GetCurrentPropertyValue (WindowPattern.IsModalProperty); }
		}

		public bool IsTopmost {
			get { return (bool) element.GetCurrentPropertyValue (WindowPattern.IsTopmostProperty); }
		}

		public WindowVisualState WindowVisualState {
			get { return (WindowVisualState) element.GetCurrentPropertyValue (WindowPattern.WindowInteractionStateProperty); }
		}

		public WindowInteractionState WindowInteractionState {
			get { return (WindowInteractionState) element.GetCurrentPropertyValue (WindowPattern.WindowInteractionStateProperty); }
		}

		// Click "OK" button of the window.
		public void OK ()
		{
			ClickButton ("OK", true);
		}

		public void OK (bool log)
		{
			ClickButton ("OK", log);
		}

		// Click "Cancel" button of the window.
		public void Cancel ()
		{
			ClickButton ("Cancel", true);
		}

		public void Cancel (bool log)
		{
			ClickButton ("Cancel", log);
		}

		// Click "Save" button of the window.
		public void Save ()
		{
			ClickButton ("Save", true);
		}

		public void Save (bool log)
		{
			ClickButton ("Save", log);
		}

		// Click "Yes" button of the window
		public void Yes ()
		{
			ClickButton ("Yes", true);
		}

		public void Yes (bool log)
		{
			ClickButton ("Yes", log);
		}

		// Click "No" button of the window
		public void No ()
		{
			ClickButton ("No", true);
		}

		public void No (bool log)
		{
			ClickButton ("No", log);
		}

		// Click button by its name.
		private void ClickButton (string name, bool log)
		{
			try {
				Button button = Find<Button> (name);
				button.Click (log);
			} catch (NullReferenceException e) {
				Console.WriteLine (e);
			}
		}
	}
}