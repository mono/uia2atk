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
using Mono.UIAutomation.Source;
using Atspi;

namespace AtspiUiaSource
{
	public class RangeValueSource : IRangeValuePattern
	{
		private Accessible accessible;
		private Value Value {
			get {
				Value val = accessible.QueryValue ();
				if (val == null)
					throw new NotSupportedException ();
				return val;
			}
		}

		public RangeValueSource (Element element)
		{
			accessible = element.Accessible;
		}

		public RangeValueProperties Properties {
			get {
				RangeValueProperties properties = new RangeValueProperties ();
				Value atspiValue = Value;
				properties.Value = atspiValue.CurrentValue;
				properties.IsReadOnly = !accessible.StateSet.Contains (StateType.Editable);
				properties.Maximum = atspiValue.MaximumValue;
				properties.Minimum = atspiValue.MinimumValue;
				properties.LargeChange = atspiValue.MinimumIncrement;
				properties.SmallChange = atspiValue.MinimumIncrement;
				return properties;
			}
		}

		public void SetValue (double value)
		{
			if (!accessible.StateSet.Contains (StateType.Enabled))
				throw new ElementNotEnabledException ();
			if ((accessible.Role == Role.Text ||
					accessible.Role == Role.SpinButton) &&
				!accessible.StateSet.Contains (StateType.Editable))
				throw new InvalidOperationException ();
			Value atspiValue = this.Value;
			if (value < atspiValue.MinimumValue ||
				value > atspiValue.MaximumValue)
				throw new ArgumentOutOfRangeException ();
			atspiValue.CurrentValue = value;
		}
	}
}
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
using Mono.UIAutomation.Source;
using Atspi;

namespace AtspiUiaSource
{
	public class RangeValueSource : IRangeValuePattern
	{
		private Accessible accessible;
		private Value Value {
			get {
				Value val = accessible.QueryValue ();
				if (val == null)
					throw new NotSupportedException ();
				return val;
			}
		}

		public RangeValueSource (Element element)
		{
			accessible = element.Accessible;
		}

		public RangeValueProperties Properties {
			get {
				RangeValueProperties properties = new RangeValueProperties ();
				Value atspiValue = Value;
				properties.Value = atspiValue.CurrentValue;
				properties.IsReadOnly = !accessible.StateSet.Contains (StateType.Editable);
				properties.Maximum = atspiValue.MaximumValue;
				properties.Minimum = atspiValue.MinimumValue;
				properties.LargeChange = atspiValue.MinimumIncrement;
				properties.SmallChange = atspiValue.MinimumIncrement;
				return properties;
			}
		}

		public void SetValue (double value)
		{
			if (!accessible.StateSet.Contains (StateType.Enabled))
				throw new ElementNotEnabledException ();
			if ((accessible.Role == Role.Text ||
					accessible.Role == Role.SpinButton) &&
				!accessible.StateSet.Contains (StateType.Editable))
				throw new InvalidOperationException ();
			Value atspiValue = this.Value;
			if (value < atspiValue.MinimumValue ||
				value > atspiValue.MaximumValue)
				throw new ArgumentOutOfRangeException ();
			atspiValue.CurrentValue = value;
		}
	}
}
