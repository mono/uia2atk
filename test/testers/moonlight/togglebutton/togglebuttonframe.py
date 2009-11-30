
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
    TEXT_ONE  = "Two State:"
    TEXT_TWO  = "Three State:"

    def __init__(self, accessible):
        super(ToggleButtonFrame, self).__init__(accessible)
        self.frame = self.findDocumentFrame("ToggleButtonSample")
        self.filler = self.frame.findFiller("Silverlight Control")
        self.toggle1 = self.filler.findPushButton(self.TOGGLEBUTTON_ONE)
        self.toggle2 = self.filler.findPushButton(self.TOGGLEBUTTON_TWO)
        self.text1 = self.filler.findLabel(self.TEXT_ONE)
        self.text2 = self.frame.findLabel(self.TEXT_TWO)
