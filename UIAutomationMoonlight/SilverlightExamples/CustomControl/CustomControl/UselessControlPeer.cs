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
	public class UselessControlPeer : FrameworkElementAutomationPeer, IInvokeProvider
	{
		public UselessControlPeer (UselessControl control)
			: base (control)
		{
			uselessControl = control;
			uselessControl.Clicked += delegate (object sender, EventArgs e) {
				RaiseAutomationEvent (AutomationEvents.InvokePatternOnInvoked);

				// Event is not raised
				RaisePropertyChangedEvent (AutomationElementIdentifiers.NameProperty,
					GetFormatedName (uselessControl.Clicks - 1), 
					GetFormatedName (uselessControl.Clicks));

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
			};
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

		protected override List<AutomationPeer> GetChildrenCore()
		{
			return null;
		}

		protected override string GetClassNameCore()
		{
			return "UselessControl";
		}

		protected override string GetNameCore()
		{
			return GetFormatedName (uselessControl.Clicks);
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

		public override object GetPattern (PatternInterface patternInterface)
		{
			if (patternInterface == PatternInterface.Invoke)
				return this;

			return null;
		}

		#endregion

		#region IInvokeProvider Members

		void IInvokeProvider.Invoke ()
		{
			uselessControl.Click ();
		}

		#endregion

		#region Private members

		string GetFormatedName (int clicks)
		{
			return string.Format("I'm an useless control clicked: {0} times", clicks);
		}

		#endregion

		UselessControl uselessControl;

	}
}
