
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        10/31/2008
# Description: toolstrip.py wrapper script
#              Used by the toolstrip-*.py tests
##############################################################################

import sys
import os
import actions
import states

from strongwind import *
from toolstrip import *


# class to represent the main window.
class ToolStripFrame(accessibles.Frame):

    def __init__(self, accessible):
        super(ToolStripFrame, self).__init__(accessible)
        self.toolstrip = self.findToolBar(None)
    
    # close main window
    def quit(self):
        self.altF4()
