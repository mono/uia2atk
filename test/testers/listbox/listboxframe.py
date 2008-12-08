# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        08/19/2008
# Description: Application wrapper for listbox.py
#              be called by ../menustrip_basic_ops.py
##############################################################################

"""Application wrapper for listbox.py"""

from strongwind import *

# class to represent the main window.
class ListBoxFrame(accessibles.Frame):

    # the available widgets on the window
    LABEL = "You select "

    def __init__(self, accessible):
        super(ListBoxFrame, self).__init__(accessible)
        self.label = self.findLabel(self.LABEL)
        self.listbox = self.findList(None)
        self.listitem = dict([(x, self.findListItem(str(x))) for x in range(20)])            


    def click(self, listitem):
        """give 'click' action"""
        listitem.click()


    def assertLabel(self, itemname):
        """Raise exception if the accessible does not match the given result"""
        procedurelogger.expectedResult('item "%s" is selected' % itemname)

        def resultMatches():
            return self.findLabel("You select %s" % itemname)
	
        assert retryUntilTrue(resultMatches)


    def assertText(self, textValue=None):
        """assert Text implementation for ListItem role"""
        procedurelogger.action('check ListItem\'s Text Value')

        for textValue in range(20):
            procedurelogger.expectedResult('item "%s"\'s Text is %s' % \
                                        (self.listitem[textValue], textValue))
            assert self.listitem[textValue].text == str(textValue)


    def assertSelectionChild(self, accessible, childIndex):
        """assert Selection implementation"""
        procedurelogger.action('select childIndex %s in "%s"' % \
                                        (childIndex, accessible))

        accessible.selectChild(childIndex)


    def assertClearSelection(self, accessible):
        """clear selections"""
        procedurelogger.action('clear selection in "%s"' % accessible)

        accessible.clearSelection()
    

    # close sample application after running the test
    def quit(self):
        self.altF4()
