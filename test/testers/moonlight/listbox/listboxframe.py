
##############################################################################
# Written by:  Neville Gao  <nevillegao@gmail.com>
# Date:        2009/09/22
# Description: listbox.py wrapper script
#              Used by the listbox-*.py tests
##############################################################################

from strongwind import *
from listbox import *
from helpers import *

# class to represent the main window.
class ListBoxFrame(accessibles.Frame):

    def __init__(self, accessible):
        super(ListBoxFrame, self).__init__(accessible)
        self.frame = self.findDocumentFrame('ListBoxSample')
        self.label = self.frame.findLabel('You selected no item.')
        self.list_box = self.frame.findTreeTable('')
        self.list_items = [self.list_box.findTableCell(str(x)) for x in range(8)]

    def select(self, accessible):
        """Select a child in list_box."""

        procedurelogger.action('Select child %s' % accessible)

        accessible.selectChild(accessible)

    def clearSelection(self, accessible):
        procedurelogger.action('Clear selection in "%s"' % accessible)

        accessible.clearSelection()
