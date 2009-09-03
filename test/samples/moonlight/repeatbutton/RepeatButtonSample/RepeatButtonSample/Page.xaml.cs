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

namespace RepeatButtonSample
{
    public partial class Page : UserControl
    {
        static int click_time = 0;
        public Page()
        {
            InitializeComponent();
        }
        void RepeatButton_Click(object sender, RoutedEventArgs e)
        {
            //this.label.Text = DateTime.Now.ToString(System.Globalization.CultureInfo.InvariantCulture);
            click_time += 1;
            this.label.Text = "Number of Clicks: " + click_time;
        }
    }
}
