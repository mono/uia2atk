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
	// Note: The difference between our implemenation and Windows 7: on Win 7 the MultipleView
	// pattern is not correctly implemented on ListView, while the pattern is perfectly implemented
	// in the native Windows Explorer (and FileDialog), so considering both correctness
	// and consistency, we choose to implement MultipleView Pattern on ListView in "perfect"
	// way. see reviewboard #730 for more discussions.
	internal class MultipleViewProviderBehavior 
		: ProviderBehavior, IMultipleViewProvider
	{
		
		#region Constructors

		public MultipleViewProviderBehavior (ListViewProvider provider)
			: base (provider)
		{
			listViewProvider = provider;
		}

		#endregion
		
		#region IProviderBehavior Interface

		public override AutomationPattern ProviderPattern { 
			get { return MultipleViewPatternIdentifiers.Pattern; }
		}
		
		public override void Connect ()
		{
			//NOTE: CurrentViewProperty NEVER changes
			//NOTE: SupportedViewsProperty NEVER changes
		}
		
		public override void Disconnect ()
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
			return viewIds;
		}

		public string GetViewName (int viewId)
		{
			if (Array.IndexOf (GetSupportedViews (), viewId) == -1)
				throw new ArgumentException ();
			return Enum.GetName (typeof (SWF.View), viewId);
		}

		public void SetCurrentView (int viewId)
		{
			if (Array.IndexOf (GetSupportedViews (), viewId) == -1)
				throw new ArgumentException ();
			listViewProvider.View = (SWF.View) viewId;
		}

		public int CurrentView {
			get { return (int) listViewProvider.View; }
		}

		#endregion 

		private static readonly int [] viewIds =
			(int []) Enum.GetValues (typeof (SWF.View));
		private ListViewProvider listViewProvider = null;
	}
}
