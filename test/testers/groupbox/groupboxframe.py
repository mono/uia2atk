
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        08/19/2008
# Description: groupbox.py wrapper script
#              Used by the groupbox-*.py tests
##############################################################################$

import sys
import os
import actions
import states

from strongwind import *
from groupbox import *


# class to represent the main window.
class GroupBoxFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    BUTTON_ONE = "button1"
    BUTTON_TWO = "button2"
    LABEL_ONE = "the first Groupbox"
    LABEL_TWO = "the second Groupbox"
    PANEL_ONE = "GroupBox1"
    PANEL_TWO = "GroupBox2"

    def __init__(self, accessible):
        super(GroupBoxFrame, self).__init__(accessible)
        self.button1 = self.findPushButton(self.BUTTON_ONE)
        self.button2 = self.findPushButton(self.BUTTON_TWO)
        self.label1 = self.findLabel(self.LABEL_ONE)
        self.label2 = self.findLabel(self.LABEL_TWO)
        self.panel1 = self.findPanel(self.PANEL_ONE)
        self.panel2 = self.findPanel(self.PANEL_TWO)

    #give 'click' action
    def click(self,button):
        button.click()

    #check the Label's text after click button
    def assertLabel(self, labelText):
        procedurelogger.expectedResult('Label text has been changed to "%s"' 
                                        % labelText)
        self.findLabel(labelText)
    
    #close application main window after running test
    def quit(self):
        self.altF4()
