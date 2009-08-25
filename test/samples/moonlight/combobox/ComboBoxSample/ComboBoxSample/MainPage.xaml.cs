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

namespace ComboBoxSample
{
    public partial class MainPage : UserControl
    {
        static ComboBoxItem[] Box1_ItemsList;

        public MainPage()
        {
            InitializeComponent();

            //add carList for combobox 3
            List<Car> carList = new List<Car>();

            carList.Add(new Car() { Name = "Ferrari", Price = 150000 });
            carList.Add(new Car() { Name = "Honda", Price = 12500 });
            carList.Add(new Car() { Name = "Toyota", Price = 11500 });

           // ComboBox cb = new ComboBox();
            combobox3.DisplayMemberPath = "Name";
            //combobox3.DisplayMemberPath = "carList";
            combobox3.ItemsSource = carList;
            combobox3.SelectedIndex = 1;
        }

        void AddButton_click(object sender, RoutedEventArgs e)
        {
            ComboBoxItem new_item = new ComboBoxItem();
            combobox1.Items.Add(new_item);
            new_item.Content = textbox1.Text;
            new_item.Name = "Box1_" + textbox1.Text;
        }

        void DeleteButton_click(object sender, RoutedEventArgs e)
        {
            if(combobox1.SelectedItem != null)
                combobox1.Items.RemoveAt(combobox1.Items.IndexOf(combobox1.SelectedItem));
        }

        void ResetButton_click(object sender, RoutedEventArgs e)
        {
            combobox1.Items.Clear();
            Box1_ItemsList = new ComboBoxItem[] { Box1_Item0, Box1_Item1, Box1_Item2, Box1_Item3, Box1_Item4, Box1_Item5, Box1_Item6, Box1_Item7, Box1_Item8, Box1_Item9 };
            foreach (ComboBoxItem box_items in Box1_ItemsList)
                combobox1.Items.Add(box_items);
            combobox1.SelectedIndex = 0;
        }

        void Item_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            label2.Text = cb.Name + " is Checked";
        }

        void Item_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            label2.Text = cb.Name + " is Unchecked";
        }

         void combobox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
             if (combobox1 != null)
             {
                 if (combobox1.SelectedItem != null)
                 {
                     String selected_item = (combobox1.SelectedItem as ComboBoxItem).Name.ToString();
                     label1.Text = "Selected: " + selected_item;
                 }
             }
        }

         void combobox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
         {
             if (combobox2 != null)
             {
                 String combobox = (combobox2.SelectedItem as ComboBoxItem).Name.ToString();
                 label2.Text = "Selected: " + combobox;
             }

         }

         void combobox3_SelectionChanged(object sender, SelectionChangedEventArgs e)
         {
             Car selectedCar = (Car) combobox3.SelectedItem;
             int price = selectedCar.Price;
             string carName = selectedCar.Name;
             label3.Text = "Selected: " + carName + " " + price;
         }
    }

    public class Car
    {
        public string Name { get; set; }
        public int Price { get; set; }
    }
}
