
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        08/05/2008
# Description: hscrollbar.py wrapper script
#              Used by the hscrollbar-*.py tests
##############################################################################$

import sys
import os
import actions
import states

from strongwind import *
from hscrollbar import *

# class to represent the main window.
class HScrollBarFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    LABEL = "Value:"
    MAXVAL = 100
    MINVAL = 0
    MININCREMENT = 10

    def __init__(self, accessible):
        super(HScrollBarFrame, self).__init__(accessible)
        self.label = self.findLabel(self.LABEL)
        self.hscrollbar = self.findScrollBar(None)
        self.maximumValue = \
                    self.hscrollbar._accessible.queryValue().maximumValue
        self.minimumValue = \
                    self.hscrollbar._accessible.queryValue().minimumValue
        self.minimumIncrement = \
                    self.hscrollbar._accessible.queryValue().minimumIncrement
        # BUG499883 - Accessible maximum value of a scroll bar is 119
        #assert self.maximumValue == self.MAXVAL, \
        #                            "maximum value was %s, expected %s" % \
        #                            (self.maximumValue, self.MAXVAL)
        assert self.minimumValue == self.MINVAL, \
                                    "minimum value was %s, expected %s" % \
                                    (self.minimumValue, self.MINVAL)
        assert self.minimumIncrement == self.MININCREMENT, \
                              "minimum increment value was %s, expected %s" % \
                              (self.minimumIncrement, self.MININCREMENT)

    # change hscrollbar's value
    def assignScrollBar(self, new_value):

        procedurelogger.action('set scrollbar value to "%s"' % new_value)
        self.hscrollbar.value = new_value

    def assertLabel(self, value):
        procedurelogger.expectedResult('label\'s value changed to "%s"' % value)
        expected_label = "Value: %s" % value
        assert self.label.text == expected_label, \
               'Label reads "%s", expected "%s"' % (self.label, expected_label)

    def assertMaximumValue(self):
        procedurelogger.action("Ensure that %s's maximum value is what we expect" % self.hscrollbar)
        procedurelogger.expectedResult("%s's maximum value is %s" % \
                                                (self.hscrollbar, self.MAXVAL))
        self.maximumValue = \
                          self.hscrollbar._accessible.queryValue().maximumValue
        assert self.maximumValue == self.MAXVAL, \
                                    "Maximum value is %s, expected %s" % \
                                    (self.maximumValue, self.MAXVAL)

    def assertMinimumValue(self):
        procedurelogger.action("Ensure that %s's minimum value is what we expect" % self.hscrollbar)
        procedurelogger.expectedResult("%s's minimum value is %s" % \
                                                (self.hscrollbar, self.MINVAL))
        self.minimumValue = \
                          self.hscrollbar._accessible.queryValue().minimumValue
        assert self.minimumValue == self.MINVAL, \
                                    "Minimum value is %s, expected %s" % \
                                    (self.minimumValue, self.MINVAL)

    def assertMinimumIncrement(self):
        procedurelogger.action("Ensure that %s's minimum increment is what we expect" % self.hscrollbar)
        procedurelogger.expectedResult("%s's minimum increment is %s" % \
                                                (self.hscrollbar, self.MINVAL))
        self.minimumIncrement = \
                      self.hscrollbar._accessible.queryValue().minimumIncrement
        assert self.minimumIncrement == self.MININCREMENT, \
                              "minimum increment value was %s, expected %s" % \
                              (self.minimumIncrement, self.MININCREMENT)

    def assertScrollBar(self, expected_value):
        procedurelogger.expectedResult('the scrollbar\'s current value is %s' % expected_value) 
        assert self.hscrollbar.value == expected_value, \
                        "scrollbar's current value is %s, expected %s" % \
                        (self.hscrollbar.value, expected_value)
    
    # close application window
    def quit(self):
        self.altF4()
