
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        06/18/2008
# Description: treeview.py wrapper script
#              Used by the checkbutton-*.py tests
##############################################################################$

import sys
import os

from strongwind import *
from treeview import *

class TableColumnHeader(Accessible):
    def click(self, log=True):
        'click TableColumnHeader'
        
        if log:
            procedurelogger.action('click %s.' % self, self)

        self.grabFocus()
        

# class to represent the main window.
class TreeViewFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    COLUMN_ZEO = "Column 0"
    PARENT_ONE = "parent 0"
    PARENT_TWO = "parent 1"
    PARENT_TREE = "parent 2"
    PARENT_FOUR = "parent 3"
    #CHILD_ONE = "child 0 of parent 3"


    # available results for the treeview
    RESULT_UNCHECKED = "unchecked"
    RESULT_CHECKED = "checked"
    # end constants

    logName = 'TreeView'

    def __init__(self, accessible):
        super(TreeViewFrame, self).__init__(accessible)
        self.column0 = self.findTableColumnHeader(self.COLUMN_ZEO)
        self.parent0 = self.findTableCell(self.PARENT_ONE)
        self.parent1 = self.findTableCell(self.PARENT_TWO)
        self.parent2 = self.findTableCell(self.PARENT_TREE)
        self.parent3 = self.findTableCell(self.PARENT_FOUR)
        #self.child1 = self.findTableCell(self.CHILD_ONE)

#set expand to send "expand or contract" action. now i know why my returning couldn't be divided regularly, because strongwind haven't set this action, so we need define it with procedurelogger.action to return "Action:" info.
    def expand(self, treeview, action, log=True):
        treeview._doAction(action)
        if log:
            procedurelogger.action('expand %s.' % treeview)
        self.grabFocus()

#set contract to send "expand or contract" action. same as expend
    def contract(self, treeview, action, log=True):
        treeview._doAction(action)
        if log:
            procedurelogger.action('contract %s.' % treeview)
        self.grabFocus()

#set Click action for TableColumnHeader
    def tchClick (self,test,log=True):
        #treeview._doAction(action)
        treeview = self.findTableColumnHeader("%s" % test)
        treeview._doAction('click')
        if log:
            procedurelogger.action('click %s.' % test)
        self.grabFocus()

#check if status list in "interface viewer" in accerciser have "expanded" status when doing expand or contract action.
    def assertResult(self, treeview, result):
        'Raise exception if the treeview does not match the given result'   
        procedurelogger.expectedResult('%s is %s.' % (treeview, result))

        def resultMatches():

            if result == "expanded":
                return treeview.expanded
            elif result == "contracted":
                return not treeview.expanded
            else:
                raise InvalidState, "%s has no such state:  %s" %\
                                 (treeview, result)
	
        assert retryUntilTrue(resultMatches)


class InvalidState(Exception):
  pass

class InvalidAccessible(Exception):
  pass

