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

namespace DefaultControls
{
    public partial class Page : UserControl
    {
        public Page()
        {
            InitializeComponent();

            List<Data> source = new List<Data>();
            int itemsCount = 50;

            for (int i = 0; i < itemsCount; i++)
            {
                source.Add(new Data()
                {
                    FirstName = "First",
                    LastName = "Last",
                    Age = i,
                    Available = i % 2 == 0,
                    Birthday = DateTime.Today.AddYears(-i),
                    Position = (Position) (i % 2)
                });
            }

            myDataGrid.ItemsSource = source;

            // Automatic columns
            List<Data1> myAutomaticSource = new List<Data1> ();
            for (int i = 0; i < itemsCount; i++)
            {
                myAutomaticSource.Add(new Data1()
                {
                    FirstName = "First",
                    LastName = "Last"
                });
            }
            myAutomaticDatagrid.ItemsSource = myAutomaticSource;
        }
    }

    public class Data
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public bool Available { get; set; }
        public DateTime Birthday { get; set; }
        public Position Position { get; set; }
    }

    public class Data1
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
    }

    public enum Position
    {
        Employee = 0,
        Contractor = 1,
        Inactive = 2
    }
}
