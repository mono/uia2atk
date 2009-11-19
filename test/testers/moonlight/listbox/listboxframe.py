
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

    ITEMS_NUM = 8

    def __init__(self, accessible):
        super(ListBoxFrame, self).__init__(accessible)
        self.frame = self.findDocumentFrame('ListBoxSample')
        self.filler = self.frame.findFiller('Silverlight Control')
        self.label = self.filler.findLabel('You selected no item.')
        self.list_box = self.filler.findList(None)
        self.list_items = [self.list_box.findListItem("Item " + str(x)) for x in range(1,9)]
        #self.list_items = [self.list_box.findListItem(str(x)) for x in range(8)]            
        assert len(self.list_items) == self.ITEMS_NUM, \
                          "actual number of list item:%s, expected:%s" % \
                                    (len(self.list_items), self.ITEMS_NUM)

    def select(self, accessible, index):
        """Select a child in list_box."""

        procedurelogger.action('Select child %s' % index)

        accessible.selectChild(index )

    def clearSelection(self, accessible):
        procedurelogger.action('Clear selection in "%s"' % accessible)

        accessible.clearSelection()
