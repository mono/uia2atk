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

namespace ScrollBarSample
{
    public partial class Page : UserControl
    {
        public Page()
        {
            InitializeComponent();
        }

        private void hscrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            textBlock1.Text = string.Format("Value of Horizontal: {0}", hscrollBar.Value);
        }

        private void vscrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            textBlock2.Text = string.Format("Value of Vertical: {0}", vscrollBar.Value);
        }
    }
}
