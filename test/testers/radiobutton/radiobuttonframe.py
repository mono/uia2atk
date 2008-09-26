
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        07/29/2008
# Description: radiobutton.py wrapper script
#              Used by the radiobutton-*.py tests
##############################################################################$

import sys
import os
import actions
import states

from strongwind import *
from radiobutton import *


# class to represent the main window.
class RadioButtonFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    BUTTON_ONE = "Male"
    BUTTON_TWO = "Female"
    BUTTON_THREE = "Disabled"
    LABEL = "Go On:____"

    def __init__(self, accessible):
        super(RadioButtonFrame, self).__init__(accessible)
        self.button1 = self.findRadioButton(self.BUTTON_ONE)
        self.button2 = self.findRadioButton(self.BUTTON_TWO)
        self.button3 = self.findRadioButton(self.BUTTON_THREE)
        self.label = self.findLabel(self.LABEL)

    #give 'click' action
    def click(self,button):
        button.click()

    #check the Label text after click RadioButton
    def assertLabel(self, labelText):
        procedurelogger.expectedResult('Label text has been changed to "%s"' % labelText)
        self.findLabel(labelText)
    
    #close application main window after running test
    def quit(self):
        self.altF4()
