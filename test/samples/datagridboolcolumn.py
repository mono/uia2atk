#!/usr/bin/env ipy

##############################################################################
# Written by:  Calen Chen <cachen@novell.com>
# Date:        09/10/2008
# Description: This is a test application sample for winforms control:
#              DataGrid
##############################################################################

import clr
import System

clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')
clr.AddReference('System.Data')

from System.Drawing import *
from System.Windows.Forms import *
from System.Data import *


class DataGridApp(Form):

    def __init__(self):
        self.Text = "DataGridBoolColumn control"
        self.Size = Size(330, 230)

        self.InitializeComponent()
        self.PopulateGrid()

        self.datagrid.DataSource = self.datatable

    def InitializeComponent(self):
        self.datagrid = DataGrid()
        self.buttonFocus = Button()
        self.datagrid.BeginInit()
        self.SuspendLayout()

        self.datagrid.DataMember = ""
        self.datagrid.HeaderForeColor = SystemColors.ControlText

        self.datagrid.Location = Point(10, 30)
        self.datagrid.Text = "datagrid"
        self.datagrid.Size = Size(300, 150)

        self.label = Label()
        self.label.Text = "CurrentCell:"
        self.label.Location = Point(10, 10)
        self.label.AutoSize = True

        self.Controls.Add(self.datagrid)
        self.Controls.Add(self.label)

        self.datagrid.EndInit()
        self.ResumeLayout(False)

    def PopulateGrid(self):
        #add datatable
        self.datatable = DataTable("DataTable")

        #add bool column
        self.boolcolumn = DataColumn("BoolColumn")
        self.boolcolumn.DataType = System.Boolean
        self.boolcolumn.DefaultValue = None
        self.datatable.Columns.Add(self.boolcolumn)

        #add style for datagrid
        self.tablestyle = DataGridTableStyle()
        self.tablestyle.MappingName = "DataTable"

        self.boolcolumnstyle = DataGridBoolColumn()
        self.boolcolumnstyle.MappingName = "BoolColumn"
        self.boolcolumnstyle.HeaderText = "BoolColumn"
        self.boolcolumnstyle.Width = 120
        self.tablestyle.GridColumnStyles.Add(self.boolcolumnstyle)

        self.datagrid.TableStyles.Add(self.tablestyle)

        #####add new rows
        for i in range(0,4):
            self.newRow1 = self.datatable.NewRow()
            self.datatable.Rows.Add(self.newRow1)
        self.datatable.Rows[1][0] = True
        self.datatable.Rows[2][0] = False

        #self.datatable.AcceptChanges()

        self.datagrid.CurrentCellChanged += self.datagrid_currencellchanged

    def datagrid_currencellchanged(self, sender, event):
        cell = self.datagrid.CurrentCell
        bool_value = self.datatable.Rows[cell.RowNumber][cell.ColumnNumber]

        self.label.Text = "row:%s col:%s Value:%s" % (cell.RowNumber, cell.ColumnNumber, bool_value)


form = DataGridApp()
Application.Run(form)
