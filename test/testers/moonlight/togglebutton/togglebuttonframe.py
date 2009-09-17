
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        09/14/2009
# Description: togglebutton.py wrapper script
#              Used by the togglebutton-*.py tests
##############################################################################

from strongwind import *
from togglebutton import *


# class to represent the main window.
class ToggleButtonFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    TOGGLEBUTTON_ONE = "ToggleButton1"
    TOGGLEBUTTON_TWO = "ToggleButton2"
    TEXE_ONE  = "Two State:"
    TEXE_TWO  = "Three State:"

    def __init__(self, accessible):
        super(ToggleFrame, self).__init__(accessible)
        self.frame = self.findDocumentFrame("ToggleButtonSample")
        self.toggle1 = self.frame.findPushButton(self.TOGGLEBUTTON_ONE)
        self.toggle2 = self.frame.findPushButton(self.TOGGLEBUTTON_TWO)
        self.text1 = self.frame.findLabel(self.TEXT_ONE)
        self.text2 = self.frame.findLabel(self.TEXT_TWO)
