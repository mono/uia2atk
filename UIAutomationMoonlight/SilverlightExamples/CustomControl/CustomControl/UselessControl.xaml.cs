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
			InitializeComponent();
			MouseLeftButtonUp += delegate (object sender, MouseButtonEventArgs e) {
				e.Handled = true;
				Click ();
			};
		}

		public event EventHandler<EventArgs> Clicked;

		public int Clicks {
			get { return clicks; }
		}

		public void Click ()
		{
			clicks++;
			if (Clicked != null)
				Clicked (this, EventArgs.Empty);
		}

		protected override AutomationPeer OnCreateAutomationPeer ()
		{
			return new UselessControlPeer (this);
		}

		int clicks = 0;
	}
}
