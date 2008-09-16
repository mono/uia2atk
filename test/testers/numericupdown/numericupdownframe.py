
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        09/08/2008
# Description: numericupdown.py wrapper script
#              Used by the numericupdown-*.py tests
##############################################################################$

import sys
import os
import actions
import states

from strongwind import *
from numericupdown import *


# class to represent the main window.
class NumericUpDownFrame(accessibles.Frame):

    # constants
    # the available widgets on the window

    def __init__(self, accessible):
        super(NumericUpDownFrame, self).__init__(accessible)
        self.numericupdown = self.findSpinButton(None)

    #check numericupdown's all expectant states
    def statesCheck(self, accessible=None):
        accessible = self.numericupdown
        procedurelogger.action('check %s\'s all states' % accessible)

        procedurelogger.expectedResult('%s\'s all states can be found' % accessible)
        for a in states.NumericUpDown.states:
            state = getattr(accessible, a)
            assert state, "Expected state: %s" % (a)

    #assert the numericupdown's percent after click button
    def assertLabel(self, percent):
        procedurelogger.expectedResult('%s percent of progress' % percent)

        def resultMatches():
            return self.findLabel(None).text == "It is %s percent " % percent + "of 100%"
        assert retryUntilTrue(resultMatches)

    #set numericupdown's value
    def valueNumericUpDown(self, newValue):
        procedurelogger.action('set numericupdown value to "%s"' % newValue)
        numericupdown = self.findSpinButton(None)
        sleep(config.LONG_DELAY)
        numericupdown.__setattr__('value', newValue)

    #assert numericupdown's value
    def assertValue(self, value):
        self.maximumValue = self.numericupdown._accessible.queryValue().maximumValue
        self.minimumValue = self.numericupdown._accessible.queryValue().minimumValue

        def resultMatches():
            if value >= self.minimumValue and value <= self.maximumValue:
                procedurelogger.expectedResult('the numericupdown\'s current value is "%s"' % value)
                return self.numericupdown.__getattr__('value') == value
            elif value > self.maximumValue:
                procedurelogger.expectedResult('value "%s" out of run, the maximum value is "%s"' % (value, self.maximumValue))
                return not self.numericupdown.__getattr__('value') == value 
                return self.numericupdown.__getattr__('value') == self.maximumValue
            elif value < self.minimumValue:
                procedurelogger.expectedResult('value "%s" out of run, the minimum value is "%s"' % (value, self.minimumValue))
                return not self.numericupdown.__getattr__('value') == value 
                return self.numericupdown.__getattr__('value') == self.minimumValue

        assert retryUntilTrue(resultMatches)
    
    #close application window
    def quit(self):
        self.altF4()
