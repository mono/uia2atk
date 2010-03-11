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

			comboboxItemsSource.SetValue (AutomationProperties.NameProperty, "ComboBoxItemsSource");
			List<Car> carList = new List<Car>();
			carList.Add(new Car() { Name = "Ferrari", Price = 150000 });
			carList.Add(new Car() { Name = "Honda", Price = 12500 });
			carList.Add(new Car() { Name = "Toyota", Price = 11500 });
			comboboxItemsSource.DisplayMemberPath = "Name";
			comboboxItemsSource.ItemsSource = carList;
		}
	}

	public class Car {
		public string Name { get; set; }
		public int Price { get; set; }
	}
}
