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
	internal class TextBoxProvider : FragmentRootControlProvider, IScrollBehaviorSubject
	{
		#region Constructors

		public TextBoxProvider (TextBoxBase textBoxBase) : base (textBoxBase)
		{
			this.textboxbase = textBoxBase;
		}

		#endregion

		#region IScrollBehaviorSubject implementation
		
		public IScrollBehaviorObserver ScrollBehaviorObserver {
			get { return observer; }
		}

		public FragmentControlProvider GetScrollbarProvider (ScrollBar scrollbar)
		{
			return new TextBoxScrollBarProvider (scrollbar, textboxbase);
		}

		#endregion

		#region Public Methods

		public override void Initialize ()
		{
			base.Initialize ();
			
			//Text pattern is supported by both Control Types: Document and Edit
			SetBehavior (TextPatternIdentifiers.Pattern,
			             new TextProviderBehavior (this));

			ScrollBar vscrollbar 
				= Helper.GetPrivateProperty<TextBoxBase, ScrollBar> (
				typeof (TextBoxBase), textboxbase, "UIAVScrollBar");
			ScrollBar hscrollbar 
				= Helper.GetPrivateProperty<TextBoxBase, ScrollBar> (
				typeof (TextBoxBase), textboxbase, "UIAHScrollBar");

			observer = new ScrollBehaviorObserver (this, hscrollbar, vscrollbar);
			observer.ScrollPatternSupportChanged += OnScrollPatternSupportChanged;
			UpdateScrollBehavior ();
			
			textboxbase.MultilineChanged += new EventHandler (OnMultilineChanged);
			UpdateBehaviors ();
		}

		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
				return textboxbase.Multiline == true ? ControlType.Document.Id : ControlType.Edit.Id;
			else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
				return textboxbase.Multiline == true ? "document" : "edit";
			else 
				return base.GetProviderPropertyValue (propertyId);
		}
		
		public override void Terminate ()
		{
			base.Terminate (); 
			
			textboxbase.MultilineChanged -= new EventHandler (OnMultilineChanged);
			observer.ScrollPatternSupportChanged -= OnScrollPatternSupportChanged;
		}

		public override void InitializeChildControlStructure ()
		{
			base.InitializeChildControlStructure ();
			
			observer.Initialize ();
		}
		
		public override void FinalizeChildControlStructure ()
		{
			base.FinalizeChildControlStructure ();
			
			observer.Terminate ();
		}

		#endregion

		#region Protected Methods

		protected void UpdateScrollBehavior ()
		{
			if (observer.SupportsScrollPattern
			    && textboxbase.Multiline) {
				SetBehavior (ScrollPatternIdentifiers.Pattern,
				             new ScrollProviderBehavior (this));
			} else {
				SetBehavior (ScrollPatternIdentifiers.Pattern, null);
			}
		}
		
		protected void UpdateBehaviors ()
		{
			//Here we are changing from Edit to Document and vice versa.
			if (textboxbase.Multiline) { //Document Control Type
				// NOTDOTNET: Document control type has no way
				// to find out of the text has changed, so we
				// implement ValuePattern always.
				//SetBehavior (ValuePatternIdentifiers.Pattern,
				//             null);
				SetBehavior (ValuePatternIdentifiers.Pattern,
				             new ValueProviderBehavior (this));
				SetBehavior (RangeValuePatternIdentifiers.Pattern,
				             null);
			} else { //Edit Control Type
				SetBehavior (ValuePatternIdentifiers.Pattern,
				             new ValueProviderBehavior (this));
				SetBehavior (RangeValuePatternIdentifiers.Pattern,
				             null);
			}

			UpdateScrollBehavior ();
		}

		#endregion
		
		#region Private Methods

		private void OnScrollPatternSupportChanged (object o, EventArgs args)
		{
			UpdateScrollBehavior ();
		}
		
		private void OnMultilineChanged (object sender, EventArgs args)
		{
			UpdateBehaviors ();
		}
		
		#endregion

		#region Protected section
		
		protected TextBoxBase textboxbase;
		protected ScrollBehaviorObserver observer;

		#endregion
		
		#region Internal Class: ScrollBar provider

		internal class TextBoxScrollBarProvider : ScrollBarProvider
		{
			public TextBoxScrollBarProvider (ScrollBar scrollbar,
			                                 TextBoxBase textbox)
				: base (scrollbar)
			{
				this.textbox = textbox;
				//TODO: i18n?
				name = scrollbar is HScrollBar ? "Horizontal Scroll Bar"
					: "Vertical Scroll Bar";
			}
			
			public override IRawElementProviderFragmentRoot FragmentRoot {
				get {
					return (IRawElementProviderFragmentRoot) ProviderFactory.FindProvider (textbox);
				}
			}			
			
			protected override object GetProviderPropertyValue (int propertyId)
			{
				if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
					return name;
				else
					return base.GetProviderPropertyValue (propertyId);
			}
			
			private TextBoxBase textbox;
			private string name;
		}
		
		#endregion
	}

}
