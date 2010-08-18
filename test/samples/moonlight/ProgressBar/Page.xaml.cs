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

namespace ProgressBarSample
{
    public partial class Page : UserControl
    {
        public Page()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (progressBar.Value < progressBar.Maximum)
                progressBar.Value += 20;
            else
                progressBar.Value = 0;

            textBlock.Text = string.Format("It is {0} out of 100%.", progressBar.Value);
        }
    }
}
