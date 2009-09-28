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

namespace CalendarSample
{
    public partial class Page : UserControl
    {
        public Page()
        {
            InitializeComponent();
            //default display and selected date time
            calendar1.DisplayDate = new DateTime(2009, 1, 1);
            calendar1.SelectedDate = new DateTime(2009, 1, 15);
            //BlackoutDate won't be selected
            calendar1.BlackoutDates.Add(new CalendarDateRange(new DateTime(2009, 1, 22)));

            label2.Text = "DisplayDate: " + calendar1.DisplayDate + " SelectedDate: " + calendar1.SelectedDate;

            calendar1.DisplayDateChanged += new EventHandler<CalendarDateChangedEventArgs>(ca1_DisplayDateChanged);
            calendar1.SelectedDatesChanged += new EventHandler<SelectionChangedEventArgs>(ca1_SelectedDatesChanged);

        }
        void HandleChecked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb.Name == "multiselect")
            {
                calendar1.SelectionMode = CalendarSelectionMode.MultipleRange;
                label1.Text = "A basic Calendar with MultipleRange SelectionMode:";
                calendar1.SelectedDates.AddRange(new DateTime(2009, 1, 10), new DateTime(2009, 1, 20));
            }
            if (cb.Name == "decade")
            {
                calendar2.DisplayMode = CalendarMode.Decade;
            }
        }
        void HandleUnchecked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb.Name == "multiselect")
            {
                calendar1.SelectedDate = new DateTime(2009, 1, 15);
                calendar1.SelectionMode = CalendarSelectionMode.SingleDate;
                label1.Text = "A basic Calendar with SingleDate SelectionMode:";
                //try
                //{
                //    calendar1.SelectionMode = CalendarSelectionMode.SingleDate;
                //}
                //catch (Exception ex)
                //{
                //    label1.Text = ex.Message;
                //}
            }
            if (cb.Name == "decade")
            {
                calendar2.DisplayMode = CalendarMode.Year;
            }
        }
        void ca1_DisplayDateChanged(object sender, CalendarDateChangedEventArgs e)
        {
            label2.Text = "Changed DisplayDate to " + e.AddedDate;
        }
        void ca1_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
                label2.Text = "Changed SelectedDate to  " + e.AddedItems[0];
        }
    }
}