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
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using SWF = System.Windows.Forms;
using Mono.UIAutomation.Bridge;
using Mono.UIAutomation.Winforms;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Events.ListView;

namespace Mono.UIAutomation.Winforms.Behaviors.ListView
{
	internal class ListItemValueProviderBehavior
		: ProviderBehavior, IValueProvider, IClipboardSupport
	{
		#region Constructors
		
		public ListItemValueProviderBehavior (ListItemProvider itemProvider)
			: base (itemProvider)
		{
			viewItem = (SWF.ListViewItem) itemProvider.ObjectItem;
		}
		
		#endregion
		
		#region ProviderBehavior Specialization
		
		public override AutomationPattern ProviderPattern {
			get { return ValuePatternIdentifiers.Pattern; }
		}
		
		public override void Connect ()
		{
			Provider.SetEvent (ProviderEventType.ValuePatternIsReadOnlyProperty,
			                   new ListItemValuePatternIsReadOnlyEvent ((ListItemProvider) Provider));
			Provider.SetEvent (ProviderEventType.ValuePatternValueProperty,
			                   new ListItemValuePatternValueEvent ((ListItemProvider) Provider));
		}
		
		public override void Disconnect ()
		{
			Provider.SetEvent (ProviderEventType.ValuePatternIsReadOnlyProperty,
			                   null);
			Provider.SetEvent (ProviderEventType.ValuePatternValueProperty,
			                   null);
		}

		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == ValuePatternIdentifiers.IsReadOnlyProperty.Id)
				return IsReadOnly;
			else if (propertyId == ValuePatternIdentifiers.ValueProperty.Id)
				return Value;
			else
				return base.GetPropertyValue (propertyId);
		}

		#endregion

		#region IValueProvider implementation 
		
		public void SetValue (string value)
		{
			if (IsReadOnly == true)
				throw new ElementNotEnabledException ();

			PerformSetValue (value);
		}
		
		public bool IsReadOnly {
			get { return viewItem.ListView.LabelEdit == false; }
		}
		
		public string Value {
			get { return viewItem.Text; }
		}
		
		#endregion

		#region IClipboardSupport Implementation	

		public void Copy (int start, int end)
		{
			string text = Value;
			start = (int) System.Math.Max (start, 0);
			end = (int) System.Math.Min (end, text.Length);
			SWF.Clipboard.SetText (text.Substring (start, end - start));
		}
		
		public void Paste (int position)
		{
			string text = Value;
			position = (int) System.Math.Max (position, 0);
			position = (int) System.Math.Min (position, text.Length);

			string newValue = Value.Insert (position, SWF.Clipboard.GetText ());
			PerformSetValue (newValue);
		}

		#endregion

		#region Private Methods

		private void PerformSetValue (string value)
		{
			if (viewItem.ListView.InvokeRequired == true) {
				viewItem.ListView.BeginInvoke (new ListItemSetValueDelegate (PerformSetValue),
				                               new object [] { value } );
				return;
			}
			
			viewItem.Text = value;
		}

		#endregion
		
		#region Private Fields
		
		private SWF.ListViewItem viewItem;
		
		#endregion
	}

	delegate void ListItemSetValueDelegate (string value);
}
