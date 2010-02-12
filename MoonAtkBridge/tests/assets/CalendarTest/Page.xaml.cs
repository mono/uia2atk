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
using System.Windows.Automation;

namespace CalendarTest
{
    public partial class Page : UserControl
    {
        public Page()
        {
            InitializeComponent ();
	    calendar.SelectedDate = null;
	    calendar.DisplayDate = new DateTime (1983, 04, 01);

	    button1s.SetValue (AutomationProperties.NameProperty, "Button1S");
	    button1u.SetValue (AutomationProperties.NameProperty, "Button1U");
	    button2s.SetValue (AutomationProperties.NameProperty, "Button2S");
	    button2u.SetValue (AutomationProperties.NameProperty, "Button2U");
	    button3s.SetValue (AutomationProperties.NameProperty, "Button3S");
	    button3u.SetValue (AutomationProperties.NameProperty, "Button3U");
	    button4.SetValue (AutomationProperties.NameProperty, "Button4");
	}

        void Button1S_Click (object sender, RoutedEventArgs e)
	{
	    calendar.SelectedDates.Add (new DateTime (1983, 04, 03));
	}

        void Button1U_Click (object sender, RoutedEventArgs e)
	{
	    calendar.SelectedDates.Remove (new DateTime (1983, 04, 03));
	}

        void Button2S_Click (object sender, RoutedEventArgs e)
	{
	    calendar.SelectedDates.Add (new DateTime (1983, 04, 15));
	}

        void Button2U_Click (object sender, RoutedEventArgs e)
	{
	    calendar.SelectedDates.Remove (new DateTime (1983, 04, 15));
	}

        void Button3S_Click (object sender, RoutedEventArgs e)
	{
	    calendar.SelectedDates.Add (new DateTime (1983, 04, 22));
	}

        void Button3U_Click (object sender, RoutedEventArgs e)
	{
	    calendar.SelectedDates.Remove (new DateTime (1983, 04, 22));
	}

        void Button4_Click (object sender, RoutedEventArgs e)
	{
	    calendar.SelectedDate = new DateTime (1983, 04, 23);
	}
    }
}
