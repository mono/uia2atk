// Permission is hereby granted, free of charge, to any person obtaining 
// a copy of this software and associated documentation files (the 
// "Software"), to deal in the Software without restriction, including 
// without limitation the rights to use, copy, modify, merge, publish, 
// distribute, sublicense, and/or sell copies of the Software, and to 
// permit persons to whom the Software is furnished to do so, subject to 
// the following conditions: 
//  
// The above copyright notice and this permission notice shall be 
// included in all copies or substantial portions of the Software. 
//  
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE 
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION 
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
// 
// Copyright (c) 2008 Novell, Inc. (http://www.novell.com) 
// 
// Authors: 
//	Mario Carrion <mcarrion@novell.com>
// 
using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using DataGridAssembly;

namespace DataGridApplication
{
    public partial class DatagridForm : Form
    {
        public DatagridForm () : this (null)
        {
        }

        protected override void OnClosed (EventArgs e)
        {
            base.OnClosed (e);

            // Writing information to test changes.
            WriteData (1);
        }

        public DatagridForm (string []args)
        {
            InitializeComponent ();

            if (args == null || args.Length == 0 || args [0] == "0")
                BindableReadonlyElement ();
            else {
                if (args [0] == "1")
                    BindableReadWriteElement();
                else if (args [0] == "2")
                    DataSetDataBinding ();
                else
                    BindableReadonlyElement ();
            }

            WriteData (0);
        }

        private void BindableReadonlyElement ()
        {
            ArrayList arraylist = new ArrayList ();

            for (int index = 0; index < Elements; index++)
                arraylist.Add (new BindableReadonlyElement (index, 
                    string.Format ("Name{0}", index)));

            dataGrid.DataSource = arraylist;
            option = 0;
        }

        private void BindableReadWriteElement ()
        {
            ArrayList arraylist = new ArrayList ();

            for (int index = 0; index < Elements; index++)
                arraylist.Add (new BindableReadWriteElement (index,
                    string.Format ("Name{0}", index)));

            dataGrid.DataSource = arraylist;
            option = 1;
        }

        private void DataSetDataBinding ()
        {
            // FIXME: Port datagrid.py
            option = 2;
        }

        private void WriteData (int step)
        {
            FileStream stream = new FileStream (string.Format ("{0}.{1}.bin", 
                Process.GetCurrentProcess ().Id, step), FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter ();
            bf.Serialize (stream, dataGrid.DataSource);
            stream.Flush ();
            stream.Close ();
            stream.Dispose ();
        }

        private const int Elements = 10;
        private int option;
    }
}
