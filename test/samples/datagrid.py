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

    def Dispose(self, disposing):
        if bool(disposing):
            if components != null:
                components.Dispose()
        base.Dispose(bool(disposing))

    def InitializeComponent(self):
        self.dgdFunctionArea = DataGrid()
        self.buttonFocus = Button()
        self.dgdFunctionArea.BeginInit()
        self.SuspendLayout()

        self.dgdFunctionArea.DataMember = ""
        self.dgdFunctionArea.HeaderForeColor = SystemColors.ControlText

        self.dgdFunctionArea.Location = Point(4, 8)
        self.dgdFunctionArea.Name = "dgdFunctionArea"
        self.dgdFunctionArea.Size = Size(500, 280)
        self.dgdFunctionArea.TabIndex = 0

    
        self.buttonFocus.Location = Point(420, 310)
        self.buttonFocus.Name = "buttonFocus"
        self.buttonFocus.Size = Size(84, 23)
        self.buttonFocus.TabIndex = 1
        self.buttonFocus.Text = "GetFocus"
        self.buttonFocus.Click += self.buttonFocus_Click

        self.Controls.Add(self.buttonFocus)
        self.Controls.Add(self.dgdFunctionArea)

        self.dgdFunctionArea.EndInit()
        self.ResumeLayout(False)


    def PopulateGrid(self):
        #####add four line
        self.dtblFunctionalArea = DataTable("FunctionArea")
        arrstrFunctionalArea = ["Area", "Min", "Max"]
        self.dtCol = DataColumn()
        for i in arrstrFunctionalArea:
            self.dtCol = DataColumn(i)
            self.dtCol.DataType = System.String
            self.dtCol.DefaultValue = ""
            self.dtblFunctionalArea.Columns.Add(self.dtCol)

        #####add boolean line.
        self.dtcCheck = DataColumn("IsMandatory")
        self.dtcCheck.DataType = System.Boolean
        self.dtcCheck.DefaultValue = False
        self.dtblFunctionalArea.Columns.Add(self.dtcCheck)

        #####bound to datagrid
        self.dgdFunctionArea.DataSource = self.dtblFunctionalArea

        #####add new rows
        for i in range(0,6):
            self.newRow1 = self.dtblFunctionalArea.NewRow()
            self.dtblFunctionalArea.Rows.Add(self.newRow1)
        self.dtblFunctionalArea.Rows[0]["Min"] = "0"
        self.dtblFunctionalArea.Rows[1]["Min"] = "1"
        self.dtblFunctionalArea.Rows[2]["Min"] = "2"
        self.dtblFunctionalArea.Rows[3]["Min"] = "3"
        self.dtblFunctionalArea.Rows[4]["Min"] = "4"
        self.dtblFunctionalArea.Rows[5]["Min"] = "5"

        #####add style for datagrid
        if self.dgdFunctionArea.TableStyles.Contains("") != "FunctionArea":
            self.dgdtblStyle = DataGridTableStyle()
            self.dgdtblStyle.MappingName = self.dtblFunctionalArea.TableName
            self.dgdFunctionArea.TableStyles.Add(self.dgdtblStyle)
            self.dgdtblStyle.RowHeadersVisible = True
            self.dgdtblStyle.HeaderBackColor = Color.LightSteelBlue
            self.dgdtblStyle.AllowSorting = False
            self.dgdtblStyle.HeaderBackColor =Color.FromArgb(8, 36, 107)
            self.dgdtblStyle.HeaderForeColor = Color.White
            self.dgdtblStyle.GridLineColor = Color.DarkGray
            self.dgdtblStyle.PreferredRowHeight = 25
            self.dgdFunctionArea.BackgroundColor = Color.White

            self.colStyle = self.dgdFunctionArea.TableStyles[0].GridColumnStyles
            self.colStyle[0].Width = 80
            self.colStyle[1].Width = 80
            self.colStyle[2].Width = 80
            self.colStyle[3].Width = 80

            self.dgtb = self.dgdFunctionArea.TableStyles[0].GridColumnStyles[0]

            self.cmbFunctionArea = ComboBox()
            self.cmbFunctionArea.Items.AddRange(("Item1", "Item2", "Item3"))
            self.cmbFunctionArea.Cursor = Cursors.Arrow
            self.cmbFunctionArea.DropDownStyle = ComboBoxStyle.DropDownList
            self.cmbFunctionArea.Dock = DockStyle.Fill
            self.cmbFunctionArea.SelectionChangeCommitted += self.cmbFunctionArea_SelectionChangeCommitted
            self.dgtb.TextBox.Controls.Add(self.cmbFunctionArea)

    def GetFocus(self, row, col):
        self.dgdFunctionArea.Focus()
        self.dgc = DataGridCell(int(row), int(col))
        self.dgdFunctionArea.CurrentCell = self.dgc
        self.dgtb = self.dgdFunctionArea.TableStyles[0].GridColumnStyles[col]

        self.dgtb.TextBox.Focus()

    def cmbFunctionArea_SelectionChangeCommitted(self, sender, event):
        self.dgdFunctionArea[self.dgdFunctionArea.CurrentCell] = self.cmbFunctionArea.SelectedItem.ToString()

    def buttonFocus_Click(self, sender, event):
        self.GetFocus(1, 0)



form = DataGridApp()
Application.Run(form)
