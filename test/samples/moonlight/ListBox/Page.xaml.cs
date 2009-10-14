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

namespace ListBoxSample
{
    public partial class Page : UserControl
    {
        public Page()
        {
            InitializeComponent();

            textBlock.Text = "You selected no item.";
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBoxItem listBoxItem = ((sender as ListBox).SelectedItem as ListBoxItem);
            textBlock.Text = "You selected " + listBoxItem.Content.ToString() + ".";
        }
    }
}
