using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Automation;
using System.Windows.Automation.Peers;

namespace ControlTypeTest
{
	public partial class Page : UserControl
	{
		public Page ()
		{
			InitializeComponent ();
		}

		private void SwitchingButton_Click (object sender, RoutedEventArgs e)
		{
			var peer = FrameworkElementAutomationPeer.CreatePeerForElement (button) as SwitchingButtonAutomationPeer;
			switch (peer.ControlType) {
			case AutomationControlType.Button:
				peer.ControlType = AutomationControlType.RadioButton;
				break;
			default:
				peer.ControlType = AutomationControlType.Button;
				break;
			}
		}
	}
}
