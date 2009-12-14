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

namespace RepeatButtonTest
{
	public partial class Page : UserControl
	{
		public Page ()
		{
			InitializeComponent ();
		}

		private void OnRepeatButtonClicked (object o, RoutedEventArgs args)
		{
			count++;

			textBlock.Text = String.Format ("Boom {0}!", count);
		}

		private int count = 0;
	}
}
