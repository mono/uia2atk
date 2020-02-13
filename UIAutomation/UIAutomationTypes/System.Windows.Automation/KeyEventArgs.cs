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
// Copyright (c) 2004-2005 Novell, Inc.
//
// Authors:
//	Mike GOrse    mgorse@novell.com
//
//


// COMPLETE

using System.Runtime.InteropServices;

namespace System.Windows.Automation {
	[ComVisible(true)]
	internal class KeyEventArgs : AutomationEventArgs {
		private bool down;
		private int keycode;
		private int keysym;
		private String str;
		private bool alt;
		private bool shift;
		private bool control;
		private bool supress_key_press;

#region Public Constructors
		public KeyEventArgs (bool down, int keycode, int keysym, String str, bool alt, bool control, bool shift): base (AutomationElementIdentifiers.KeyEvent) {
			this.down = down;
			this.keycode = keycode;
			this.keysym = keysym;
			this.str = str;
			this.alt = alt;
			this.shift = shift;
			this.control = control;
		}
#endregion	// Public Constructors

#region Public Instance Properties
		public  bool Down {
			get {
				return down;
			}
		}

		public bool Alt {
			get {
				return alt;
			}
		}

		public int Keycode {
			get {
				return keycode;
			}
		}

		public int Keysym {
			get {
				return keysym;
			}
		}

		public String Str {
			get {
				return str;
			}
		}

		public bool Shift {
			get {
				return shift;
			}
		}

		public bool Control {
			get {
				return control;
			}
		}

		public int KeyCode {
			get {
				return keycode;
			}
		}

		public bool SuppressKeyPress {
			get {
				return supress_key_press;
			}
			set {
				supress_key_press = value;
			}
		}
#endregion	// Public Instance Properties
	}
}
