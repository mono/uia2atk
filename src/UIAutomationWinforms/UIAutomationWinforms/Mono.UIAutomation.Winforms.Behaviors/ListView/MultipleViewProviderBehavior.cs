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
using SWF = System.Windows.Forms;
using Mono.UIAutomation.Winforms;

namespace Mono.UIAutomation.Winforms.Behaviors.ListView
{

	internal class MultipleViewProviderBehavior 
		: ProviderBehavior, IMultipleViewProvider
	{
		
		#region Constructors

		public MultipleViewProviderBehavior (ListViewProvider provider)
			: base (provider)
		{
		}

		#endregion
		
		#region IProviderBehavior Interface

		public override AutomationPattern ProviderPattern { 
			get { return MultipleViewPatternIdentifiers.Pattern; }
		}
		
		public override void Connect (SWF.Control control)
		{
			//NOTE: CurrentViewProperty NEVER changes
			//NOTE: SupportedViewsProperty NEVER changes
		}
		
		public override void Disconnect (SWF.Control control)
		{	
			//NOTE: CurrentViewProperty NEVER changes
			//NOTE: SupportedViewsProperty NEVER changes
		}

		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == MultipleViewPatternIdentifiers.CurrentViewProperty.Id)
				return CurrentView;
			else if (propertyId == MultipleViewPatternIdentifiers.SupportedViewsProperty.Id)
				return GetSupportedViews ();
			else
				return base.GetPropertyValue (propertyId);
		}
		
		#endregion

		#region IMultipleViewProvider implementation 
		
		public int[] GetSupportedViews ()
		{
			return new int [] { 0 }; //NOTE: Value returned from Tests
		}
		
		public string GetViewName (int viewId)
		{
			if (Array.IndexOf (GetSupportedViews (), viewId) == -1)
				throw new ArgumentException ();
			
			return "Icons"; //NOTE: Value returned from Tests
		}
		
		public void SetCurrentView (int viewId)
		{
			if (Array.IndexOf (GetSupportedViews (), viewId) == -1)
				throw new ArgumentException ();
			
			//Doesn't do anything, because the value to set is the only 
			//value available (according to tests in Vista)
		}
		
		public int CurrentView {
			get { return 0; } //NOTE: Value returned from Tests
		}
		
		#endregion 

	}
				                      
}
