
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

    def __init__(self, accessible):
        super(GroupBoxFrame, self).__init__(accessible)
        self.button1 = self.findPushButton(self.BUTTON_ONE)
        self.button2 = self.findPushButton(self.BUTTON_TWO)
        self.label = self.findAllLabels(None)

    #search groupbox
    def searchGroupBox(self,boxname=None):
        if boxname == 'GroupBox1':
            procedurelogger.action('search for panel of "%s"' % boxname)

            sleep(config.SHORT_DELAY)
            procedurelogger.expectedResult('"%s" existed' % boxname)
            self.panel1 = self.findPanel('GroupBox1')

        elif boxname =='GroupBox2':
            procedurelogger.action('search for panel of "%s"' % boxname)

            sleep(config.SHORT_DELAY)
            procedurelogger.expectedResult('"%s" existed' % boxname)
            self.panel2 = self.findPanel('GroupBox2')

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
