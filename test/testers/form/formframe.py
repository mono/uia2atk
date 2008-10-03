
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        08/11/2008
# Description: form.py wrapper script
#              Used by the form-*.py tests
##############################################################################

import sys
import os
import actions
import states

from strongwind import *
from form import *


# class to represent the main window.
class FormFrame(accessibles.Frame):

    def __init__(self, accessible):
        super(FormFrame, self).__init__(accessible)
        self.button1 = self.findPushButton("button1")
        self.button2 = self.findPushButton("button2")

    #perform click action
    def click(self, button):
        button.click()
    
    #close Form window
    def quit(self):
        self.altF4()
