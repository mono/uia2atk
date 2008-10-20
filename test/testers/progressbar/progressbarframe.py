
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
    
    #close application window
    def quit(self):
        self.altF4()
