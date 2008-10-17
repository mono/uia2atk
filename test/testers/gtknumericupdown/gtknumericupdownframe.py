
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
from gtknumericupdown import *


# class to represent the main window.
class GtkNumericUpDownFrame(accessibles.Frame):

    # constants
    # the available widgets on the window

    def __init__(self, accessible):
        super(GtkNumericUpDownFrame, self).__init__(accessible)
        self.numericupdown = self.findAllSpinButtons(None)
        #editable
        self.numericupdown0 = self.numericupdown[0]

    #assert numericupdown's Text value
    def assertText(self, accessible, value):
        procedurelogger.expectedResult('the %s\'s Text value is "%s"' % (accessible, value))

        assert accessible.text == value, 'Text value not match %s' % value

    #set numericupdown's value
    def valueNumericUpDown(self, accessible, newValue):
        procedurelogger.action('set %s value to "%s"' % (accessible, newValue))

        sleep(config.SHORT_DELAY)
        accessible.__setattr__('value', newValue)

    #assert numericupdown's value
    def assertValue(self, accessible, newValue):
        self.maximumValue = accessible._accessible.queryValue().maximumValue
        self.minimumValue = accessible._accessible.queryValue().minimumValue

        if  self.minimumValue <= newValue <= self.maximumValue:
            procedurelogger.expectedResult('the %s\'s current value is "%s"' % (accessible, newValue))
            assert accessible.__getattr__('value') == newValue, \
                       "numericupdown's current value is %s:" % accessible.__getattr__('value')
        else:
            procedurelogger.expectedResult('value "%s" out of run' % newValue)
            assert not accessible.__getattr__('value') == newValue, \
                       "scrollbar's current value is %s:" % accessible.__getattr__('value')

    #enter Text Value for EditableText
    def enterTextValue(self, accessible, values):
        procedurelogger.action('in %s enter %s "' % (accessible, values))

        accessible.__setattr__('text', values)

    
    #close application window
    def quit(self):
        self.altF4()
