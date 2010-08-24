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

namespace ScrollViewerSample
{
    public partial class Page : UserControl
    {
        public Page()
        {
            InitializeComponent();
        }
        void Button_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            if (button.Name == "Button1")
            {
                if (ScrollViewer1.VerticalScrollBarVisibility == ScrollBarVisibility.Auto)
                {
                    ScrollViewer1.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                    button.Content = "Auto Vertical";
                }
                else
                {
                    ScrollViewer1.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                    button.Content = "Hidden Vertical";
                }
            }
            else if (button.Name == "Button2")
            {
                if (ScrollViewer1.HorizontalScrollBarVisibility == ScrollBarVisibility.Auto)
                {
                    ScrollViewer1.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                    button.Content = "Auto Horizontal";
                }
                else
                {
                    ScrollViewer1.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                    button.Content = "Hidden Horizontal";
                }
            }
            else
            {
                if (ScrollViewer1.Width == 300)
                {
                    ScrollViewer1.Width = 500;
                    ScrollViewer1.Height = 500;
                }
                else
                {
                    ScrollViewer1.Width = 300;
                    ScrollViewer1.Height = 300;
                }
            }
        }
    }
}
