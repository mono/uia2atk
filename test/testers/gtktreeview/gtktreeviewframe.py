
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        06/18/2008
# Description: gtktreeview.py wrapper script
#              Used by the gtktreeview-*.py tests
##############################################################################$

import sys
import os
import gtkactions as a

from strongwind import *
from gtktreeview import *


# class to represent the main window.
class GtkTreeViewFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    COLUMN_ZERO = "Column 0"
    PARENT_ONE = "parent 0"
    PARENT_TWO = "parent 1"
    PARENT_TREE = "parent 2"
    PARENT_FOUR = "parent 3"

    def __init__(self, accessible):
        super(GtkTreeViewFrame, self).__init__(accessible)
        self.column0 = self.findTableColumnHeader(self.COLUMN_ZERO)
        self.parent0 = self.findTableCell(self.PARENT_ONE)
        self.parent1 = self.findTableCell(self.PARENT_TWO)
        self.parent2 = self.findTableCell(self.PARENT_TREE)
        self.parent3 = self.findTableCell(self.PARENT_FOUR)

    # set expand to send "expand or contract" action. now i know why my
    # returning couldn't be divided regularly, because strongwind haven't set
    # this action, so we need define it with procedurelogger.action to return
    # "Action:" info.
    def expand(self, treeview):
        procedurelogger.action('expand %s.' % treeview)
        treeview._doAction(a.TreeView.EXPAND_OR_CONTRACT)

    #set contract to send "expand or contract" action. same as expend
    def contract(self, treeview):
        procedurelogger.action('contract %s.' % treeview)
        treeview._doAction(a.TreeView.EXPAND_OR_CONTRACT)

    #set Click action for TableColumnHeader
    def tchClick (self,test):
        procedurelogger.action('click %s.' % test)
        treeview = self.findTableColumnHeader("%s" % test)
        treeview._doAction(a.TableColumnHeader.CLICK)

    #check if status list in "interface viewer" in accerciser have "expanded" 
    #status when doing expand or contract action.
    def assertContracted(self, accessible):
        'Raise exception if accessible does not match the given result'   
	procedurelogger.expectedResult('%s is %s.' % (accessible, "contracted"))
        def resultMatches():
            return not accessible.expanded
	
        assert retryUntilTrue(resultMatches)

    def assertExpanded(self, accessible):
        'Raise exception if accessible does not match the given result'   
	procedurelogger.expectedResult('%s is %s.' % (accessible, "expanded"))
        def resultMatches():
            return accessible.expanded
	
        assert retryUntilTrue(resultMatches)


class InvalidState(Exception):
  pass

class InvalidAccessible(Exception):
  pass

