
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        09/04/2008
# Description: progressbar.py wrapper script
#              Used by the progressbar-*.py tests
##############################################################################$

import sys
import os
import actions
import states

from strongwind import *
from progressbar import *


# class to represent the main window.
class ProgressBarFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    LABEL = "It is 0 percent of 100%"
    BUTTON = "Click"

    def __init__(self, accessible):
        super(ProgressBarFrame, self).__init__(accessible)
        self.label = self.findLabel(self.LABEL)
        self.button = self.findPushButton(self.BUTTON)
        self.progressbar = self.findProgressBar(None)

    #give 'click' action
    def click(self, button):
        button.click()

    #assert the progress's percent after click button
    def assertLabel(self, percent):
        procedurelogger.expectedResult('%s percent of progress' % percent)

        def resultMatches():
            return self.findLabel(None).text == "It is %s percent " % percent + "of 100%"
        assert retryUntilTrue(resultMatches)

    #insert value
    def value(self, newValue=None):
        procedurelogger.action('set ProgressBar value to "%s"' % newValue)
        self.progressbar.__setattr__('value', newValue)

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

        assert accessible.__getattr__('value') == value, \
                       "progressbar's current value is %s:" % accessible.__getattr__('value')

    
    #close application window
    def quit(self):
        self.altF4()
