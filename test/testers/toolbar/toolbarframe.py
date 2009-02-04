
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        02/04/2009
# Description: toolbar.py wrapper script
#              Used by the toolbar-*.py tests
##############################################################################

import sys
import os
import actions
import states

from strongwind import *
from toolbar import *


# class to represent the main window.
class ToolBarFrame(accessibles.Frame):

    def __init__(self, accessible):
        super(ToolBarFrame, self).__init__(accessible)

    #search for toolbar as toolbar role name
    def assertToolBar(self):
        procedurelogger.action('seach for "tool bar" children')

        procedurelogger.expectedResult('succeeded in finding "tool bar"')
        def resultMatches():
            self.toolbar = self.findToolBar(None)
            return self.toolbar
        
        assert retryUntilTrue(resultMatches)
    
    #close main window
    def quit(self):
        self.altF4()
