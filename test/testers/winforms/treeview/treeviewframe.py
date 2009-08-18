##############################################################################
# Written by:  Brian G. Merrell <bgmerrell@novell.com>
# Date:        02/12/2009
# Description: treeview.py wrapper script
#              Used by the treeview-*.py tests
##############################################################################

import sys
import os
import re
import actions
import states
import pyatspi

from strongwind import *
from treeview import *


# class to represent the main window.
class TreeViewFrame(accessibles.Frame):

    # constants
    PARENT1 = "Parent 1"
    PARENT2 = "Parent 2"
    CHILD1 = "Child 1"
    CHILD2 = "Child 2"
    CHILD3 = "Child 3"
    GRANDCHILD = "Grandchild"
    GREATGRANDCHILD = "Great Grandchild"
    

    def __init__(self, accessible):
        super(TreeViewFrame, self).__init__(accessible)
        # find the tree table
        self.tree_table = self.findTreeTable("")
        # the rest of the controls should be children of the tree table
        self.parent1 = self.tree_table.findTableCell(self.PARENT1)
        self.parent2 = self.tree_table.findTableCell(self.PARENT2)
        self.child1 = self.tree_table.findTableCell(self.CHILD1,
                                                    checkShowing=False)
        self.child2 = self.tree_table.findTableCell(self.CHILD2,
                                                    checkShowing=False)
        self.child3 = self.tree_table.findTableCell(self.CHILD3,
                                                    checkShowing=False)
        self.grandchild = self.tree_table.findTableCell(self.GRANDCHILD,
                                                        checkShowing=False)
        self.great_grandchild = \
                        self.tree_table.findTableCell(self.GREATGRANDCHILD,
                                                      checkShowing=False)

    def quit(self):
        self.altF4()
