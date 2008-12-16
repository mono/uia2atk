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
//	Sandy Armstrong <sanfordarmstrong@gmail.com>
// 

using System;
using SWF = System.Windows.Forms;

using System.Windows.Automation;
using System.Windows.Automation.Provider;

using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Events.TreeView;

namespace Mono.UIAutomation.Winforms.Behaviors.TreeView
{
	internal class ValueProviderBehavior : ProviderBehavior, IValueProvider
	{
		#region Private Members
		
		private TreeNodeProvider nodeProvider;

		#endregion

		#region Constructor
		
		public ValueProviderBehavior (TreeNodeProvider nodeProvider) :
			base (nodeProvider)
		{
			this.nodeProvider = nodeProvider;
		}

		#endregion

		#region ProviderBehavior Overrides

		public override AutomationPattern ProviderPattern {
			get {
				return ValuePatternIdentifiers.Pattern;
			}
		}

		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == ValuePatternIdentifiers.ValueProperty.Id)
				return Value;
			else if (propertyId == ValuePatternIdentifiers.IsReadOnlyProperty.Id)
				return IsReadOnly;
			return base.GetPropertyValue(propertyId);
		}

		public override void Disconnect ()
		{
			nodeProvider.SetEvent (ProviderEventType.ValuePatternValueProperty,
			                       null);
		}

		public override void Connect ()
		{
			nodeProvider.SetEvent (ProviderEventType.ValuePatternValueProperty,
			                       new ValuePatternValueEvent (nodeProvider));
		}

		#endregion
		
		#region IValueProvider implementation
		
		public void SetValue (string val)
		{
			SWF.TreeView treeView = nodeProvider.TreeNode.TreeView;
			if (!treeView.Enabled)
				throw new ElementNotEnabledException ();
			if (treeView.InvokeRequired) {
				treeView.BeginInvoke (new SetValueDelegate (PerformSetValue),
				                      new object [] {val});
				return;
			}
			PerformSetValue (val);
		}
		
		public bool IsReadOnly {
			get {
				return false;
			}
		}
		
		public string Value {
			get {
				return nodeProvider.TreeNode.Text;
			}
		}
		
		#endregion

		#region Private Methods

		private void PerformSetValue (string val)
		{
			nodeProvider.TreeNode.Text = val;
		}

		private delegate void SetValueDelegate (string val);

		#endregion
	}
}
