
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        09/04/2008
# Description: toolstripprogressbar.py wrapper script
#              Used by the toolstripprogressbar-*.py tests
##############################################################################$

import sys
import os
import actions
import states

from strongwind import *
from toolstripprogressbar import *


# class to represent the main window.
class ToolStripProgressBarFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    LABEL = "It is 0% of 100%"
    BUTTON = "button1"

    def __init__(self, accessible):
        super(ToolStripProgressBarFrame, self).__init__(accessible)
        self.label = self.findLabel(self.LABEL)
        self.button = self.findPushButton(self.BUTTON)
        self.progressbar = self.findProgressBar(None)

    #give 'click' action
    def click(self, button):
        button.click()

    #assert the toolstripprogress's percent after click button
    def assertLabel(self, newlabel):
        procedurelogger.expectedResult('label shows "%s"' % newlabel)

        def resultMatches():
            return self.label.text == newlabel
        assert retryUntilTrue(resultMatches)

    #insert value
    def value(self, newValue=None):
        procedurelogger.action('set ProgressBar value to "%s"' % newValue)
        self.progressbar.value = newValue

    #assert maximumValue and minimumValue
    def assertValueImplemented(self, valueType):
        maximumValue = self.progressbar._accessible.queryValue().maximumValue
        minimumValue = self.progressbar._accessible.queryValue().minimumValue

        procedurelogger.action('check for %s' % valueType)

        if valueType == "maximumValue":
            procedurelogger.expectedResult('%s is 100' % valueType)
            assert maximumValue == 100, "maximumValue is %s:" % maximumValue
        if valueType == "minimumValue":
            procedurelogger.expectedResult('%s is 0' % valueType)
            assert minimumValue == 0, "minimumValue is %s:" % minimumValue

    #assert progressbar's value
    def assertCurrnetValue(self, accessible, value):
        procedurelogger.expectedResult('ProgressBar\'s current value is "%s"' % value)

        assert accessible.value == value, \
                       "progressbar's current value is %s:" % accessible.value

    
    #close application window
    def quit(self):
        self.altF4()
