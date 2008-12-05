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
    def valueNumericUpDown(self, accessible, value):
        procedurelogger.action('set %s value to %s' % (accessible, value))
        accessible.value = value


    # enter Text Value for EditableText
    def enterTextValue(self, accessible, text):
        procedurelogger.action('set %s text to "%s"' % (accessible, text))
        if accessible == self.editable_numericupdown:
            accessible.text = text
        elif accessible == self.uneditable_numericupdown:
            try:
                accessible.text = text
            except NotImplementedError:
                pass

    # assert numericupdown's Text value
    def assertText(self, accessible, text):
        procedurelogger.expectedResult('%s text is "%s"' % \
                                                (accessible, accessible.text))
        assert accessible.text == text, '%s text is not match with "%s"' % \
                                                (accessible, accessible.text)


    # assert numericupdown's value
    def assertValue(self, accessible, value):
        self.maximumValue = accessible._accessible.queryValue().maximumValue
        self.minimumValue = accessible._accessible.queryValue().minimumValue
        if  self.minimumValue <= value <= self.maximumValue:
            procedurelogger.expectedResult('%s value is %d' % \
                                            (accessible, accessible.value))
            assert accessible.value == value, \
                                    "%s value is not match with %d" % \
                                            (accessible, accessible.value)
        else:
            procedurelogger.expectedResult('value %s is out of range' % value)
            assert not accessible.value == value, \
                                "value %d is out of range" % accessible.value

    
    #close application window
    def quit(self):
        self.altF4()
