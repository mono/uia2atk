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

	def __init__(self, accessible):
		super(DataGridViewFrame, self).__init__(accessible)
		self.treetable = self.findTreeTable(None)
		self.checkbox_column = self.findTableColumnHeader(self.COLUMN_CHECKBOX, checkShowing=False)
		self.textbox_column = self.findTableColumnHeader(self.COLUMN_TEXTBOX, checkShowing=False)
		self.tablecells = self.findAllTableCells(None)

		self.edits = dict([(x, self.findTableCell("Item" + str(x))) for x in range(6)])
		self.checkboxes = self.findAllCheckBoxes(None)

	#assert Edits' Text implementation for ListView Items
	def assertEditsText(self, accessible):
		procedurelogger.action("check DataGridView Items Text Value")

		items = ["Item0", "Item1", "Item2", "Item3", "Item4", "Item5"]
		for index in range(6):
			procedurelogger.expectedResult('the text[%s] is %s' % (index,items[index]))
			new_value = 'Item%s' % (index + 10)
			accessible[index].text = new_value
			sleep(config.SHORT_DELAY) # We need this because sometimes the assert fails

			if index % 2 == 0: # Not Editable
				assert accessible[index].text == items[index]
			else: # Editable
				assert accessible[index].text == new_value

	#assert Selection implementation
	def assertSelectionChild(self, accessible, childIndex):
		procedurelogger.action('selected childIndex %s in "%s"' % (childIndex, accessible))
		accessible.selectChild(childIndex)

	def assertClearSelection(self, accessible):
		procedurelogger.action('clear selection in "%s"' % (accessible))
		accessible.clearSelection()

	def assertTable(self, accessible, row=0, col=0):
		procedurelogger.action('check "%s" Table implemetation' % accessible)
		itable = accessible._accessible.queryTable()

		procedurelogger.expectedResult('"%s" have %s Rows and %s Columns' % (accessible, row, col))
		assert itable.nRows == row and itable.nColumns == col, "Not match Rows %s and Columns %s" % (itable.nRows, itable.nColumns)
 
	#close application main window after running test
	def quit(self):
		self.altF4()

