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

namespace DatePickerSample
{
    public partial class Page : UserControl
    {
        public Page()
        {
            InitializeComponent();
        }
        void DatePicker_CalenderOpened(object sender, RoutedEventArgs e)
        {
            label2.Text = "DatePicker2 Calender opended";
        }
        void DatePicker_CalenderClosed(object sender, RoutedEventArgs e)
        {
            label2.Text = "DatePicker2 Calender closed";
        }
        void DatePicker_SelectedDateChanged(Object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 0)
            {
                label1.Text = "Select Date: " + datepicker1.SelectedDate;
            }
        }
    }
}
