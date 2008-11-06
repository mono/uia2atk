
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        11/07/2008
# Description: statusstrip.py wrapper script
#              Used by the statusstrip-*.py tests
##############################################################################$

import sys
import os
import actions
import states

from strongwind import *
from statusstrip import *


# class to represent the main window.
class StatusStripFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    BUTTON_ONE = "Click Me"
    BUTTON_TWO = "ToolStripDropDownButton"
    BUTTON_THREE = "ToolStripSplitButton"
    LABEL_ONE = "Examples for: StatusStrip."
    LABEL_TWO = "ToolStripLabel Text..."

    def __init__(self, accessible):
        super(StatusStripFrame, self).__init__(accessible)
        self.StripButton = self.findPushButton(self.BUTTON_ONE)
        self.DropDownButton = self.findPushButton(self.BUTTON_TWO)
        self.SplitButton = self.findPushButton(self.BUTTON_THREE)
        self.MainLabel = self.findLabel(self.LABEL_ONE)
        self.StripLabel = self.findLabel(self.LABEL_TWO)
        self.ProgressBar = self.findProgressBar(None)
        self.DropDownButton_item1 = self.findMenuItem("Red")
        self.DropDownButton_item2 = self.findMenuItem("Blue")
        self.SplitButton_item1 = self.findMenuItem("Blue Color")
        self.SplitButton_item2 = self.findMenuItem("Red Color")

    #give 'click' action
    def click(self,button):
        button.click()

    #assert if can find statusbar
    def assertStatusBar(self):
        procedurelogger.action('search for statusbar role')

        procedurelogger.expectedResult('succeeded in finding StatusBar role')
        self.statusbar = self.findStatusBar("None")
        assert self.statusbar

#####################ToolStripProgressBar test################################
    #assert the progress's percent after click button, it also can check the 
    #change of ToolStripStatusLable's text
    def assertLabel(self, label, percent):
        procedurelogger.expectedResult('%s percent of progress' % percent)

        def resultMatches():
            return label.text == "It is %s " % percent + "of 100%"
        assert retryUntilTrue(resultMatches)

    #assert progressbar's value
    def assertValue(self, progressbar, newValue=None):
        maximumValue = progressbar._accessible.queryValue().maximumValue

        if 0 <= newValue <= maximumValue:
            procedurelogger.expectedResult('the %s\'s current value is "%s"' % (progressbar, newValue))
            assert progressbar.__getattr__('value') == newValue, \
                       "progressbar's current value is %s:" % progressbar.__getattr__('value')
        else:
            procedurelogger.expectedResult('value "%s" out of run' % newValue)
            assert not progressbar.__getattr__('value') == newValue, \
                       "progressbar's current value is %s:" % progressbar.__getattr__('value')

################ToolStripDropDownButton and ToolStripSplitButton test#################
    #assert the Text of MainLabel's color attribute to make sure clicking menu item in 
    #ToolStripDropDownButton is taking affect. it also can text the ToolStripStatusLabel's 
    #Text implementation
    def assertTextColor(self, label, color):
        procedurelogger.expectedResult('Color of %s turn to %s' % (label, color))
        
        text = label._accessible.queryText()
        attributes = text.getAttributeRun(0, False)
        fg = attributes[0][0]
        if color == "Red":
            assert fg == "fg-color:65535,0,0"
        elif color == "Blue":
            assert fg == "fg-color:0,0,65535"

    
    #close application main window after running test
    def quit(self):
        self.altF4()
