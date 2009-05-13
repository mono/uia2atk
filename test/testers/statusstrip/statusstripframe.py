
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
    BUTTON_ONE = "button1"
    BUTTON_TWO = "ToolStripDropDownButton"
    BUTTON_THREE = "ToolStripSplitButton"
    LABEL_ONE = "Examples for: StatusStrip."
    LABEL_TWO = "ToolStripLabel Text..."

    def __init__(self, accessible):
        super(StatusStripFrame, self).__init__(accessible)
        self.button = self.findPushButton(self.BUTTON_ONE)
        self.statusstrip = self.findStatusBar(None)
        self.MainLabel = self.findLabel(self.LABEL_ONE)
        self.StripLabel = self.findLabel(self.LABEL_TWO)
        self.ProgressBar = self.findProgressBar(None)

    def assertLabel(self, label, expected_text):
        """
        Make sure Label text is changed to expected_text
        """
        procedurelogger.expectedResult('%s\'s text is "%s"' % \
                                                        (label, expected_text))

        actual_text = label.text
        def resultMatches():
            return actual_text == expected_text
        assert retryUntilTrue(resultMatches), \
                  'text was "%s", expected "%s"' % (actual_text, expected_text)
                                               

    def assertProgressBarValue(self, accessible, expected_value):
        """
        Make sure current value of accessible is expected value
        """
        procedurelogger.expectedResult('ProgressBar\'s current value is "%s"' % expected_value)
        actual_value = accessible.value
        assert actual_value == expected_value, \
                                  "progressbar's value was %s, expected %s" % \
                                  (actual_value, expected_value)
    
    # close application main window after running test
    def quit(self):
        self.altF4()
