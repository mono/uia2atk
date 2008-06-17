# -*- coding: utf-8 -*-

##############################################################################$
# Written by:  Brian G. Merrell <bgmerrell@novell.com>$
# Date:        May 23 2008$
# Description: checkButton.py wrapper script
#              Used by the checkbutton-*.py tests
##############################################################################$

import sys
import os

from strongwind import *
from treeview import *


# class to represent the main window.
class TreeViewFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    COLUMN_ZEO = "Column 0"
    PARENT_ONE = "parent 0"
    PARENT_TWO = "parent 1"
    PARENT_TREE = "parent 2"
    PARENT_FOUR = "parent 3"

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

    #check RoleName
    def checkRoleName(self, treeview, result):
        'Raise exception if the treeview does not match the given result'  
        procedurelogger.expectedResult('\"%s\" RoleName is \"%s\".' % (treeview,result))

        def resultMatches():
            if result == treeview._accessible.getRoleName():
                return treeview._accessible.getRoleName()

            if result !=treeview._accessible.getRoleName():  
                return not treeview._accessible.getRoleName()

        try:
            assert retryUntilTrue(resultMatches)
        except AssertionError:
            print "error:", '\"%s\" RoleName is not \"%s\" but \"%s\".' % (treeview, result, treeview._accessible.getRoleName())

    def assertResult(self, treeview, result):
        'Raise exception if the treeview does not match the given result'   
        procedurelogger.expectedResult('%s is %s.' % (treeview, result))

        def resultMatches():
            if result == "activate" or "click":
                return treeview.activate or treeview.click
            elif result == "unactivate" or "unclick":
                return not treeview.unactivate or treeview.unclick
            else:
                raise InvalidState, "%s has no such state:  %s" %\
                                 (treeview, result)
	
        assert retryUntilTrue(resultMatches)


class InvalidState(Exception):
  pass

class InvalidAccessible(Exception):
  pass

