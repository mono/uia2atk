using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using System.Collections.Generic;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;

namespace CustomControl
{
	public class UselessControlPeer : FrameworkElementAutomationPeer, ISelectionProvider
	{
		public UselessControlPeer (UselessControl control)
			: base (control)
		{
			//AcceleratorKeyProperty
			//AccessKeyProperty
			//AutomationIdProperty
			//BoundingRectangleProperty
			//ClassNameProperty
			//ClickablePointProperty
			//ControlTypeProperty
			//HasKeyboardFocusProperty
			//HelpTextProperty
			//IsContentElementProperty
			//IsControlElementProperty
			//IsEnabledProperty
			//IsKeyboardFocusableProperty
			//IsOffscreenProperty
			//IsPasswordProperty
			//IsRequiredForFormProperty
			//ItemStatusProperty
			//ItemTypeProperty
			//LabeledByProperty
			//LocalizedControlTypeProperty
			//NameProperty
			//OrientationProperty
		}

		#region Overridden methods

		protected override string GetAcceleratorKeyCore ()
		{
			return base.GetAcceleratorKeyCore ();
		}

		protected override string GetAccessKeyCore ()
		{
			return base.GetAccessKeyCore ();
		}

		protected override AutomationControlType GetAutomationControlTypeCore ()
		{
			return AutomationControlType.Custom;
		}

		protected override Rect GetBoundingRectangleCore( )
		{
			return base.GetBoundingRectangleCore ();
		}

		protected override List<AutomationPeer> GetChildrenCore ()
		{
			List<AutomationPeer> peers = new List<AutomationPeer> ();
			foreach (UIElement element in uselessControl.UselessChildrenControls)
				peers.Add (FrameworkElementAutomationPeer.CreatePeerForElement (element));
			return peers;
		}

		protected override string GetClassNameCore()
		{
			return "UselessControl";
		}

		protected override string GetNameCore()
		{
			return "I'm an useless control. 2 ellipses and 2 rectangles.";
		}

		protected override bool IsEnabledCore()
		{
			return uselessControl.IsEnabled;
		}

		protected override bool IsContentElementCore()
		{
			return true;
		}

		protected override bool IsControlElementCore()
		{
			return true;
		}

		protected override bool IsPasswordCore()
		{
			return false;
		}

		protected override bool IsRequiredForFormCore()
		{
			return false;
		}

		protected override bool IsKeyboardFocusableCore()
		{
			return false;
		}

		protected override bool HasKeyboardFocusCore()
		{
			return false;
		}

		public override object GetPattern(PatternInterface patternInterface)
		{
			if (patternInterface == PatternInterface.Selection)
				return this;

			return null;
		}

		#endregion

		UselessControl uselessControl;

		#region ISelectionProvider Members

		bool ISelectionProvider.CanSelectMultiple {
			get { return true; }
		}

		IRawElementProviderSimple[] ISelectionProvider.GetSelection ()
		{
			List<IRawElementProviderSimple> selection = new List<IRawElementProviderSimple> ();
			foreach (AutomationPeer child in GetChildrenCore ()) {
				ISelectionItemProvider provider = child.GetPattern (PatternInterface.SelectionItem) as ISelectionItemProvider;
				if (provider != null && provider.IsSelected)
					selection.Add (ProviderFromPeer (child));
			}
			return selection.ToArray ();
		}

		bool ISelectionProvider.IsSelectionRequired {
			get { return false; }
		}

		#endregion
	}
}
