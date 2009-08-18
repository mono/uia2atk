##############################################################################
# Written by:  Mario Carrion <mcarrion@novell.com>
# Date:        02/23/2009
# Description: datagridviewframe.py wrapper script
#              Used by the datagridview-*.py tests
##############################################################################$

import sys
import os
import actions
import states

from strongwind import *
from datagridview import *

# class to represent the main window.
class DataGridViewFrame(accessibles.Frame):

	# constants
	# the available widgets on the frame
	COLUMN_CHECKBOX = "COLUMN_CHECKBOX"
	COLUMN_TEXTBOX = "COLUMN_TEXTBOX"
	COLUMN_COMBOBOX = "COLUMN_COMBOBOX"
	COLUMN_BUTTON = "COLUMN_BUTTON"
	COLUMN_LINK = "COLUMN_LINK"

	def __init__(self, accessible):
		super(DataGridViewFrame, self).__init__(accessible)
		self.treetable = self.findTreeTable(None)
		self.checkbox_column = self.findTableColumnHeader(self.COLUMN_CHECKBOX, checkShowing=False)
		self.textbox_column = self.findTableColumnHeader(self.COLUMN_TEXTBOX, checkShowing=False)
		self.combobox_column = self.findTableColumnHeader(self.COLUMN_TEXTBOX ,checkShowing=False)
		self.button_column = self.findTableColumnHeader(self.COLUMN_BUTTON , checkShowing=False)
		self.link_column = self.findTableColumnHeader(self.COLUMN_LINK , checkShowing=False)
		
		self.columns = [self.checkbox_column, self.textbox_column, self.combobox_column, self.button_column, self.link_column ]
		
		self.tablecells = self.findAllTableCells(None)

		self.edits = self.findAllTableCells(re.compile("Item*"))
		self.checkboxes = self.findAllCheckBoxes(None)
		self.buttons = self.findAllPushButtons(re.compile("Item*"))
		self.comboboxes = self.findAllComboBoxes(None)
		self.labels = self.findAllLabels(re.compile("Item*"))
		
		self.label_cellclick = self.findLabel(re.compile("^CellClick(.*)$"))
		self.label_currentcellchanged = self.findLabel(re.compile("^CurrentCell(.*)$"))

	#assert Edits' Text implementation for ListView Items
	def assertEditsText(self, accessible):
		procedurelogger.action("check DataGridView Items Text Value")
		
		index = 0
		for item in accessible:
			actual_states = item._accessible.getState().getStates()
			actual_states = [pyatspi.stateToString(s) for s in actual_states]

			new_value = 'Item%s' % (index + 10)
			old_value = item.text
			
			item.text = new_value
			sleep(config.SHORT_DELAY) # We need this because sometimes the assert fails

			if "editable" in actual_states:
				procedurelogger.expectedResult('editable: current %s new %s' % (item.text,new_value))
				assert item.text == new_value
			else:
				procedurelogger.expectedResult('not-editable: current %s new %s' % (item.text,old_value))
				assert item.text == old_value
				
			index += 1

	#assert Selection implementation
#	def assertSelectionChild(self, accessible, childIndex):
#		procedurelogger.action('selected childIndex %s in "%s"' % (childIndex, accessible))
#		accessible.selectChild(childIndex)

	def assertClearSelection(self, accessible):
		procedurelogger.action('clear selection in "%s"' % (accessible))
		accessible.clearSelection()

	def assertTable(self, accessible, row=0, col=0):
		procedurelogger.action('check "%s" Table implementation' % accessible)
		itable = accessible._accessible.queryTable()

		procedurelogger.expectedResult('"%s" have %s Rows and %s Columns' % (accessible, row, col))
		assert itable.nRows == row and itable.nColumns == col, "Not match Rows %s and Columns %s" % (itable.nRows, itable.nColumns)
	
	#assert Cell Click Value
	def assertCellClickValue(self, row, column):
		new_labelcellclick_value = "CellClick: %s,%s" % (row,column)
		procedurelogger.action('assert label \"cell click \" \'s value ')

	        procedurelogger.expectedResult('\"cell click \" change to %s, %s' % (row, column))
		assert self.label_cellclick.text == new_labelcellclick_value
 
	#assert Current Cell Value
	def assertCurrentCellValue(self, row, column):
		new_labelcurrentcell_value = "CurrenCellChanged: %s,%s" % (row,column)
		procedurelogger.action('assert label \"Current cell \" \'s value ')

	        procedurelogger.expectedResult('\"Current cell \" change to %s, %s' % (row, column))
		assert self.label_currentcellchanged.text == new_labelcurrentcell_value 
 
	#close application main window after running test
	def quit(self):
		self.altF4()

