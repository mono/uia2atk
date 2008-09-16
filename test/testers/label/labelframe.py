
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        08/07/2008
# Description: label.py wrapper script
#              Used by the label-*.py tests
##############################################################################

import sys
import os
import actions
import states

from strongwind import *
from label import *


# class to represent the main window.
class LabelFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    BUTTON = "button2"
    LABEL = ""

    def __init__(self, accessible):
        super(LabelFrame, self).__init__(accessible)
        self.button = self.findPushButton(self.BUTTON)
        self.label = self.findLabel(None)

    #check Label's all expectant states
    def statesCheck(self, accessible):
        procedurelogger.action('check %s\'s all states' % "label")

        procedurelogger.expectedResult('%s\'s all states can be found' % "label")
        for a in states.Label.states:
            state = getattr(accessible, a)
            assert state, "Expected state: %s" % (a)

    #give 'click' action
    def click(self,button):
        button.click()

    #check the Label text after click button2
    def assertLabel(self, labelText):
        procedurelogger.expectedResult('Label text has been changed to "%s"' % labelText)
        self.findLabel(labelText)

    #check label's text value
    def assertText(self, textValue):
        procedurelogger.expectedResult('Label\'s text value shows in accerciser is "%s"' % textValue)
        def resultMatches():
            return self.label.text == textValue
        assert retryUntilTrue(resultMatches)
    
    #close application main window after running test
    def quit(self):
        self.altF4()
