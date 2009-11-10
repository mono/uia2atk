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

namespace ButtonTest
{
	public partial class Page : UserControl
	{
		public Page ()
		{
			InitializeComponent ();
		}

		private void OnDisableButtonClicked (object sender, RoutedEventArgs e)
		{
			button.IsEnabled = !button.IsEnabled;
		}

		private void OnButtonClicked (object sender, RoutedEventArgs e)
		{
			textBlock.Text = (Convert.ToInt32 (textBlock.Text) + 1).ToString ();
		}
	}
}
