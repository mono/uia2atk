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

namespace PasswordBox
{
    public partial class Page : UserControl
    {
        public Page()
        {
            InitializeComponent();
        }

        private int pwdChanges = 0;

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            int count = ++pwdChanges;
            string s = count == 1 ? "" : "s";
            textBlock.Text = string.Format("You changed {0} time{1}.", Convert.ToString(count), s);
        }
    }
}
