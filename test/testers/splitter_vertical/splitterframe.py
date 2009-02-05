##############################################################################
# Written by:  Brian G. Merrell <bgmerrell@novell.com>
# Date:        01/26/2009
# Description: splitter.py wrapper script
#              Used by the splitter-*.py tests
##############################################################################

import sys
import os
import actions
import states

from strongwind import *
from splitter_vertical import *


# class to represent the main window.
class SplitterFrame(accessibles.Frame):

    # Contants
    NODE0 = "TreeView Node"
    NODE1 = "Another Node"
    RIGHT_BUTTON = "Right Side"

    def __init__(self, accessible):
        super(SplitterFrame, self).__init__(accessible)
        self.tree_table = self.findTreeTable("")
        self.table_cell0 = self.findTableCell(self.NODE0)
        self.tabel_cell1 = self.findTableCell(self.NODE1)
        self.tree_view_scrollbar = self.tree_table.findScrollBar("")
        self.push_button = self.findButton(self.RIGHT_BUTTON)

    #close Splitter window
    def quit(self):
        self.altF4()
