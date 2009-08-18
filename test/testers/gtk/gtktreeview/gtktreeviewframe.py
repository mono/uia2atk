
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        06/18/2008
# Description: gtktreeview.py wrapper script
#              Used by the gtktreeview-*.py tests
##############################################################################$

import sys
import os
 
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
 
    # the ordering of table cell names we expect when we call the
    # findAllTableCells method
    ASCENDING = ('parent 0', 'parent 1', 'parent 2', 'parent 3',
                 'child 0 of parent 0', 'child 1 of parent 0',
                 'child 2 of parent 0', 'child 0 of parent 1',
                 'child 1 of parent 1', 'child 2 of parent 1',
                 'child 0 of parent 2', 'child 1 of parent 2',
                 'child 2 of parent 2', 'child 0 of parent 3',
                 'child 1 of parent 3', 'child 2 of parent 3')
    DESCENDING = ('parent 3', 'parent 2', 'parent 1', 'parent 0',
                  'child 2 of parent 3', 'child 1 of parent 3',
                  'child 0 of parent 3', 'child 2 of parent 2',
                  'child 1 of parent 2', 'child 0 of parent 2',
                  'child 2 of parent 1', 'child 1 of parent 1',
                  'child 0 of parent 1', 'child 2 of parent 0',
                  'child 1 of parent 0', 'child 0 of parent 0')
 
    # Use the constructor to find some of the accessibles we will be testing
    # and store an object for each accessible in a variable that can be used
    # from the test script.
    def __init__(self, accessible):
        super(GtkTreeViewFrame, self).__init__(accessible)
        self.column0 = self.findTableColumnHeader(self.COLUMN_ZERO)
        self.parent0 = self.findTableCell(self.PARENT_ONE)
        self.parent1 = self.findTableCell(self.PARENT_TWO)
        self.parent2 = self.findTableCell(self.PARENT_TREE)
        self.parent3 = self.findTableCell(self.PARENT_FOUR)
 
    # expand accessible by performing an "expand or contract" action.
    # this method will also contract the accessible if used out of order
    def expand(self, parent):
        procedurelogger.action('expand %s.' % parent)
        parent.expandOrContract()
 
    # contract accessible by performing an "expand or contract" action.
    # this method will also expand the accessible if used out of order
    def contract(self, parent):
        procedurelogger.action('contract %s.' % parent)
        parent.expandOrContract()
 
    # perform a click action for TableColumnHeader
    def tchClick (self, thc):
        procedurelogger.action('click %s.' % thc)
        thc.click()
 
    # assert that the accessible has the "contracted" state after performing
    # the "expand or contract" action.  states can be found on the "status" list
    # on the "interface viewer" tab in Accerciser.
    def assertContracted(self, accessible):
        'Raise exception if accessible does not match the given result'   
        procedurelogger.expectedResult('%s is %s.' % (accessible, "contracted"))
        def resultMatches():
            return not accessible.expanded
        
        assert retryUntilTrue(resultMatches)
 
    # assert that the accessible has the "expanded" state after performing
    # the "expand or contract" action.
    def assertExpanded(self, accessible):
        'Raise exception if accessible does not match the given result'   
        procedurelogger.expectedResult('%s is %s.' % (accessible, "expanded"))
        def resultMatches():
            return accessible.expanded
        
        assert retryUntilTrue(resultMatches)
 
    # assert that the sorting of the TreeView is ascending by comparing the 
    # order in which the table cells are found to the order we expect them
    # to be in (self.ASCENDING).
    def assertAscending(self):
        'Raise exception if the sorting of the tree view is not ascending'   
        procedurelogger.expectedResult('TreeView sorting is ascending')
        self.table_cells = self.findAllTableCells(None, checkShowing=False)
        tcs = [table_cell.name for table_cell in  self.table_cells]
 
        def resultMatches():
            self.table_cells = self.findAllTableCells(None, checkShowing=False)
            return tuple(tcs) == self.ASCENDING
 
        assert retryUntilTrue(resultMatches)
 
    # assert that the sorting of the TreeView is descending by comparing the 
    # order in which the table cells are found to the order we expect them
    # to be in (self.DESCENDING).
    def assertDescending(self):
        'Raise exception if the sorting of the tree view is not descending'   
        procedurelogger.expectedResult('TreeView sorting is descending')
        self.table_cells = self.findAllTableCells(None, checkShowing=False)
        tcs = [table_cell.name for table_cell in  self.table_cells]
 
        def resultMatches():
            self.table_cells = self.findAllTableCells(None, checkShowing=False)
            return tuple(tcs) == self.DESCENDING
 
        assert retryUntilTrue(resultMatches)
