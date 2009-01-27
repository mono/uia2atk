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
from splitter import *


# class to represent the main window.
class SplitterFrame(accessibles.Frame):

    def __init__(self, accessible):
        super(SplitterFrame, self).__init__(accessible)

    #close Splitter window
    def quit(self):
        self.altF4()
