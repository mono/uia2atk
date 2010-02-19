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

namespace TabControlTest
{
    public partial class Page : UserControl
    {
        public Page()
        {
            InitializeComponent();
        }
        void tab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tab1 != null && tab1.SelectedItem != null)
            {
                 String selected_item = (tab1.SelectedItem as TabItem).Header.ToString();
                 label.Text = "Selected: " + selected_item;
            }
        }
    }
}
