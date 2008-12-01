# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        09/08/2008
# Description: numericupdown.py wrapper script
#              Used by the numericupdown-*.py tests
##############################################################################$

'''Application wrapper for numericupdown.py'''

from strongwind import *

# class to represent the main window.
class NumericUpDownFrame(accessibles.Frame):

    def __init__(self, accessible):
        super(NumericUpDownFrame, self).__init__(accessible)
        self.numericupdown = self.findAllSpinButtons(None)
        # editable
        self.editable_numericupdown = self.numericupdown[0]
        # uneditable
        self.uneditable_numericupdown = self.numericupdown[1]


    # set numericupdown's value
    def valueNumericUpDown(self, accessible, newValue):
        procedurelogger.action('set %s value to "%s"' % (accessible, newValue))
        sleep(config.SHORT_DELAY)
        accessible.value = newValue


    # enter Text Value for EditableText
    def enterTextValue(self, accessible, values):
        procedurelogger.action('in %s enter %s "' % (accessible, values))
        if accessible == self.editable_numericupdown:
            accessible.text = values
        elif accessible == self.uneditable_numericupdown:
            try:
                accessible.text = values
            except NotImplementedError:
                pass

    # assert numericupdown's Text value
    def assertText(self, accessible, value):
        procedurelogger.expectedResult('the %s\'s Text value is "%s"' % \
                                                        (accessible, value))
        assert accessible.text == value, 'Text value not match %s' % value


    # assert numericupdown's value
    def assertValue(self, accessible, newValue):
        self.maximumValue = accessible._accessible.queryValue().maximumValue
        self.minimumValue = accessible._accessible.queryValue().minimumValue
        if  self.minimumValue <= newValue <= self.maximumValue:
            procedurelogger.expectedResult('the %s\'s current value is "%d"' % \
                                          (accessible, newValue))
            assert accessible.value == newValue, \
                                    "numericupdown's current value is %s:" % \
                                    accessible.value
        else:
            procedurelogger.expectedResult('value "%s" out of run' % newValue)
            assert not accessible.value == newValue, \
                       "numericupdown's current value is %s:" % accessible.value

    
    #close application window
    def quit(self):
        self.altF4()
