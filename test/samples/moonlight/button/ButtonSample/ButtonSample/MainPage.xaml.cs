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

namespace ButtonSample
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();

            TextBlock.Text = "Button2 is not clicked now.";
        }

        private int button2ClickedNum = 0;

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Successfully clicked Button1!");
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            int count = ++button2ClickedNum;
            string s = count == 1 ? "" : "s";
            TextBlock.Text = string.Format("Button2 is clicked {0} time{1}.", count.ToString(), s);
        }
    }
}
