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
        self.Size = Size(400, 350)

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

        self.datagrid.Location = Point(10, 50)
        self.datagrid.Text = "datagrid"
        self.datagrid.Size = Size(350, 200)
    
        self.buttonFocus.Location = Point(300, 280)
        self.buttonFocus.Name = "buttonFocus"
        self.buttonFocus.TabIndex = 1
        self.buttonFocus.Text = "GetFocus"
        self.buttonFocus.Click += self.buttonFocus_Click

        self.Controls.Add(self.buttonFocus)
        self.Controls.Add(self.datagrid)

        self.datagrid.EndInit()
        self.ResumeLayout(False)

    def PopulateGrid(self):
        #add datatable
        self.datatable = DataTable("DataTable")

        #add textbox column
        self.datatable.Columns.Add(DataColumn("TextBoxColumn"))

        #add bool column
        self.boolcolumn = DataColumn("BoolColumn")
        self.boolcolumn.DefaultValue = False
        self.datatable.Columns.Add(self.boolcolumn)

        #add style for datagrid
        self.tablestyle = DataGridTableStyle()
        self.tablestyle.MappingName = "DataTable"

        self.textcolumnstyle = DataGridTextBoxColumn()
        self.textcolumnstyle.MappingName = "TextBoxColumn"
        self.textcolumnstyle.HeaderText = "TextBoxColumn"
        self.textcolumnstyle.Width = 150
        self.tablestyle.GridColumnStyles.Add(self.textcolumnstyle)

        self.boolcolumnstyle = DataGridBoolColumn()
        self.boolcolumnstyle.MappingName = "BoolColumn"
        self.boolcolumnstyle.HeaderText = "BoolColumn"
        self.boolcolumnstyle.Width = 150	

        self.tablestyle.GridColumnStyles.Add(self.boolcolumnstyle)

        self.datagrid.TableStyles.Add(self.tablestyle)


        #####add new rows
        for i in range(0,4):
            self.newRow1 = self.datatable.NewRow()
            self.datatable.Rows.Add(self.newRow1)
        self.datatable.Rows[0]["TextBoxColumn"] = "Item0"
        self.datatable.Rows[1]["TextBoxColumn"] = "Item1"
        self.datatable.Rows[2]["TextBoxColumn"] = "Item2"
        self.datatable.Rows[3]["TextBoxColumn"] = "Item3"

    def GetFocus(self, row, col):
        self.datagrid.Focus()
        self.dgc = DataGridCell(int(row), int(col))
        self.datagrid.CurrentCell = self.dgc
        self.dgtb = self.datagrid.TableStyles[0].GridColumnStyles[col]

        self.dgtb.TextBox.Focus()

    def buttonFocus_Click(self, sender, event):
        self.GetFocus(1, 0)


form = DataGridApp()
Application.Run(form)
