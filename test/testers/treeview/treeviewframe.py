##############################################################################
# Written by:  Brian G. Merrell <bgmerrell@novell.com>
# Date:        02/12/2009
# Description: treeview.py wrapper script
#              Used by the treeview-*.py tests
##############################################################################

import sys
import os
import re
import actions
import states
import pyatspi

from strongwind import *
from treeview import *


# class to represent the main window.
class TreeViewFrame(accessibles.Frame):

    # constants

    def __init__(self, accessible):
        super(TreeViewFrame, self).__init__(accessible)

    def quit(self):
        self.altF4()
