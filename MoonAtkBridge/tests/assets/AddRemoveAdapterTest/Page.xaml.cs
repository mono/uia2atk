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

namespace AddRemoveAdapterTest
{
	public partial class Page : UserControl
	{
		public Page ()
		{
			InitializeComponent ();
		}

		private void removeButton_Click (object sender, RoutedEventArgs e)
		{
			if (listBox.Items.Count == 0)
				return;

			listBox.Items.RemoveAt (listBox.Items.Count - 1);
		}

		private void addButton_Click (object sender, RoutedEventArgs e)
		{
			listBox.Items.Add (new ListBoxItem () {
				Content = new TextBlock () {
					Text = String.Format ("Item {0}", count)
				}
			});
			count++;
		}
		
		private int count = 2;
	}
}
