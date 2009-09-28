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

namespace SliderSample
{
    public partial class Page : UserControl
    {
        public Page()
        {
            InitializeComponent();

            vertical_slider.Minimum = 10;
            vertical_slider.Value = 15;

            label1.Text = "Horizontal Slider Value: " + horizontal_slider.Value.ToString();
            label2.Text = "Vertical Slider Value: " + vertical_slider.Value.ToString();
        }
        void slider_ValueChange(object sender, RoutedEventArgs e)
        {
            Slider slider = sender as Slider;

            if (slider.Name == "horizontal_slider")
            {
                label1.Text = "Horizontal Slider Value: " + slider.Value.ToString();
            }
            else
            {
                label2.Text = "Vertical Slider Value: " + slider.Value.ToString();
            }
        }
        void checkBox_clickEvent(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb.IsChecked == true)
                vertical_slider.IsDirectionReversed = true;
            else
                vertical_slider.IsDirectionReversed = false;
        }
    }
}
