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
//      Mike Gorse <mgorse@novell.com>
//

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation;
using Mono.Unix;
using Mono.UIAutomation.Services;
using Mono.UIAutomation.Source;
using Atspi;

namespace AtspiUiaSource
{
	public class ValueSource : IValuePattern
	{
		private Accessible accessible;
		private Text text;
		private EditableText editableText;

		public ValueSource (Element element)
		{
			accessible = element.Accessible;
			text = accessible.QueryText ();
			editableText = accessible.QueryEditableText ();
		}

		public ValueProperties Properties {
			get {
				ValueProperties properties = new ValueProperties ();
				properties.IsReadOnly = !(accessible.StateSet.Contains (StateType.Editable));
				properties.Value = text.GetText ();
				return properties;
			}
		}

		public void SetValue (string value)
		{
			if (!accessible.StateSet.Contains (StateType.Enabled))
				throw new ElementNotEnabledException ();
			if (!accessible.StateSet.Contains (StateType.Editable))
				throw new InvalidOperationException ();
			if (!editableText.SetTextContents (value))
				Log.Warn ("AtspiUiaSource: SetTextContents failed");
				return;
		}
	}
}
