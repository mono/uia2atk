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
//	Brad Taylor <brad@getcoded.net>
//

using System;
using System.Windows;
using System.Windows.Automation.Peers;

namespace Moonlight.AtkBridge
{
	[AttributeUsage (AttributeTargets.Class,
	                 AllowMultiple=true, Inherited=false)]
	internal class ImplementsPatternAttribute : Attribute
	{
		public PatternInterface Pattern {
			get;
			set;
		}

		public string ElementType {
			get;
			set;
		}

		public AutomationControlType? ControlType {
			get;
			set;
		}

		public PatternInterface Provides {
			get { return provides; }
			set {
				provides = value;
				IsProvidesSet = true;
			}
		}

		public bool IsProvidesSet {
			get;
			private set;
		}

		public ImplementsPatternAttribute (PatternInterface pattern)
		{
			Pattern = pattern;
		}

		public ImplementsPatternAttribute (string elementType)
		{
			ElementType = elementType;
		}

		public ImplementsPatternAttribute (AutomationControlType controlType)
		{
			ControlType = controlType;
		}

		// Hack around a C# limitation which does not allow for
		// nullable types in named attributes
		private PatternInterface provides;
	}
}
