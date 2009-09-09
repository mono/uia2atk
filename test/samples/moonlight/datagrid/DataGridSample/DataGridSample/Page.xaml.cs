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

namespace DataGridSample
{
    public partial class Page : UserControl
    {
        public Page()
        {
            InitializeComponent();

            //dataGrid.ItemsSource = "H e l l o W o r l d !".Split();

            //int itemsCount = 100;
            //for (int i = 0; i < itemsCount; i++)
            //{
            //    data.Add(new Data()
            //    {
            //        FirstName = "First",
            //        LastName = "Last",
            //        Age = i,
            //        Available = (i % 2 == 0)
            //    });
            //}

            List<Data> data = new List<Data>();
            Data item1 = new Data() { BoolColumn = null, EditColumn = "Edit0", ReadColumn = "Read0" };
            Data item2 = new Data() { BoolColumn = true, EditColumn = "Edit1", ReadColumn = "Read1" };
            Data item3 = new Data() { BoolColumn = false, EditColumn = "Edit2", ReadColumn = "Read2" };

            data.AddRange(new Data[] { item1,item2, item3 });
            dataGrid.ItemsSource = data;
        }
    }
}
