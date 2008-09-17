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
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Forms;
using Mono.UIAutomation.Winforms.Behaviors;
using Mono.UIAutomation.Winforms.Behaviors.TextBox;

namespace Mono.UIAutomation.Winforms
{

	internal class TextBoxProvider : FragmentControlProvider
	{
	
		#region Constructors

		public TextBoxProvider (TextBoxBase textBoxBase) : base (textBoxBase)
		{
			this.textboxbase = textBoxBase;
			
			//Text pattern is supported by both Control Types: Document and Edit
			SetBehavior (TextPatternIdentifiers.Pattern,
			             new TextProviderBehavior (this));
			
			textboxbase.MultilineChanged += new EventHandler (OnMultilineChanged);
			UpdateBehaviors ();
		}

		#endregion

		#region Public Methods

		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
				return textboxbase.Multiline == true ? ControlType.Document.Id : ControlType.Edit.Id;
			else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
				return textboxbase.Multiline == true ? "document" : "edit";
			else 
				return base.GetPropertyValue (propertyId);
		}
		
		public override void Terminate ()
		{
			base.Terminate (); 
			
			textboxbase.MultilineChanged -= new EventHandler (OnMultilineChanged);
		}

		#endregion
		
		#region Private Methods
		
		private void OnMultilineChanged (object sender, EventArgs args)
		{
			UpdateBehaviors ();
		}
		
		private void UpdateBehaviors ()
		{
			//Here we are changing from Edit to Document and vice versa.
			if (textboxbase.Multiline == true) { //Document Control Type
				SetBehavior (ScrollPatternIdentifiers.Pattern,
				             new ScrollProviderBehavior (this));
				SetBehavior (ValuePatternIdentifiers.Pattern,
				             null);
				SetBehavior (RangeValuePatternIdentifiers.Pattern,
				             null);
			} else { //Edit Control Type
				SetBehavior (ScrollPatternIdentifiers.Pattern,
				             null);
				SetBehavior (ValuePatternIdentifiers.Pattern,
				             new ValueProviderBehavior (this));
				SetBehavior (RangeValuePatternIdentifiers.Pattern,
				             null);
			}
		}
		
		#endregion

		#region Protected section
		
		protected TextBoxBase textboxbase;

		#endregion
		
	}

}
