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

            textBlock1.Text = "You changed 0 times.";
            textBlock2.Text = "Your password is: ";
        }

        private int pwdChanges = 0;

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            textBlock1.Text = string.Format("You changed {0} times.", ++pwdChanges);
            textBlock2.Text = string.Format("Your password is: {0}", pwdBox.Password);

            if (pwdBox.Password.Length == pwdBox.MaxLength)
                MessageBox.Show("You've entered the maximum characters.");
        }
    }
}
