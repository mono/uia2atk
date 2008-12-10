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
// 	Neville Gao <nevillegao@gmail.com>
// 

using System;
using System.Drawing;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

using Mono.UIAutomation.Bridge;

using Mono.UIAutomation.Winforms.Behaviors;
using Mono.UIAutomation.Winforms.Behaviors.StatusBar;
using Mono.UIAutomation.Winforms.Events;
using ESB = Mono.UIAutomation.Winforms.Events.StatusBar;
using Mono.UIAutomation.Winforms.Navigation;

namespace Mono.UIAutomation.Winforms
{
	internal class StatusBarProvider : FragmentRootControlProvider
	{
		#region Constructor

		public StatusBarProvider (StatusBar statusBar) : base (statusBar)
		{
			this.statusBar = statusBar;
			panels = new List<StatusBarPanelProvider> ();
		}

		#endregion
		
		#region SimpleControlProvider: Specializations

		public override void Initialize()
		{
			base.Initialize ();
			
			SetBehavior (GridPatternIdentifiers.Pattern,
			             new GridProviderBehavior (this));
		}
		
		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
				return ControlType.StatusBar.Id;
			else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
				return "status bar";
			else if (propertyId == AutomationElementIdentifiers.LabeledByProperty.Id)
				return null;
			else if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
				return statusBar.Text;
			else
				return base.GetProviderPropertyValue (propertyId);
		}
		
		#endregion
		
		#region FragmentRootControlProvider: Specializations
		
		public override void InitializeChildControlStructure ()
		{	
			try {
				Helper.AddPrivateEvent (typeof (StatusBar.StatusBarPanelCollection),
				                        statusBar.Panels,
				                        "UIACollectionChanged",
				                        this,
				                        "OnCollectionChanged");
			} catch (NotSupportedException) { }
		
			for (int i = 0; i < statusBar.Panels.Count; ++i) {
				StatusBarPanelProvider panel = GetPanelProvider (i);
				OnNavigationChildAdded (false, panel);
			}
		}
		
		public override void FinalizeChildControlStructure ()
		{
			try {
				Helper.RemovePrivateEvent (typeof (StatusBar.StatusBarPanelCollection),
				                           statusBar.Panels,
				                           "UIACollectionChanged",
				                           this,
				                           "OnCollectionChanged");
			} catch (NotSupportedException) { }
			
			foreach (StatusBarPanelProvider panel in panels)
				OnNavigationChildRemoved (false, panel);
			OnNavigationChildrenCleared (false);
		}

		#endregion
		
		#region Public Methods
		
		public StatusBarPanelProvider GetPanelProvider (int index)
		{
			StatusBarPanelProvider panel = null;
			
			if (index < 0 || index >= statusBar.Panels.Count)
				return null;
			else if (index >= panels.Count) {
				for (int loop = panels.Count - 1; loop < index; ++loop) {
					panel = new StatusBarPanelProvider (statusBar.Panels [index]);
					panels.Add (panel);
					panel.Initialize ();
				}
			}
			
			return panels [index];
		}
		
		public StatusBarPanelProvider RemovePanelAt (int index)
		{
			StatusBarPanelProvider panel = null;
			
			if (index < panels.Count) {
				panel = panels [index];
				panels.RemoveAt (index);
				panel.Terminate ();
			}
			
			return panel;
		}
		
		public void ClearPanelsCollection ()
		{
			while (panels.Count > 0) {
				RemovePanelAt (panels.Count - 1);
			}
		}
		
		#endregion
		
		#region Private Methods

		#pragma warning disable 169
		
		private void OnCollectionChanged (object sender, CollectionChangeEventArgs e)
		{
			if (e.Action == CollectionChangeAction.Add) {
				StatusBarPanelProvider panel = GetPanelProvider ((int) e.Element);
				OnNavigationChildAdded (true, panel);
			} else if (e.Action == CollectionChangeAction.Remove) {
				StatusBarPanelProvider panel = RemovePanelAt ((int) e.Element);
				OnNavigationChildRemoved (true, panel);
			} else if (e.Action == CollectionChangeAction.Refresh) {
				ClearPanelsCollection ();
				OnNavigationChildrenCleared (true);
			}
		}
		
		#pragma warning restore 169
		
		#endregion
		
		#region Private Fields
		
		private StatusBar statusBar;
		private List<StatusBarPanelProvider> panels;
		
		#endregion
		
		#region Internal Class: StatusBarPanel Provider
		
		internal class StatusBarPanelProvider : FragmentControlProvider, IEmbeddedImage
		{
			#region Constructor

			public StatusBarPanelProvider (StatusBarPanel statusBarPanel) : base (statusBarPanel)
			{
				this.statusBarPanel = statusBarPanel;
			}
		
			#endregion
			
			#region SimpleControlProvider: Specializations

			public override void Initialize()
			{
				base.Initialize ();
				
				SetBehavior (GridItemPatternIdentifiers.Pattern,
				             new StatusBarPanelGridItemProviderBehavior (this));
				SetEvent (ProviderEventType.AutomationElementNameProperty,
				          new ESB.AutomationNamePropertyEvent (this));
				SetEvent (ProviderEventType.TextPatternTextChangedEvent,
				          new ESB.TextPatternTextChangedEvent (this));
			}
			
			#endregion
		
			#region Public Methods
		
			protected override object GetProviderPropertyValue (int propertyId)
			{
				if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
					return ControlType.Text.Id;
				else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
					return "text";
				else if (propertyId == AutomationElementIdentifiers.BoundingRectangleProperty.Id)
					return BoundingRectangle;
				else if (propertyId == AutomationElementIdentifiers.IsEnabledProperty.Id)
					return statusBarPanel.Parent != null && statusBarPanel.Parent.Enabled;
				else if (propertyId == AutomationElementIdentifiers.IsOffscreenProperty.Id)
					return IsOffscreen ();
				else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
					return false;
				else if (propertyId == AutomationElementIdentifiers.HasKeyboardFocusProperty.Id)
					return false;
				else if (propertyId == AutomationElementIdentifiers.LabeledByProperty.Id)
					return null;
				else
					return base.GetProviderPropertyValue (propertyId);
			}
		
			public override Rect BoundingRectangle {
				get {
					return Helper.RectangleToRect (GetScreenBounds ());
				}
			}

			public System.Drawing.Rectangle GetScreenBounds ()
			{
				System.Drawing.Rectangle rectangle = System.Drawing.Rectangle.Empty;
				Control parent = statusBarPanel.Parent;
				if (parent != null && parent.Parent != null)
					rectangle = parent.Parent.RectangleToScreen (parent.Bounds);
				rectangle.X += statusBarPanel.X;
				rectangle.Width = statusBarPanel.Width;
				return rectangle;
			}

			public bool IsOffscreen()
			{
				System.Drawing.Rectangle bounds =
					GetScreenBounds ();				
				System.Drawing.Rectangle screen =
					Screen.GetWorkingArea (bounds);
				// True iff the *entire* control is off-screen
				return !screen.Contains (bounds.Left, bounds.Bottom) &&
					!screen.Contains (bounds.Left, bounds.Top) &&
					!screen.Contains (bounds.Right, bounds.Bottom) &&
					!screen.Contains (bounds.Right, bounds.Top);
			}
			#endregion

			#region IEmbedded Image Members

			public Rect Bounds {
				get {
					if (statusBarPanel.Icon == null)
						return Rect.Empty;
					Rect boundingRect = BoundingRectangle;

					// The following code comes mostly from
					// ThemeWin32Classic.DrawStatusBarPanel
					Image backbuffer = new Bitmap (statusBarPanel.Parent.ClientSize.Width,
					                               statusBarPanel.Parent.ClientSize.Height);
					
					using (Graphics dc = Graphics.FromImage (backbuffer)) {
						int x;
						int len;
						int icon_x = 0;
						int y = (int) boundingRect.Y +
							(((int) boundingRect.Height / 2 - (int) statusBarPanel.Parent.Font.Size / 2) - 1);
	
						string text = statusBarPanel.Text;
						
						switch (statusBarPanel.Alignment) {
						case HorizontalAlignment.Right:
							len = (int) dc.MeasureString (text, statusBarPanel.Parent.Font).Width;
							x = (int) boundingRect.Right - len - 4;
							icon_x = x - statusBarPanel.Icon.Width - 2;
							break;
						case HorizontalAlignment.Center:
							len = (int) dc.MeasureString (text, statusBarPanel.Parent.Font).Width;
							x = (int) boundingRect.Left + ((statusBarPanel.Width - len) / 2);
							icon_x = x - statusBarPanel.Icon.Width - 2;
							break;
						default:
							icon_x = (int) boundingRect.Left + 2;
							break;
						}
	
						return new Rect (icon_x,
						                 y,
						                 statusBarPanel.Icon.Width,
						                 statusBarPanel.Icon.Height);
					}
				}
			}
	
			public string Description {
				get { return string.Empty; }
			}

			#endregion
			
			#region Private Fields
		
			private StatusBarPanel statusBarPanel;
		
			#endregion
		}
		
		#endregion
	}
}
