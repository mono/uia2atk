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
    LABEL = "Sample for ProgressBar"
    BUTTON = "button1"
    TOOLSTRIPLABEL = "ToolStripLabel Text"

    def __init__(self, accessible):
        super(ToolStripProgressBarFrame, self).__init__(accessible)
        self.label = self.findLabel(self.LABEL)
        self.toolstriplabel = self.findLabel(self.TOOLSTRIPLABEL)
        self.button = self.findPushButton(self.BUTTON)
        # the progress bar control typically gets its name from a static$
        # text label (according to MSDN).$
        # BUG500400 - the progress bar is getting the wrong name$
        # self.progressbar = self.findProgressBar(LABEL)$

        # This is a temporary fix for BUG500400.  Delete this when that bug$
        # is fixed.$
        self.progressbar = self.findProgressBar(None)

    def assertLabel(self, expected_text):
        """
        assert the toolstripprogress's percent after click button
        """
        procedurelogger.expectedResult('label shows "%s"' % expected_text)

        actual_text = self.label.text
        def resultMatches():
            return actual_text == expected_text
        assert retryUntilTrue(resultMatches), \
            'Label text was "%s", expected "%s"' % (actual_text, expected_text)

    def assignValue(self, new_value):
        """
        insert value
        """
        procedurelogger.action('set ProgressBar value to "%s"' % new_value)
        self.progressbar.value = new_value

    def assertMaximumValue(self):
        """
        assert maximumValue 
        """
        procedurelogger.action('Ensure that the maximum value of \
                                          the progress bar is what we expect')
        actual_max_value = \
                         self.progressbar._accessible.queryValue().maximumValue
        procedurelogger.expectedResult('maximum value is 100')
        assert actual_max_value == 100, \
                   "actual value was %s, expected %s" % (actual_max_value, 100)

    def assertMinimumValue(self):
        """
        assert minimumValue
        """
        procedurelogger.action('Ensure that the minimum value of the progress\
                                                       bar is what we expect')
        actual_min_value = \
                         self.progressbar._accessible.queryValue().minimumValue
        procedurelogger.expectedResult('minimum value is 0')
        assert actual_min_value == 0, \
                   "actual value was %s, expected %s" % (actual_min_value, 0)

    def assertCurrentValue(self, accessible, value):
        """
        assert progressbar's  Current value
        """
        procedurelogger.expectedResult('ProgressBar\'s current value is \
                                                                 "%s"' % value)

        assert accessible.value == value, \
                       "progressbar's current value is %s:" % accessible.value

    def quit(self):
        """
        close application window
        """
        self.altF4()
