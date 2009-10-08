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
			internal TransformPatternInformation (TransformProperties properties)
			{
				CanMove = properties.CanMove;
				CanResize = properties.CanResize;
				CanRotate = properties.CanRotate;
			}

			public bool CanMove {
				get; private set;
			}

			public bool CanResize {
				get; private set;
			}

			public bool CanRotate {
				get; private set;
			}
		}

		private ITransformPattern source;

		internal TransformPattern (ITransformPattern source)
		{
			this.source = source;
		}

		public TransformPatternInformation Cached {
			get {
				throw new NotImplementedException ();
			}
		}

		public TransformPatternInformation Current {
			get {
				return new TransformPatternInformation (source.Properties);
			}
		}

		public void Move (double x, double y)
		{
			source.Move (x, y);
		}

		public void Resize (double width, double height)
		{
			source.Resize (width);
		}

		public void Rotate (double degrees)
		{
			source.Rotate (degrees);
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
