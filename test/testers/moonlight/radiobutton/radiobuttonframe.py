
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        09/10/2009
# Description: radiobutton.py wrapper script
#              Used by the radiobutton-*.py tests
##############################################################################

from strongwind import *
from radiobutton import *


# class to represent the main window.
class RadioButtonFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    RADIOBUTTON_ONE = "radiobutton1"
    RADIOBUTTON_TWO = "radiobutton2"
    RADIOBUTTON_THREE = "radiobutton3"
    RADIOBUTTON_FOUR = "radiobutton4"
    TEXE_ONE  = "First Group:"
    TEXE_TWO  = "Second Group:"
    TEXT_THREE = ""

    def __init__(self, accessible):
        super(RadioButtonFrame, self).__init__(accessible)
        self.frame = self.findDocumentFrame("RadioButtonSample")
        self.radio1 = self.frame.findRadioButton(self.RADIOBUTTON_ONE)
        self.radio2 = self.frame.findRadioButton(self.RADIOBUTTON_TWO)
        self.radio3 = self.frame.findRadioButton(self.RADIOBUTTON_THREE)
        self.radio4 = self.frame.findRadioButton(self.RADIOBUTTON_FOUR)
        self.text1 = self.frame.findLabel(self.TEXT_ONE)
        self.text2 = self.frame.findLabel(self.TEXT_TWO)
        self.text3 = self.frame.findLabel(self.TEXT_THREE)
