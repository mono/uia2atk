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
        self.Text = "DataGrid control"
        self.AutoScaleBaseSize = Size(6, 14)
        self.ClientSize = Size(600, 350)
        self.Name = "DataGrid"

        self.InitializeComponent()
        self.PopulateGrid()

    #def Dispose(self, disposing):
    #    if bool(disposing):
    #        if components != null:
    #            components.Dispose()
    #    base.Dispose(bool(disposing))

    def InitializeComponent(self):
        self.datagrid = DataGrid()
        self.buttonFocus = Button()
        self.datagrid.BeginInit()
        self.SuspendLayout()

        self.datagrid.DataMember = ""
        self.datagrid.HeaderForeColor = SystemColors.ControlText

        self.datagrid.Location = Point(10, 50)
        self.datagrid.Name = "datagrid"
        self.datagrid.Size = Size(550, 230)
        self.datagrid.TabIndex = 0

        self.label = Label()
        self.label.Text = "CurrentCell:"
        self.label.Location = Point(10, 10)
        self.label.AutoSize = True

        self.Controls.Add(self.datagrid)
        self.Controls.Add(self.label)

        self.datagrid.EndInit()
        self.ResumeLayout(False)


    def PopulateGrid(self):

        self.datatable = DataTable("DataTable")

        #####add boolean column.
        self.boolcolumn = DataColumn("BoolColumn")
        self.boolcolumn.DataType = System.Boolean
        self.boolcolumn.DefaultValue = None
        self.datatable.Columns.Add(self.boolcolumn)

        #####add 2 textbox column and 1 combobox column
        columns = ["TextBox_Edit", "TextBox_Read", "ComboBox"]
        self.textcolumn = DataColumn()
        for i in columns:
            self.textcolumn = DataColumn(i)
            self.textcolumn.DataType = System.String
            self.textcolumn.DefaultValue = ""
            self.datatable.Columns.Add(self.textcolumn)

        #####bound to datagrid
        self.datagrid.DataSource = self.datatable

        #####add new rows
        for i in range(0,3):
            self.newRow1 = self.datatable.NewRow()
            self.datatable.Rows.Add(self.newRow1)
        self.datatable.Rows[0]["TextBox_Edit"] = "Edit0"
        self.datatable.Rows[1]["TextBox_Edit"] = "Edit1"
        self.datatable.Rows[2]["TextBox_Edit"] = "Edit2"

        self.datatable.Rows[0]["TextBox_Read"] = "Read0"
        self.datatable.Rows[1]["TextBox_Read"] = "Read1"
        self.datatable.Rows[2]["TextBox_Read"] = "Read2"

        self.datatable.Rows[0]["ComboBox"] = "Box0"
        self.datatable.Rows[1]["ComboBox"] = "Box1"
        self.datatable.Rows[2]["ComboBox"] = "Box2"

        self.datatable.Rows[1]["BoolColumn"] = True
        self.datatable.Rows[2]["BoolColumn"] = False

        #add style for datagrid
        self.tablestyle = DataGridTableStyle()
        self.tablestyle.MappingName = "DataTable"

        self.boolcolumnstyle = DataGridBoolColumn()
        self.boolcolumnstyle.MappingName = "BoolColumn"
        self.boolcolumnstyle.HeaderText = "BoolColumn"
        self.boolcolumnstyle.Width = 120
        self.tablestyle.GridColumnStyles.Add(self.boolcolumnstyle)

        self.textcolumnstyle = DataGridTextBoxColumn()
        self.textcolumnstyle.MappingName = "TextBox_Edit"
        self.textcolumnstyle.HeaderText = "TextBox_Edit"
        self.textcolumnstyle.Width = 120
        self.tablestyle.GridColumnStyles.Add(self.textcolumnstyle)

        self.textcolumnstyle1 = DataGridTextBoxColumn()
        self.textcolumnstyle1.MappingName = "TextBox_Read"
        self.textcolumnstyle1.HeaderText = "TextBox_Read"
        self.textcolumnstyle1.Width = 120
        self.textcolumnstyle1.ReadOnly = True
        self.tablestyle.GridColumnStyles.Add(self.textcolumnstyle1)

        self.datagrid.TableStyles.Add(self.tablestyle)

        self.dgtb = self.datagrid.TableStyles[0].GridColumnStyles[3]
        self.comboboxstyle = ComboBox()
        self.comboboxstyle.Items.AddRange(("Item1", "Item2", "Item3"))
        self.comboboxstyle.Cursor = Cursors.Arrow
        self.comboboxstyle.DropDownStyle = ComboBoxStyle.DropDownList
        self.comboboxstyle.Dock = DockStyle.Fill
        self.comboboxstyle.SelectionChangeCommitted += self.comboboxstyle_SelectionChangeCommitted
        self.dgtb.TextBox.Controls.Add(self.comboboxstyle)

        self.datagrid.CurrentCellChanged += self.datagrid_currencellchanged

    def datagrid_currencellchanged(self, sender, event):
        cell = self.datagrid.CurrentCell
        bool_value = self.datatable.Rows[cell.RowNumber][cell.ColumnNumber]

        self.label.Text = "row:%s col:%s Value:%s" % (cell.RowNumber, cell.ColumnNumber, bool_value)

    def comboboxstyle_SelectionChangeCommitted(self, sender, event):
        self.datagrid[self.datagrid.CurrentCell] = self.comboboxstyle.SelectedItem.ToString()

form = DataGridApp()
Application.Run(form)
