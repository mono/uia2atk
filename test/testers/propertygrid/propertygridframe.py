##############################################################################
# Written by:  Andres G. Aragoneses <aaragoneses@novell.com>
# Date:        02/24/2009
# Description: propertygridframe.py wrapper script
#              Used by the propertygrid-*.py tests
##############################################################################$

import sys
import os
import actions
import states

from strongwind import *
from datagridview import *

# class to represent the main window.
class PropertyGridFrame(accessibles.Frame):

	# constants
	# the available widgets on the frame
	CELL_A11Y = "Accessibility"
	CELL_ACCESSIBLE_DESC = "AccessibleDescription"
	CELL_VISIBLE = "Visible"

	def __init__(self, accessible):
		super(PropertyGridFrame, self).__init__(accessible)
                self.toolbar = self.findToolBar(None)
		self.treetable = self.findTreeTable(None)
                self.propertygrid = self.findPanel("Property Grid")
                self.subpanels = self.propertygrid.findAllPanels(None)

                #maybe this check is useless because the first panel just holds the treetable
                #so change it to "1" if this changes in the future:
                assert len(self.subpanels) = 2

                self.textpanel = self.subpanels [1]

                #enable the line above when bug 479113 is fixed:
		#self.a11ycell = self.findTableCell(self.CELL_A11Y, checkShowing=False)

                #check for relations of accessibledesccell when bug 479142 is fixed
		self.accessibledesccell = self.findTableCell(self.CELL_ACCESSIBLE_DESC, checkShowing=False)
		self.visible = self.findTableCell(self.CELL_VISIBLE, checkShowing=False)

		self.tablecells = self.findAllTableCells(None)
                self.togglebuttons = self.toolbar.findAllToggleButtons(None)

                self.categorizedbutton = self.togglebuttons[1]
                self.alphabeticbutton = self.togglebuttons[2]


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

