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
        self.label = self.findLabel(None)
        self.checkbox = self.findCheckBox(self.CHECKBOX)
        self.treetable = self.findTreeTable(None)
        self.tablecell = dict([(x, self.findTableCell("Item " + str(x))) for x in range(5)]) 

    def click(self, tablecell):
        """'click' action"""
        procedurelogger.action('Do click action on %s' % tablecell)
        tablecell.click()

    def assertText(self, accessible, text):
        """assert Text implementation for TableCell's role"""
        procedurelogger.action("Assert the text of %s" % accessible)
        procedurelogger.expectedResult('%s text is %s' % (accessible, text))
        assert accessible.text == text, '%s text is not match with "%s"' % \
                                                (accessible, accessible.text)

    def selectChild(self, accessible, childIndex):
        """assert Selection implementation"""
        procedurelogger.action('Select child at index %s in "%s"' % \
                                                    (childIndex, accessible))
        accessible.selectChild(childIndex)
        sleep(config.SHORT_DELAY)

        procedurelogger.expectedResult("child at index %s is selected" % childIndex)
        assert accessible.getChildAtIndex(childIndex).selected

    def clearSelection(self, accessible):
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
    
    def changeText(self, accessible, text):
        """change item's Text"""
        procedurelogger.action("change %s's text to %s" % (accessible, text))
        accessible.text = text
        
    # close application main window after running test
    def quit(self):
        self.altF4()
