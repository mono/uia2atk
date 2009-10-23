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

namespace AdapterEventsTest
{
	public partial class Page : UserControl
	{
		public Page ()
		{
			InitializeComponent ();
		}

		private void OnVisibilityToggled (object sender, RoutedEventArgs e)
		{
			textBox.Visibility = (textBox.Visibility == Visibility.Visible)
				? Visibility.Collapsed : Visibility.Visible;
		}

		private void OnSensitivityToggled (object sender, RoutedEventArgs e)
		{
			textBox.IsEnabled = !textBox.IsEnabled;
		}

		private void OnSizeToggled (object sender, RoutedEventArgs e)
		{
			textBox.Height = textBox.Height == 100 ? 200 : 100;
		}
	}
}
