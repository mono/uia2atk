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

    NUMSEPARATORS = 3

    def __init__(self, accessible):
        super(ToolStripSeparatorFrame, self).__init__(accessible)
        self.separators = self.findAllSeparators("")
        assert len(self.separators) == self.NUMSEPARATORS, \
                                "Found %s separator(s), expected %s" % \
                                (len(self.separators), self.NUMSEPARATORS)

    # close main window
    def quit(self):
        self.altF4()
