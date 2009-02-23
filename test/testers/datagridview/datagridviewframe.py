
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
 
    #close application main window after running test
    def quit(self):
        self.altF4()

