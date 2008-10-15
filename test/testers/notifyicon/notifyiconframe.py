
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        09/16/2008
# Description: notifyicon.py wrapper script
#              Used by the notifyicon-*.py tests
##############################################################################$

import sys
import os
import actions
import states

from strongwind import *
from notifyicon import *


# class to represent the main window.
class NotifyIconFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    BUTTON_ONE = "button"

    def __init__(self, accessible):
        super(NotifyIconFrame, self).__init__(accessible)
        self.button = self.findPushButton(self.BUTTON_ONE)

    #give 'click' action
    def click(self, button):
        button.click()
 
    #close application main window after running test
    def quit(self):
        self.altF4()
