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
using Mono.UIAutomation.Bridge;
using Mono.UIAutomation.Winforms;
using Mono.UIAutomation.Winforms.Events;

namespace Mono.UIAutomation.Winforms.Behaviors.NumericUpDown
{
	
	internal class EditableRangeProviderBehavior : ProviderBehavior, IEditableRangeProvider
	{
		
		#region Constructor
		
		public EditableRangeProviderBehavior (NumericUpDownProvider provider)
			: base (provider)
		{
			numericUpDown = (SWF.NumericUpDown) provider.Control;
		}
		
		#endregion

		#region IProviderBehavior Interface
		
		public override AutomationPattern ProviderPattern { 
			get { return EditableRangePatternIdentifiers.Pattern; }
		}
		
		public override void Connect ()
		{
		}
		
		public override void Disconnect ()
		{
		}

		#endregion

		#region IEditableRange Members
		public void BeginEdit (string text)
		{
			if (numericUpDown.ReadOnly)
				throw new ElementNotEnabledException ();
			if (numericUpDown.InvokeRequired == true) {
				numericUpDown.BeginInvoke (new NumericUpDownBeginEditDelegate (BeginEdit),
				                           new object [] { text });
				return;
			}
			numericUpDown.txtView.Text = text;
		}

		public void CommitEdit ()
		{
			if (numericUpDown.InvokeRequired) {
				numericUpDown.BeginInvoke (new NumericUpDownCommitEditDelegate (CommitEdit));
				return;
			}
			decimal value = decimal.Parse (numericUpDown.Text);
			if (value < numericUpDown.Minimum)
				value = numericUpDown.Minimum;
			if (value > numericUpDown.Maximum)
				value = numericUpDown.Maximum;
			numericUpDown.Value = value;
		}
		#endregion

		#region Private Fields
		
		private SWF.NumericUpDown numericUpDown;
		
		#endregion
	}

	delegate void NumericUpDownBeginEditDelegate (string value);
	delegate void NumericUpDownCommitEditDelegate ();		
}
