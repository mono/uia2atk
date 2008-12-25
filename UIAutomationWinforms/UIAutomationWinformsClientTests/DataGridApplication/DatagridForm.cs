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
using System.Drawing;
using System.Windows.Forms;

namespace DataGridApplication
{
    public partial class DatagridForm : Form
    {
        public DatagridForm () : this (null)
        {
        }

        public DatagridForm (string []args)
        {
            InitializeComponent ();

            if (args == null || args.Length == 0 || args [0] == "0")
                BindableReadonlyElement();
            else {
                if (args[0] == "1")
                    BindableReadWriteElement();
                else if (args[0] == "2")
                    DataSetDataBinding ();
                else
                    BindableReadonlyElement ();
            }
        }

        private void BindableReadonlyElement ()
        {
            ArrayList arraylist = new ArrayList ();

            for (int index = 0; index < 10; index++)
                arraylist.Add (new BindableReadonlyElement (index, 
                    string.Format ("Name{0}", index)));

            dataGrid.DataSource = arraylist;
        }

        private void BindableReadWriteElement ()
        {
            ArrayList arraylist = new ArrayList ();

            for (int index = 0; index < 10; index++)
                arraylist.Add (new BindableReadWriteElement (index,
                    string.Format ("Name{0}", index)));

            dataGrid.DataSource = arraylist;
        }

        private void DataSetDataBinding ()
        {
            // FIXME: Port the datagrid.py example here
        }
    }
}
