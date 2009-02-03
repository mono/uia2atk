
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        02/03/2009
# Description: toolstripseparator.py wrapper script
#              Used by the toolstripseparator-*.py tests
##############################################################################

import sys
import os
import actions
import states

from strongwind import *
from toolstripseparator import *


# class to represent the main window.
class ToolStripSeparatorFrame(accessibles.Frame):

    def __init__(self, accessible):
        super(ToolStripSeparatorFrame, self).__init__(accessible)

    #search for ToolStripSeparator as separator role name
    def assertSeparator(self, num=0):
        procedurelogger.action('seach for Separators in ToolBar')

        procedurelogger.expectedResult('there are %s separators in ToolBar' % num)
        def resultMatches():
            self.separators = self.findAllSeparators(None)
            return len(self.separators) == num
        
        assert retryUntilTrue(resultMatches)
    
    #close main window
    def quit(self):
        self.altF4()
