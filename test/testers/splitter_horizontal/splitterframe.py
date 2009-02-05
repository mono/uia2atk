##############################################################################
# Written by:  Brian G. Merrell <bgmerrell@novell.com>
# Date:        01/26/2009
# Description: splitter.py wrapper script
#              Used by the splitter-*.py tests
##############################################################################

import sys
import os
import actions
import states

from strongwind import *
from splitter_horizontal import *


# class to represent the main window.
class SplitterFrame(accessibles.Frame):

    # CONSTANTS
    LABEL0 = "label0 on one side against splitter"
    LABEL1 = "label1 on one side against splitter"
    LABEL2 = "label2 on one side against splitter"
    LABEL3 = "label3 on one side against splitter"
    LABEL4 = "label4 on the other side against splitter"

    def __init__(self, accessible):
        super(SplitterFrame, self).__init__(accessible)
        self.split_pane = self.findSplitPane(None)
        self.label0 = self.findLabel(self.LABEL0)
        self.label1 = self.findLabel(self.LABEL1)
        self.label2 = self.findLabel(self.LABEL2)
        self.label3 = self.findLabel(self.LABEL3)
        self.label4 = self.findLabel(self.LABEL4)

    #close Splitter window
    def quit(self):
        self.altF4()
