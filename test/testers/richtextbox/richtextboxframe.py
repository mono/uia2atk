##############################################################################
# Written by:  Brian G. Merrell <bgmerrell@novell.com>
# Date:        01/20/2009
# Description: richtextbox.py wrapper script
#              Used by the richtextbox-*.py tests
##############################################################################$

import sys
import os
import actions
import states

from strongwind import *
from richtextbox import *

# class to represent the main window.
class RichTextBoxFrame(accessibles.Frame):

    def __init__(self, accessible):
        super(RichTextBoxFrame, self).__init__(accessible)

    #close application window
    def quit(self):
        self.altF4()
