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

namespace TextBoxSample
{
    public partial class Page : UserControl
    {
        public Page()
        {
            InitializeComponent();
        }
        private void ReadWriteTB_TextChanged(object sender, RoutedEventArgs e)
        {
            ReadOnlyTB.Text = ReadWriteTB.Text;
        }

        //The foreground color of the text in SearchTB is set to Magenta when SearchTB
        //gets focus.
        private void SearchTB_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchTB.Text = "";
            SolidColorBrush Brush1 = new SolidColorBrush();
            Brush1.Color = Colors.Magenta;
            SearchTB.Foreground = Brush1;

        }

        //The foreground color of the text in SearchTB is set to Blue when SearchTB
        //loses focus. Also, if SearchTB loses focus and no text is entered, the
        //text "Search" is displayed.
        private void SearchTB_LostFocus(object sender, RoutedEventArgs e)
        {
            if (SearchTB.Text == String.Empty)
            {
                SearchTB.Text = "Search";
                SolidColorBrush Brush2 = new SolidColorBrush();
                Brush2.Color = Colors.Blue;
                SearchTB.Foreground = Brush2;
            }
        }
    }
}

