using System;
using System.Collections.ObjectModel;
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

namespace DataGridTest {
	public partial class Page : UserControl {
		public Page ()
		{
			InitializeComponent ();

			data = new ObservableCollection<Data> ();
			Data item1 = new Data () { BoolColumn = false, EditColumn = "Edit0", ReadColumn = "Read0" };
			Data item2 = new Data () { BoolColumn = true, EditColumn = "Edit1", ReadColumn = "Read1" };
			Data item3 = new Data () { BoolColumn = false, EditColumn = "Edit2", ReadColumn = "Read2" };
			Data item4 = new Data () { BoolColumn = true, EditColumn = "Edit3", ReadColumn = "Read3" };

			data.Add (item1);
			data.Add (item2);
			data.Add (item3);
			data.Add (item4);
			datagrid.ItemsSource = data;

			texblock.Text = data.Count.ToString ();
		}

		private void ButtonAdd_Click (object sender, RoutedEventArgs e)
		{
			data.Add (new Data () {
				BoolColumn = data.Count % 2 == 0,
				EditColumn = string.Format ("Edit{0}", data.Count),
				ReadColumn = string.Format ("Read{0}", data.Count),
			});
			texblock.Text = data.Count.ToString ();
		}

		private void ButtonRemove_Click (object sender, RoutedEventArgs e)
		{
			if (data.Count > 0) {
				data.RemoveAt (data.Count - 1);
				texblock.Text = data.Count.ToString ();
			}
		}

		private ObservableCollection<Data> data;
	}

   public class Data
    {
        public bool? BoolColumn { get; set; }
        public string EditColumn { get; set; }
        public string ReadColumn { get; set; }
    }
}
