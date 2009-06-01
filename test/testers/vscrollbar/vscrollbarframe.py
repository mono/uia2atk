
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        08/04/2008
# Description: vscrollbar.py wrapper script
#              Used by the vscrollbar-*.py tests
##############################################################################$

import sys
import os
import actions
import states

from strongwind import *
from vscrollbar import *


# class to represent the main window.
class VScrollBarFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    LABEL = "Value:"
    MAXVAL = 119
    MINVAL = 0
    MININCREMENT = 10

    def __init__(self, accessible):
        super(VScrollBarFrame, self).__init__(accessible)
        self.label = self.findLabel(self.LABEL)
        self.vscrollbar = self.findScrollBar(None)

    # change vscrollbar's value
    def assignScrollBarValue(self, new_value):
        procedurelogger.action('set scrollbar value to "%s"' % new_value)
        self.vscrollbar.value = new_value

    def assertLabel(self, value):
        procedurelogger.expectedResult('label\'s value is "%s"' % value)
        expected_label = "Value: %s" % value
        assert self.label.text == expected_label, \
               'Label reads "%s", expected "%s"' % (self.label, expected_label)

    def assertMaximumValue(self):
        procedurelogger.action("Ensure that %s's maximum value is what we expect" % self.vscrollbar)
        procedurelogger.expectedResult("%s's maximum value is %s" % \
                                                (self.vscrollbar, self.MAXVAL))
        self.maximumValue = \
                        self.vscrollbar._accessible.queryValue().maximumValue
        assert self.maximumValue == self.MAXVAL, \
                                        "Maximum value is %s, expected %s" % \
                                        (self.maximumValue, self.MAXVAL)

    def assertMinimumValue(self):
        procedurelogger.action("Ensure that %s's minimum value is what we expect" % self.vscrollbar)
        procedurelogger.expectedResult("%s's minimum value is %s" % \
                                                (self.vscrollbar, self.MINVAL))
        self.minimumValue = \
                        self.vscrollbar._accessible.queryValue().minimumValue
        assert self.minimumValue == self.MINVAL, \
                                        "Minimum value is %s, expected %s" % \
                                        (self.minimumValue, self.MAXVAL)

    def assertMinimumIncrement(self):
        procedurelogger.action("Ensure that %s's minimum increment value is what we expect" % self.vscrollbar)
        procedurelogger.expectedResult("%s's minimum increment value is %s" % \
                                          (self.vscrollbar, self.MININCREMENT))
        self.minimumIncrement = \
                      self.vscrollbar._accessible.queryValue().minimumIncrement
        assert self.minimumIncrement == self.MININCREMENT, \
                               "Minimum increment value is %s, expected %s" % \
                               (self.minimumIncrement, self.MININCREMENT)

    def assertScrollBar(self, expected_value):
        procedurelogger.expectedResult('the scrollbar\'s current value is %s' % expected_value)
        assert self.vscrollbar.value == expected_value, \
                         "scrollbar's current value is %s, expected %s" % \
                         (self.vscrollbar.value, expected_value)
    
    # close application window
    def quit(self):
        self.altF4()
