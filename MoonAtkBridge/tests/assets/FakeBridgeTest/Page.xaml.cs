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

using System.Reflection;

namespace FakeBridgeTest {
	public partial class Page : UserControl {
		public Page ()
		{
			InitializeComponent ();
			instance = this;
		}

		public static Page instance = null;
		public static Page Instance {
			get { return instance; }
		}

		public Button TheStatusLabel {
			get { return label; }
		}

		private void Button_Click (object sender, RoutedEventArgs e)
		{
			new MoonAtkBridge.AttackVector ().InternalsVisibleToCall ();
			label.Content = "it worked";
		}
	}
}
