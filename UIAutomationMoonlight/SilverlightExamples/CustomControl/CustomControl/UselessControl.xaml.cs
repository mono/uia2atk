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
using System.Windows.Automation.Peers;

namespace CustomControl
{
	public partial class UselessControl : UserControl
	{
		public UselessControl ()
		{
			InitializeComponent ();
		}

		public void RemoveUselessChildControl (UselessChildControl control)
		{
			Grid content = (Grid) Content;
			if (content.Children.Contains (control)) {
				content.Children.Remove (control);
				AutomationPeer peer = FrameworkElementAutomationPeer.FromElement (control);
				if (peer != null)
					peer.RaiseAutomationEvent (AutomationEvents.StructureChanged);
			}
		}

		protected override AutomationPeer OnCreateAutomationPeer ()
		{
			return new UselessControlPeer (this);
		}

		public List<UselessChildControl> UselessChildrenControls {
			get {
				Grid content = (Grid) Content;
				List<UselessChildControl> children = new List<UselessChildControl> ();
				foreach (UselessChildControl child in content.Children)
					children.Add (child);
				return children;
			}
		}

		int clicks = 0;
	}
}
