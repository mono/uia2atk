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
// Copyright (c) 2008 Novell, Inc. (http://www.novell.com) 
// 
// Authors: 
//      Brad Taylor <brad@getcoded.net>
// 

using System;
using System.Windows.Automation.Provider;

namespace System.Windows.Automation
{
	public sealed class AutomationElement
	{
#region Private Members
		private AutomationElementInformation current;
#endregion
		
#region Public Properties
		public AutomationElementInformation Cached {
			get { throw new NotImplementedException (); }
		}

		public AutomationElementCollection CachedChildren {
			get { throw new NotImplementedException (); }
		}

		public AutomationElement CachedParent {
			get { throw new NotImplementedException (); }
		}

		public AutomationElementInformation Current {
			get { return current; }
		}
#endregion

#region Public Static Properties
		public static AutomationElement FocusedElement {
			get { throw new NotImplementedException (); }
		}

		public static AutomationElement RootElement {
			get { throw new NotImplementedException (); }
		}
#endregion

#region Constructor
		internal AutomationElement ()
		{
			current = new AutomationElementInformation ();
		}
#endregion
		
#region Public Methods
		public override bool Equals (Object obj)
		{
			AutomationElement other = obj as AutomationElement;
			if (other == null)
				return false;
			return other.Current.AutomationId == Current.AutomationId; // TODO: Review
		}

		public AutomationElementCollection FindAll (TreeScope scope, Condition condition)
		{
			throw new NotImplementedException ();
		}

		public AutomationElement FindFirst (TreeScope scope, Condition condition)
		{
			throw new NotImplementedException ();
		}

		public Object GetCachedPattern (AutomationPattern pattern)
		{
			throw new NotImplementedException ();
		}

		public Object GetCachedPropertyValue (AutomationProperty property)
		{
			throw new NotImplementedException ();
		}

		public Object GetCachedPropertyValue (AutomationProperty property,
		                                      bool ignoreDefaultValue)
		{
			throw new NotImplementedException ();
		}

		public override int GetHashCode ()
		{
			throw new NotImplementedException ();
		}

		public Point GetClickablePoint ()
		{
			throw new NotImplementedException ();
		}

		public Object GetCurrentPattern (AutomationPattern pattern)
		{
			throw new NotImplementedException ();
		}

		public Object GetCurrentPropertyValue (AutomationProperty property)
		{
			throw new NotImplementedException ();
		}

		public Object GetCurrentPropertyValue (AutomationProperty property,
		                                       bool ignoreDefaultValue)
		{
			throw new NotImplementedException ();
		}

		public int[] GetRuntimeId ()
		{
			throw new NotImplementedException ();
		}

		public AutomationPattern[] GetSupportedPatterns ()
		{
			throw new NotImplementedException ();
		}

		public AutomationProperty[] GetSupportedProperties ()
		{
			throw new NotImplementedException ();
		}

		public AutomationElement GetUpdatedCache (CacheRequest request)
		{
			throw new NotImplementedException ();
		}

		public void SetFocus ()
		{
			throw new NotImplementedException ();
		}

		public bool TryGetCachedPattern (AutomationPattern pattern,
		                                 out Object patternObject)
		{
			throw new NotImplementedException ();
		}

		public bool TryGetClickablePoint (out Point pt)
		{
			throw new NotImplementedException ();
		}

		public bool TryGetCurrentPattern (AutomationPattern pattern,
		                                  out Object patternObject)
		{
			throw new NotImplementedException ();
		}
#endregion

#region Public Static Methods
		public static AutomationElement FromHandle (IntPtr hwnd)
		{
			throw new NotImplementedException ();
		}

		public static AutomationElement FromLocalProvider (IRawElementProviderSimple localImpl)
		{
			throw new NotImplementedException ();
		}

		public static AutomationElement FromPoint (Point pt)
		{
			throw new NotImplementedException ();
		}

		public static bool operator == (AutomationElement left,
		                                AutomationElement right)
		{
			throw new NotImplementedException ();
		}

		public static bool operator != (AutomationElement left,
		                                AutomationElement right)
		{
			throw new NotImplementedException ();
		}
#endregion

#region public structures
		public struct AutomationElementInformation
		{
			public string AcceleratorKey {
				get { throw new NotImplementedException (); }
			}

			public string AccessKey {
				get { throw new NotImplementedException (); }
			}

			public string AutomationId {
				get { throw new NotImplementedException (); }
			}

			public Rect BoundingRectangle {
				get { throw new NotImplementedException (); }
			}

			public string ClassName {
				get { throw new NotImplementedException (); }
			}

			public ControlType ControlType {
				get { throw new NotImplementedException (); }
			}

			public string FrameworkId {
				get { throw new NotImplementedException (); }
			}

			public bool HasKeyboardFocus {
				get { throw new NotImplementedException (); }
			}

			public string HelpText {
				get { throw new NotImplementedException (); }
			}

			public bool IsContentElement {
				get { throw new NotImplementedException (); }
			}

			public bool IsControlElement {
				get { throw new NotImplementedException (); }
			}

			public bool IsEnabled {
				get { throw new NotImplementedException (); }
			}

			public bool IsKeyboardFocusable {
				get { throw new NotImplementedException (); }
			}

			public bool IsOffscreen {
				get { throw new NotImplementedException (); }
			}

			public bool IsPassword {
				get { throw new NotImplementedException (); }
			}

			public bool IsRequiredForForm {
				get { throw new NotImplementedException (); }
			}

			public string ItemStatus {
				get { throw new NotImplementedException (); }
			}

			public string ItemType {
				get { throw new NotImplementedException (); }
			}

			public AutomationElement LabeledBy {
				get { throw new NotImplementedException (); }
			}

			public string LocalizedControlType {
				get { throw new NotImplementedException (); }
			}

			public string Name {
				get { throw new NotImplementedException (); }
			}

			public int NativeWindowHandle {
				get { throw new NotImplementedException (); }
			}

			public OrientationType Orientation {
				get { throw new NotImplementedException (); }
			}

			public int ProcessId {
				get { throw new NotImplementedException (); }
			}
		}
#endregion

#region Public Static ReadOnly Fields

		public static readonly AutomationProperty IsScrollPatternAvailableProperty;

		public static readonly AutomationProperty ControlTypeProperty;

		public static readonly AutomationProperty FrameworkIdProperty;

		public static readonly AutomationProperty NativeWindowHandleProperty;

#endregion
	}
}
