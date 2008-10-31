
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

    #search for ToolStrip as toolbar role name
    def assertToolStrip(self):
        procedurelogger.action('seach for "tool bar" children')

        procedurelogger.expectedResult('succeeded in finding "tool bar"')
        def resultMatches():
            self.toolstrip = self.findToolBar(None)
            return self.toolstrip
        
        assert retryUntilTrue(resultMatches)
    
    #close main window
    def quit(self):
        self.altF4()
