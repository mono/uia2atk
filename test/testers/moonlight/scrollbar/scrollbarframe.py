
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

    def __init__(self, accessible):
        super(ScrollBarFrame, self).__init__(accessible)
        self.frame = self.findDocumentFrame('ScrollBarSample')
        self.hlabel = self.frame.findLabel('Value of Horizontal: 0')
        self.vlabel = self.frame.findLabel('Value of Vertical: 0')
        self.hscrollBar = self.frame.findScrollBar('')
        self.vscrollBar = self.frame.findScrollBar('')

    def setValue(self, accessible, value):
        """
        Set scrollBar's value
        """
        procedurelogger.action("set scrollBar's value to %s" % value)

        accessible.value = value
