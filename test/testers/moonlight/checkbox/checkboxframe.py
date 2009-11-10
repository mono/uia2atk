
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        09/09/2009
# Description: checkbox.py wrapper script
#              Used by the checkbox-*.py tests
##############################################################################

import sys
import os

from strongwind import *
from checkbox import *


# class to represent the main window.
class CheckBoxFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    CHECK_ONE = "Two State CheckBox"
    CHECK_TWO = "Three State CheckBox"
    TEXT_ONE  = "text1"
    TEXT_TWO  = "text2"

    def __init__(self, accessible):
        super(CheckBoxFrame, self).__init__(accessible)
        self.frame = self.findDocumentFrame("CheckBoxSample")
        self.check1 = self.frame.findCheckBox(self.CHECK_ONE)
        self.check2 = self.frame.findCheckBox(self.CHECK_TWO)
        self.text1 = self.frame.findLabel(self.TEXT_ONE)
        self.text2 = self.frame.findLabel(self.TEXT_TWO)
