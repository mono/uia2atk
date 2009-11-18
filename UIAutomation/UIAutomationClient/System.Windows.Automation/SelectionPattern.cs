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
	public class SelectionPattern : BasePattern
	{
		public struct SelectionPatternInformation
		{
			private bool cached;
			private ISelectionPattern source;
			private bool canSelectMultiple;
			private bool isSelectionRequired;
			private AutomationElement [] selection;

			internal SelectionPatternInformation (SelectionProperties properties)
			{
				cached = true;
				canSelectMultiple = properties.CanSelectMultiple;
				isSelectionRequired = properties.IsSelectionRequired;
				selection = SourceManager.GetOrCreateAutomationElements (properties.Selection);
				source = null;
			}

			internal SelectionPatternInformation (ISelectionPattern source)
			{
				this.cached = false;
				this.source = source;
				// Below fields are unused for Current
				canSelectMultiple = false;
				isSelectionRequired = false;
				selection = null;
			}

			public bool CanSelectMultiple {
				get {
					return (cached ?
						canSelectMultiple :
						source.CanSelectMultiple);
				}
			}

			public bool IsSelectionRequired {
				get {
					return (cached ?
						isSelectionRequired :
						source.IsSelectionRequired);
				}
			}

			public AutomationElement [] GetSelection ()
			{
				return (cached ?
					selection :
					SourceManager.GetOrCreateAutomationElements (source.Selection));
			}
		}

		private ISelectionPattern source;

		internal SelectionPattern (ISelectionPattern source)
		{
			this.source = source;
		}

		public SelectionPatternInformation Cached {
			get {
				throw new NotImplementedException ();
			}
		}

		public SelectionPatternInformation Current {
			get {
				return new SelectionPatternInformation (source);
			}
		}

		public static readonly AutomationPattern Pattern =
			SelectionPatternIdentifiers.Pattern;

		public static readonly AutomationProperty SelectionProperty =
			SelectionPatternIdentifiers.SelectionProperty;

		public static readonly AutomationProperty CanSelectMultipleProperty =
			SelectionPatternIdentifiers.CanSelectMultipleProperty;

		public static readonly AutomationProperty IsSelectionRequiredProperty =
			SelectionPatternIdentifiers.IsSelectionRequiredProperty;

		public static readonly AutomationEvent InvalidatedEvent =
			SelectionPatternIdentifiers.InvalidatedEvent;
	}
}
