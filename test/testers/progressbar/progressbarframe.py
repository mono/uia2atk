
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        09/04/2008
# Description: progressbar.py wrapper script
#              Used by the progressbar-*.py tests
##############################################################################

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

    def assertLabel(self, percent):
        """
        Make sure Lable is changed to show how many percent of the progress 
	after click button

        """
        procedurelogger.expectedResult('%s percent of progress' % percent)

        def resultMatches():
            return self.findLabel(None).text == "It is %s percent " % \
                                                          percent + "of 100%"
        assert retryUntilTrue(resultMatches)

    def value(self, newValue=None):
        """
        Change accessible.value to newValue

        """
        procedurelogger.action('set ProgressBar value to "%s"' % newValue)
        self.progressbar.value = newValue

    # assert maximumValue and minimumValue
    def assertValueImplemented(self, valueType):
        """
        Give valueType as "maximumValue" or "minimumValue" to make sure 
	progressbar's Max and Min value are expected

        """
        maximumValue = self.progressbar._accessible.queryValue().maximumValue
        minimumValue = self.progressbar._accessible.queryValue().minimumValue

        procedurelogger.action('check for %s' % valueType)

        if valueType == "maximumValue":
            procedurelogger.expectedResult('%s is 100' % valueType)
            assert maximumValue == 100, "maximumValue is %s:" % maximumValue
        elif valueType == "minimumValue":
            procedurelogger.expectedResult('%s is 0' % valueType)
            assert minimumValue == 0, "minimumValue is %s:" % minimumValue

    # assert progressbar's current value
    def assertCurrnetValue(self, accessible, value):
        """
        Make sure current accessible.value is expected value. This method to 
	be used after click button to change progress

        """
        procedurelogger.expectedResult('ProgressBar\'s current value is "%s"' % \
                                                                         value)

        assert accessible.value == value, \
                       "current value %s not match %s:" % (accessible.value, 
                                                                         value)

    
    # close application window
    def quit(self):
        self.altF4()
