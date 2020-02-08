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
// Copyright (c) 2010 Novell, Inc. (http://www.novell.com)
//
// Authors:
//	Mario Carrion <mcarrion@novell.com>
//

using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

namespace Mono.UIAutomation.Winforms
{
	internal class SimpleProviderWrapper : FragmentControlProvider
	{
		public SimpleProviderWrapper (Component component, IRawElementProviderSimple simpleProvider)
			: base (component)
		{
			wrappedProvider = simpleProvider;
		}

		public override object GetPatternProvider (int patternId)
		{
			object pattern = wrappedProvider.GetPatternProvider (patternId);
			if (pattern != null)
				return pattern;
			return base.GetPatternProvider (patternId);
		}

		// FIXME: Hard to tell when wrappedProvider *really* wants to return null
		public override object GetPropertyValue (int propertyId)
		{
			object value = wrappedProvider.GetPropertyValue (propertyId);
			if (value != null)
				return value;
			return base.GetPropertyValue (propertyId);
		}

		public override ProviderOptions ProviderOptions {
			get { return wrappedProvider.ProviderOptions; }
		}

		public override IRawElementProviderSimple HostRawElementProvider {
			get { return wrappedProvider.HostRawElementProvider; }
		}

		protected override bool IsBehaviorEnabled (AutomationPattern pattern)
		{
			object behavior = GetPatternProvider (pattern.Id);
			if (behavior != null)
				return true;
			return base.IsBehaviorEnabled (pattern);
		}

		protected IRawElementProviderSimple wrappedProvider;

		public override string ToString ()
		{
			var wrapperClassName = this.GetType ().ToString ().Split ('.').Last ();
			return $"<{wrapperClassName}{{{wrappedProvider}}},{Component}:{runtimeId}>";
		}
	}
}

