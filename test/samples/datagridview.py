#!/usr/bin/env ipy

##############################################################################
# Written by:  Mario Carrion <mcarrion@novell.com>
# Date:        02/23/2009
# Description: This is a test application sample for winforms control:
#              DataGridView
##############################################################################

# The docstring below is used in the generated log file
"""
This sample will show "DataGricView". It can be used for Autotest tools(e.g. Strongwind) to test the behaviors of controls.
"""

import clr

clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')

from System.Drawing import *
from System.Windows.Forms import *
import System

class RunApp(Form):
	"""DataGridView control class"""

	def __init__(self):
		"""RunApp class init function."""

		self.Text = "DataGridView control"
		self.Size = Size(650,300)

		# Set up DataGridView control
		self.datagridview1 = DataGridView()
		self.datagridview1.Name = "datagridview1"
		self.datagridview1.Location = Point(10,10)
		self.datagridview1.Size = Size(600,200)
		self.datagridview1.AllowUserToAddRows = False
		# Set up Columns
		dtgvcboxcolumn = DataGridViewCheckBoxColumn()
		dtgvcboxcolumn.HeaderText = "COLUMN_CHECKBOX"
		dtgvtextboxcolumn = DataGridViewTextBoxColumn()
		dtgvtextboxcolumn.HeaderText = "COLUMN_TEXTBOX"
		dtgvcomboboxcolumn = DataGridViewComboBoxColumn()
		dtgvcomboboxcolumn.HeaderText = "COLUMN_COMBOBOX"		
		combobox_items = ["0", "1", "2", "3", "4", "5", "6", "7", "8", "9"]
		for combobox_item in combobox_items:
			dtgvcomboboxcolumn.Items.Add(combobox_item)
		dtgvbuttoncolumn = DataGridViewButtonColumn()
		dtgvbuttoncolumn.HeaderText = "COLUMN_BUTTON"
		dtgvlinkcolumn = DataGridViewLinkColumn()
		dtgvlinkcolumn.HeaderText = "COLUMN_LINK"
		#dtgvlinkcolumn.Text = "Click here"
		#dtgvlinkcolumn.UseColumnTextForLinkValue = True 

		self.datagridview1.Columns.Add(dtgvcboxcolumn)
		self.datagridview1.Columns.Add(dtgvtextboxcolumn)
		self.datagridview1.Columns.Add(dtgvcomboboxcolumn)
		self.datagridview1.Columns.Add(dtgvbuttoncolumn)
		self.datagridview1.Columns.Add(dtgvlinkcolumn)

		# Even rows = editable
		# Odd rows = not editable
		textbox_items = ["Item0", "Item1", "Item2", "Item3", "Item4", "Item5"]
		checkbox_items = [True, False, True, False, True, False]

		for count in range (6):
			row = DataGridViewRow()
		
			checkboxcell = DataGridViewCheckBoxCell()
			checkboxcell.Value = checkbox_items[count]
			checkboxcell.ReadOnly = checkbox_items[count]
			row.Cells.Add(checkboxcell)
		
			textboxcell = DataGridViewTextBoxCell()
			textboxcell.Value = textbox_items[count]
			textboxcell.ReadOnly = checkbox_items[count]
			row.Cells.Add(textboxcell)
			
			comboboxcell = DataGridViewComboBoxCell()
			comboboxcell.Value = "0"
			for combobox_item in combobox_items:
				comboboxcell.Items.Add(combobox_item)
			comboboxcell.ReadOnly = checkbox_items[count]
			row.Cells.Add(comboboxcell)
			
			buttoncell = DataGridViewButtonCell()
			buttoncell.Value = textbox_items[count]
			row.Cells.Add(buttoncell)
			
			linkcell = DataGridViewLinkCell()
			linkcell.Value = textbox_items[count]
			row.Cells.Add(linkcell)
		
			self.datagridview1.Rows.Add(row)

		# Add DataGridView
		self.Controls.Add(self.datagridview1)
	
form = RunApp()
Application.Run(form)

