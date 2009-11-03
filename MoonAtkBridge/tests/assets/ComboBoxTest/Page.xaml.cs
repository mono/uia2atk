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

namespace ComboBoxTest {
	public partial class Page : UserControl {
		public Page ()
		{
			InitializeComponent ();

			button.SetValue (AutomationProperties.NameProperty, "Button");
			combobox.SetValue (AutomationProperties.NameProperty, "ComboBox");
			combobox.SetValue (AutomationProperties.AccessKeyProperty, "ALT+A");

			bool set = true;
			button.Click += (o, e) => {
				combobox.SetValue (AutomationProperties.AccessKeyProperty, 
				                   set ? "ALT+B" : "ALT+A");
				set = !set;
			};

		}
	}
}
