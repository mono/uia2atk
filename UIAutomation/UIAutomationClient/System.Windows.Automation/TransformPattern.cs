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
//  Sandy Armstrong <sanfordarmstrong@gmail.com>
//  Mike Gorse <mgorse@novell.com>
// 

using System;
using Mono.UIAutomation.Source;

namespace System.Windows.Automation
{
	public class TransformPattern : BasePattern
	{
		public struct TransformPatternInformation
		{
			private bool cache;
			private TransformPattern pattern;

			internal TransformPatternInformation (TransformPattern pattern, bool cache)
			{
				this.pattern = pattern;
				this.cache = cache;
			}

			public bool CanMove {
				get { return (bool) pattern.element.GetPropertyValue (CanMoveProperty, cache); }
			}

			public bool CanResize {
				get { return (bool) pattern.element.GetPropertyValue (CanResizeProperty, cache); }
			}

			public bool CanRotate {
				get { return (bool) pattern.element.GetPropertyValue (CanRotateProperty, cache); }
			}
		}

		private AutomationElement element;
		private bool cached;
		private TransformPatternInformation currentInfo;
		private TransformPatternInformation cachedInfo;

		internal TransformPattern ()
		{
		}

		internal TransformPattern (ITransformPattern source, AutomationElement element, bool cached)
		{
			this.element = element;
			this.cached = cached;
			this.Source = source;
			currentInfo = new TransformPatternInformation (this, false);
			if (cached)
				cachedInfo = new TransformPatternInformation (this, true);
		}

		internal ITransformPattern Source { get; private set; }

		public TransformPatternInformation Cached {
			get {
				if (!cached)
					throw new InvalidOperationException ("Cannot request a property or pattern that is not cached");
				return cachedInfo;
			}
		}

		public TransformPatternInformation Current {
			get {
				return currentInfo;
			}
		}

		public void Move (double x, double y)
		{
			Source.Move (x, y);
		}

		public void Resize (double width, double height)
		{
			Source.Resize (width, height);
		}

		public void Rotate (double degrees)
		{
			Source.Rotate (degrees);
		}

		public static readonly AutomationPattern Pattern =
			TransformPatternIdentifiers.Pattern;

		public static readonly AutomationProperty CanMoveProperty =
			TransformPatternIdentifiers.CanMoveProperty;

		public static readonly AutomationProperty CanResizeProperty =
			TransformPatternIdentifiers.CanResizeProperty;

		public static readonly AutomationProperty CanRotateProperty =
			TransformPatternIdentifiers.CanRotateProperty;
	}
}
