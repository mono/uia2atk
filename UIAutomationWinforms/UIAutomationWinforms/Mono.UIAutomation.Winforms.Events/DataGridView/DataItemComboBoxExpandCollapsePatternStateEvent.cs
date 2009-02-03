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
//	Mario Carrion <mcarrion@novell.com>
// 
using System;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using SWF = System.Windows.Forms;
using Mono.UIAutomation.Winforms;
using Mono.UIAutomation.Winforms.Events;

namespace Mono.UIAutomation.Winforms.Events.DataGridView
{
	
	internal class DataItemComboBoxExpandCollapsePatternStateEvent
		: BaseAutomationPropertyEvent
	{
		
		#region Constructor

		public DataItemComboBoxExpandCollapsePatternStateEvent (DataGridViewProvider.DataGridViewDataItemComboBoxProvider provider) 
			: base (provider, 
			        ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty)
		{
		}
		
		#endregion

		#region Public Properties

		public SWF.ComboBox CurrentComboBox {
			set {
				if (value == currentComboBox)
					return;

				Disconnect ();
				currentComboBox = value;
				Connect ();
			}
		}

		#endregion
		
		#region Overridden Methods
		
		public override void Connect ()
		{
			if (currentComboBox != null) {
				currentComboBox.DropDown += OnDropDown;
				currentComboBox.DropDownClosed += OnDropDownClosed;
				currentComboBox.Disposed += OnDisposed;
			}
		}

		public override void Disconnect ()
		{
			if (currentComboBox != null) {
				currentComboBox.DropDown -= OnDropDown;
				currentComboBox.DropDownClosed -= OnDropDownClosed;
				currentComboBox.Disposed -= OnDisposed;
			}
		}
		
		#endregion


		#region Protected Methods

		protected override object GetNewPropertyValue ()
		{
			return expandCollapseState;
		}

		#endregion

		#region Private Methods
		
		private void OnDropDown (object sender, EventArgs args)
		{
			expandCollapseState = ExpandCollapseState.Expanded;
			RaiseAutomationPropertyChangedEvent ();
		}

		private void OnDropDownClosed (object sender, EventArgs args)
		{
			expandCollapseState = ExpandCollapseState.Collapsed;
			RaiseAutomationPropertyChangedEvent ();
		}

		private void OnDisposed (object sender, EventArgs args)
		{
			currentComboBox = null;
		}
		
		#endregion

		#region Private Fields

		private SWF.ComboBox currentComboBox;
		private ExpandCollapseState expandCollapseState;		

		#endregion
	}
}
