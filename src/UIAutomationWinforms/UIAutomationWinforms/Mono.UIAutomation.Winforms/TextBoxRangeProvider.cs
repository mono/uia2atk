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
//	Mario Carrion <mcarrion@novell.com>
// 

using System;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Automation.Text;

namespace Mono.UIAutomation.Winforms
{

	public class TextBoxRangeProvider : ITextRangeProvider
	{
		
#region Constructor
		
		public TextBoxRangeProvider()
		{
		}
		
#endregion

		public void AddToSelection ()
		{
			throw new NotImplementedException();
		}

		public ITextRangeProvider Clone ()
		{
			throw new NotImplementedException();
		}

		public bool Compare (ITextRangeProvider range)
		{
			throw new NotImplementedException();
		}

		public int CompareEndpoints (TextPatternRangeEndpoint endpoint, 
		                             ITextRangeProvider targetRange, 
		                             TextPatternRangeEndpoint targetEndpoint)
		{
			throw new NotImplementedException();
		}

		public void ExpandToEnclosingUnit (TextUnit unit)
		{
			throw new NotImplementedException();
		}

		public ITextRangeProvider FindAttribute (int attribute, object value, 
		                                         bool backward)
		{
			throw new NotImplementedException();
		}

		public ITextRangeProvider FindText (string text, bool backward, 
		                                    bool ignoreCase)
		{
			throw new NotImplementedException();
		}

		public object GetAttributeValue (int attribute)
		{
			throw new NotImplementedException();
		}

		public double[] GetBoundingRectangles ()
		{
			throw new NotImplementedException();
		}

		public IRawElementProviderSimple[] GetChildren ()
		{
			throw new NotImplementedException();
		}

		public IRawElementProviderSimple GetEnclosingElement ()
		{
			throw new NotImplementedException();
		}

		public string GetText (int maxLength)
		{
			throw new NotImplementedException();
		}

		public int Move (TextUnit unit, int count)
		{
			throw new NotImplementedException();
		}

		public void MoveEndpointByRange (TextPatternRangeEndpoint endpoint, 
		                                 ITextRangeProvider targetRange, 
		                                 TextPatternRangeEndpoint targetEndpoint)
		{
			throw new NotImplementedException();
		}

		public int MoveEndpointByUnit (TextPatternRangeEndpoint endpoint, 
		                               TextUnit unit, int count)
		{
			throw new NotImplementedException();
		}

		public void RemoveFromSelection ()
		{
			throw new NotImplementedException();
		}

		public void ScrollIntoView (bool alignToTop)
		{
			throw new NotImplementedException();
		}

		public void Select ()
		{
			throw new NotImplementedException();
		
		}

	}
}
