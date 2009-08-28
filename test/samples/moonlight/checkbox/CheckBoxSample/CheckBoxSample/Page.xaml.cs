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

namespace CheckBoxSample
{
    public partial class Page : UserControl
    {
        public Page()
        {
            InitializeComponent();
        }
        void HandleChecked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb.Name == "checkbox1")
                text1.Text = "Two state CheckBox checked";
            else
                text2.Text = "Three state CheckBox checked";
        }
        void HandleUnchecked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb.Name == "checkbox1")
                text1.Text = "Two state CheckBox unchecked";
            else
                text2.Text = "Three state CheckBox unchecked";
        }
        void HandleThirdState(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            text2.Text = "Three state CheckBox indeterminate";
        }
    }
}
