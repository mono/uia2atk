
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
    RADIOBUTTON_FOUR = ""
    LABEL = "radiobutton4"
    TEXE_ONE  = "First Group:"
    TEXE_TWO  = "Second Group:"
    TEXT_THREE = ""

    def __init__(self, accessible):
        super(RadioButtonFrame, self).__init__(accessible)
        self.frame = self.findDocumentFrame("RadioButtonSample")
        self.filler = self.frame.findFiller(None)
        self.radio1 = self.filler.findRadioButton(self.RADIOBUTTON_ONE)
        self.radio2 = self.filler.findRadioButton(self.RADIOBUTTON_TWO)
        self.radio3 = self.filler.findRadioButton(self.RADIOBUTTON_THREE)
        self.radio4 = self.filler.findRadioButton(self.RADIOBUTTON_FOUR)
        self.radio4_label = self.radio4.findLabel(self.LABEL)
        self.radio4_image = self.radio4.findImage("")
        self.text1 = self.filler.findLabel(self.TEXE_ONE)
        self.text2 = self.filler.findLabel(self.TEXE_TWO)
        self.text3 = self.filler.findLabel(self.TEXT_THREE)
