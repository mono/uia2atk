
##############################################################################
# Written by:  Neville Gao  <nevillegao@gmail.com>
# Date:        2009/09/22
# Description: scrollbar.py wrapper script
#              Used by the scrollbar-*.py tests
##############################################################################

from strongwind import *
from scrollbar import *

# class to represent the main window.
class ScrollBarFrame(accessibles.Frame):

    BUTTONS_NUM = 5

    def __init__(self, accessible):
        super(ScrollBarFrame, self).__init__(accessible)
        self.frame = self.findDocumentFrame('ScrollBarSample')
        self.filler = self.findFiller('Silverlight Control')
        self.hlabel = self.filler.findLabel('Value of Horizontal: 0')
        self.vlabel = self.filler.findLabel('Value of Vertical: 0')

        scrollBars = self.filler.findAllScrollBars(None)
        for scrollBar in scrollBars:
            if scrollBar.vertical:
                self.vscrollBar = scrollBar
            else:
                self.hscrollBar = scrollBar

        self.vs_buttons = self.vscrollBar.findAllPushButtons(None)
        assert len(self.vs_buttons) == BUTTONS_NUM, \
               "actual number is %s, expected %s" % \
                  (len(self.vs_buttons), BUTTONS_NUM)

        self.hs_buttons = self.hscrollBar.findAllPushButtons(None)
        assert len(self.hs_buttons) == BUTTONS_NUM, \
               "actual number is %s, expected %s" % \
                  (len(self.hs_buttons), BUTTONS_NUM)

    def setValue(self, accessible, value):
        """
        Set scrollBar's value
        """
        procedurelogger.action("set scrollBar's value to %s" % value)

        accessible.value = value
