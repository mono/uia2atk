
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
    MAXVAL = 100
    MINVAL = 0
    

    def __init__(self, accessible):
        super(ProgressBarFrame, self).__init__(accessible)
        self.label = self.findLabel(self.LABEL)
        self.button = self.findPushButton(self.BUTTON)
        self.progressbar = self.findProgressBar(None)
        self.maximumValue = self.progressbar._accessible.queryValue().maximumValue
        self.minimumValue = self.progressbar._accessible.queryValue().minimumValue

    def assertLabel(self, percent):
        """
        Make sure Label is changed to show how many percent of the progress 
        after click button
        """
        procedurelogger.expectedResult('%s percent of progress' % percent)

        def resultMatches():
            return self.findLabel(None).text == "It is %s percent " % \
                                                          percent + "of 100%"
        assert retryUntilTrue(resultMatches)

    def assignValue(self, newValue=None):
        """
        Attempt to change accessible.value to newValue using the Value
        interface.  This currently does *not* set the value.  This is fine
        because it doesn't set the value in Gtk either.
        """
        procedurelogger.action('set ProgressBar value to "%s"' % newValue)
        self.progressbar.value = newValue
    
    def assertMaximumValue(self):
        procedurelogger.action('Ensure that "%s"\'s maximum value is what we expect' % self.progressbar)
        procedurelogger.expectedResult("%s's maximum value is %s" % \
                                                (self.progressbar, self.MAXVAL))
        self.maximumValue = \
                          self.progressbar._accessible.queryValue().maximumValue
        assert self.maximumValue == self.MAXVAL, \
                                         "Maximum value is %s, expected %s" % \
                                         (self.maximumValue, self.MAXVAL)
 
    def assertMinimumValue(self):
        procedurelogger.action("Ensure that %s's minimum value is what we expect" % self.progressbar)
        procedurelogger.expectedResult('"%s"\'s minimum value is %s' % \
                                               (self.progressbar, self.MINVAL))
        assert self.minimumValue == self.MINVAL, \
                                        "Minimum value is %s, expected %s" % \
                                        (self.minimumValue, self.MINVAL)

    # assert progressbar's current value
    def assertCurrentValue(self, accessible, expected_value):
        """
        Make sure current accessible.value is expected value. This method is to
        be used after click button to change progress
        """
        procedurelogger.action('Ensure current value is what we expect' % self.progressbar)
        procedurelogger.expectedResult('Current value is "%s"' % expected_value)
        assert accessible.value == expected_value, \
                       "current value is %s, expected %s" % \
                                           (accessible.value, expected_value)
    
    # close application window
    def quit(self):
        self.altF4()
