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
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;

namespace ControlTypeTest
{
	public class SwitchingButton : Button
	{
		protected override AutomationPeer OnCreateAutomationPeer ()
		{
			if (peer == null)
				peer = new SwitchingButtonAutomationPeer (this);

			return peer;
		}

		private AutomationPeer peer = null;
	}

	public class SwitchingButtonAutomationPeer : ButtonAutomationPeer
	{
		public AutomationControlType ControlType {
			get { return controlType; }
			set {
				if (value == controlType)
					return;

				RaisePropertyChangedEvent (
					AutomationElementIdentifiers.ControlTypeProperty,
					controlType, value
				);

				controlType = value;
			}
		}

		public SwitchingButtonAutomationPeer (SwitchingButton b)
			: base (b)
		{
		}

		protected override AutomationControlType GetAutomationControlTypeCore ()
		{
			return controlType;
		}

		private AutomationControlType controlType = AutomationControlType.Button;
	}
}
