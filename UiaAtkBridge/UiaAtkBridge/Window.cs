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
// Copyright (c) 2008-2009 Novell, Inc. (http://www.novell.com)
// 
// Authors: 
//      Andres G. Aragoneses <aaragoneses@novell.com>
//      Mario Carrion <mcarrion@novell.com>
// 

using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

namespace UiaAtkBridge
{
	public class Window : ComponentParentAdapter
	{
		private IWindowProvider windowProvider;
		private ITransformProvider transformProvider;
		private IRawElementProviderFragmentRoot rootProvider;
		private Splitter splitter = null;
		private bool balloonWindow = false;
		private TextLabel fakeLabel = null;
		private Image fakeImage = null;
		
		public Window (IRawElementProviderSimple provider) : base (provider)
		{
			if (provider != null)
				Role = Atk.Role.Frame;
			
			balloonWindow = (bool)(provider.GetPropertyValue (AutomationElementIdentifiers.IsNotifyIconProperty.Id) != null);
			rootProvider = (IRawElementProviderFragmentRoot) provider;
			
			if (rootProvider != null && balloonWindow) {
				Role = Atk.Role.Alert;
				Name = (string) provider.GetPropertyValue (AutomationElementIdentifiers.HelpTextProperty.Id);
			}

			transformProvider = (ITransformProvider) provider.GetPatternProvider (TransformPatternIdentifiers.Pattern.Id);
			windowProvider = (IWindowProvider) provider.GetPatternProvider (WindowPatternIdentifiers.Pattern.Id);
		}

		internal Window () : base (null)
		{
			Role = Atk.Role.Window;
		}
		
		internal override void PostInit ()
		{
			base.PostInit ();
			if (balloonWindow) {
				fakeLabel = AutomationBridge.CreateAdapter<TextLabel> (Provider);
				if (fakeLabel != null)
					AddOneChild (fakeLabel);
				fakeImage = AutomationBridge.CreateAdapter<Image> (Provider);
				if (fakeImage != null)
					AddOneChild (fakeImage);
			} else {
				if (RefStateSet ().ContainsState (Atk.StateType.Modal))
					Role = Atk.Role.Dialog;
			}
		}
		
		public override void RaiseStructureChangedEvent (object childProvider, StructureChangedEventArgs e)
		{
			/*IRawElementProviderSimple simpleChildProvider =
				(IRawElementProviderSimple) childProvider;
			//TODO: remove elements
			if (e.StructureChangeType == StructureChangeType.ChildrenBulkAdded) {
				int controlTypeId = (int) simpleChildProvider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id);
				if (controlTypeId == ControlType.Button.Id) {
					// TODO: Consider generalizing...
					Button button = new Button ((IInvokeProvider) childProvider);
					AddOneChild (button);
					AddRelationship (Atk.RelationType.Embeds, button);
					//TODO: add to mappings
				}
			}*/
		}
		
		public override void RaiseAutomationEvent (AutomationEvent eventId, AutomationEventArgs e)
		{
			if (eventId == AutomationElementIdentifiers.AutomationFocusChangedEvent)
				GainActiveState ();
			if (eventId == AutomationElementIdentifiers.WindowDeactivatedEvent)
				LoseActiveState ();
			else
				base.RaiseAutomationEvent (eventId, e);
		}

		public override void RaiseAutomationPropertyChangedEvent (AutomationPropertyChangedEventArgs e)
		{
			if (fakeLabel != null && balloonWindow && e.Property == AutomationElementIdentifiers.NameProperty)
				fakeLabel.RaiseAutomationPropertyChangedEvent (e);
			else if (e.Property == TransformPatternIdentifiers.CanResizeProperty)
				NotifyStateChange (Atk.StateType.Resizable, (bool) e.NewValue);
			else if (e.Property == WindowPatternIdentifiers.WindowVisualStateProperty) {
				WindowVisualState newValue = (WindowVisualState) e.NewValue;
				
				if (newValue == WindowVisualState.Maximized)
					GLib.Signal.Emit (this, "maximize");
				else if (newValue == WindowVisualState.Minimized)
					GLib.Signal.Emit (this, "minimize");
				else // Back to Normal, so is Restored
					GLib.Signal.Emit (this, "restore");
			} else if (e.Property == AutomationElementIdentifiers.BoundingRectangleProperty) {
				Rect oldValue = (Rect) e.OldValue;
				Rect newValue = (Rect) e.NewValue;
				
				if (oldValue.X != newValue.X || oldValue.Y != newValue.Y)
					GLib.Signal.Emit (this, "move");
				if (oldValue.Width != newValue.Width || oldValue.Height != newValue.Height)
					GLib.Signal.Emit (this, "resize");

				base.RaiseAutomationPropertyChangedEvent (e);
			} else
				base.RaiseAutomationPropertyChangedEvent (e);
		}
		
		protected override void RemoveUnmanagedChildren ()
		{
			if (fakeLabel != null)
				RemoveChild (fakeLabel);
			if (fakeImage != null)
				RemoveChild (fakeImage);
			fakeLabel = null;
			fakeImage = null;
		}

		public override Atk.Layer Layer {
			get { return Atk.Layer.Window; }
		}
		
		public override int MdiZorder {
			get { return -1; }
		}

		private bool active = false;
		private bool needStateChange = false;
		
		protected override Atk.StateSet OnRefStateSet ()
		{
			Atk.StateSet states = base.OnRefStateSet ();

			if (states.ContainsState (Atk.StateType.Defunct))
				return states;

			if (active)
				states.AddState (Atk.StateType.Active);
			else
				states.RemoveState (Atk.StateType.Active);

			if (transformProvider != null && transformProvider.CanResize)
				states.AddState (Atk.StateType.Resizable);

			if (windowProvider != null) {
				if (windowProvider.IsModal)
					states.AddState (Atk.StateType.Modal);
				else
					states.RemoveState (Atk.StateType.Modal);
			}
			
			states.RemoveState (Atk.StateType.Focusable);

			return states;
		}
		
		internal override void AddOneChild (Atk.Object child)
		{
			if (splitter != null) {
				splitter.AddOneChild (child);
				return;
			}

			base.AddOneChild (child);
			if (child is Splitter) {
				splitter = (Splitter)child;
				// Remove any other existing children and
				// add them to the splitter
				int count = NAccessibleChildren;
				for (int i = 0; i < count;) {
					Atk.Object obj = RefAccessibleChild (i);
					if (obj == child) {
						i++;
						continue;
					}
					RemoveChild (obj, false);
					obj.Parent = child;
					splitter.AddOneChild (obj);
					count--;
				}
			//LAMESPEC: yeah, atk docs just mention this subtle difference between a Frame and a Dialog...
			} else if (child is MenuBar && Role == Atk.Role.Dialog)
				Role = Atk.Role.Frame;
		}

		internal override void PreRemoveChild (Atk.Object childToRemove)
		{
			if (splitter != null && childToRemove == splitter) {
				splitter = null;
				ParentAdapter parentAdapter = childToRemove as ParentAdapter;
				int count = parentAdapter.NAccessibleChildren;
				while (count > 0) {
					Atk.Object obj = parentAdapter.RefAccessibleChild (0);
					parentAdapter.RemoveChild (obj, false);
					obj.Parent = this;
					AddOneChild (obj);
					count--;
				}
			}
			base.PreRemoveChild (childToRemove);
		}

		private void NewActiveState (bool active)
		{
			if (this.active == active)
				return;
			this.active = active;
			if (active)
				GLib.Signal.Emit (this, "activate");
			needStateChange = true;
		}
		
		// gail sends activate followed by object:state-changed:focused
		// for the control, followed by state-changed:active, so this
		// function emulates that order
		internal void SendActiveStateChange ()
		{
			if (needStateChange) {
				NotifyStateChange (Atk.StateType.Active, active);
				needStateChange = false;
			}
		}

		internal void LoseActiveState ()
		{
			NewActiveState (false);
			TopLevelRootItem.Instance.WindowDeactivated (this);
		}

		internal void GainActiveState ()
		{
			NewActiveState (true);
		}
	}
}
