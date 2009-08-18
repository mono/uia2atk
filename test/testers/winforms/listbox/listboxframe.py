# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        08/19/2008
# Description: Application wrapper for listbox.py
#              be called by ../listbox_basic_ops.py
##############################################################################

"""Application wrapper for listbox.py"""

from strongwind import *
from helpers import *

# class to represent the main window.
class ListBoxFrame(accessibles.Frame):

    # the available widgets on the window
    LABEL = "You select "
    CHECKBOX = "SelectedIndexChanged Event"

    def __init__(self, accessible):
        super(ListBoxFrame, self).__init__(accessible)
        self.checkbox = self.findCheckBox(self.CHECKBOX)
        self.label = self.findLabel(self.LABEL)
        self.treetable = self.findTreeTable(None)
        self.tablecell = [self.findTableCell(str(x)) for x in range(20)]            
    def assertLabel(self, itemname):
        """Raise exception if the accessible does not match the given result"""
        procedurelogger.expectedResult('item "%s" is selected' % itemname)
        assert self.label.text == "You select %s" % itemname

    def assertItemsText(self):
        """assert the text of list items is equal to what we see"""
        procedurelogger.action('Check ListItem\'s Text')

        for i in range(20):
            procedurelogger.expectedResult('item "%s"\'s Text is %s' % \
                                                        (self.tablecell[i], i))
            assert self.tablecell[i].text == str(i), \
                'Text was "%s", expected "%s"' % (self.tablecell[i].text, i)

    def selectChildAndCheckStates(self, accessible, childIndex, add_states=[], invalid_states=[]):
        """
        Assert Selection implementation by select the child at childIndex and
        make sure the appropriate accessible was selected

        """
        # select childIndex
        procedurelogger.action('Select child at index %s in "%s"' % \
                                                    (childIndex, accessible))
        accessible.selectChild(childIndex)
        sleep(config.SHORT_DELAY)
        self.assertLabel(str(accessible.getChildAtIndex(childIndex)))

        # check selected item's states
        statesCheck(accessible.getChildAtIndex(childIndex), "TableCell",\
                                                 invalid_states, add_states)

    def clearSelection(self, accessible):
        """Clear selections"""
        procedurelogger.action('Clear selection in "%s"' % accessible)
        accessible.clearSelection()

    # close sample application after running the test
    def quit(self):
        self.altF4()
