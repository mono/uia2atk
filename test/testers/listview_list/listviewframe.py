# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
#              Ray Wang <rawang@novell.com>
# Date:        12/13/2008
# Description: Application wrapper for listview_list.py
#              be called by ../listview_list_basic_ops.py
##############################################################################

"""Application wrapper for listview_list.py"""

from strongwind import *

class ListViewFrame(accessibles.Frame):
    """the profile of the listview_list sample"""

    
    CHECKBOX = "MultiSelect"

    def __init__(self, accessible):
        super(ListViewFrame, self).__init__(accessible)
        self.checkbox = self.findCheckBox(self.CHECKBOX)
        self.treetable = self.findTreeTable(None)
        self.tablecell = dict([(x, self.findTableCell("Item " + str(x))) for x in range(5)]) 

    def click(self, tablecell):
        """'click' action"""
        procedurelogger.action('Do click action on %s' % tablecell)
        tablecell.click()

    def assertText(self, accessible, text):
        """assert Text implementation for ListItem role"""
        procedurelogger.action("Assert the text of %s" % accessible)
        procedurelogger.expectedResult('%s text is %s' % (accessible, text))
        assert accessible.text == text, '%s text is not match with "%s"' % \
                                                (accessible, accessible.text)

    def selectChild(self, accessible, childIndex):
        """assert Selection implementation"""
        procedurelogger.action("Select childIndex %s from %s" % \
                                                    (childIndex, accessible))
        accessible.selectChild(childIndex)

    def assertClearSelection(self, accessible):
        """assert ClearSelection implementation"""
        procedurelogger.action('clear selection in "%s"' % (accessible))
        accessible.clearSelection()

    def assertTable(self, accessible, row=None, col=None):
        """assert Table implementation"""
        procedurelogger.action('check "%s" Table implemetation' % accessible)
        itable = accessible._accessible.queryTable()
        procedurelogger.expectedResult('"%s" have %s Rows and %s Columns' % \
                                                        (accessible, row, col))
        assert itable.nRows == row and itable.nColumns == col, \
                                        "Not match Rows %s and Columns %s" % \
                                        (itable.nRows, itable.nColumns)
    
    def inputText(self, accessible, text):
        """input text to change item's Text"""
        procedurelogger.action('input %s in "%s"' % (text, accessible))
        accessible.text = text
        
    # close application main window after running test
    def quit(self):
        self.altF4()
