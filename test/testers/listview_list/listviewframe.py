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

    def assertText(self, accessible, expected_text):
        """assert that the accessible text is equal to the expected text"""
        procedurelogger.expectedResult('"%s" text is "%s"' % \
                                                (accessible, expected_text))
        assert accessible.text == expected_text, \
                                            'Text was "%s", expected "%s"' % \
                                            (accessible.text, expected_text)

    def selectChild(self, accessible, index):
        """call Strongwind's selectChild(index) on accessible"""
        procedurelogger.action('Select index %s in "%s"' % (index, accessible))
        accessible.selectChild(index)

    def clearSelection(self, accessible):
        """assert ClearSelection implementation"""
        procedurelogger.action('clear selection in "%s"' % (accessible))
        accessible.clearSelection()

    def assertTable(self, accessible, num_rows, num_cols):
        """
        assert Table implementation, make sure row and col's number are expected
        """
        procedurelogger.action('check "%s" Table implemetation' % accessible)
        itable = accessible._accessible.queryTable()
        procedurelogger.expectedResult('"%s" has %s row(s) and %s column(s)' %\
                                              (accessible, num_rows, num_cols))

        assert itable.nRows == num_rows, \
               "%s rows reported, expected %s" % (itable.nRows, num_rows)

        assert itable.nColumns == num_cols, \
               "%s columns reported, expected %s" % (itable.nColumns, num_rows)

    def assignText(self, accessible, text):
        """assign a text to an item"""
        procedurelogger.action("assign %s's text to %s" % (accessible, text))
        accessible.text = text
        
    # close application main window after running test
    def quit(self):
        self.altF4()
