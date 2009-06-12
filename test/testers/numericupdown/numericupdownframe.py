# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        09/08/2008
# Description: Application wrapper for numericupdown.py
#              be called by ../numericupdown_basic_ops.py
##############################################################################$

"""Application wrapper for numericupdown.py"""

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

        self.uneditableMaximumValue = \
            self.uneditable_numericupdown._accessible.queryValue().maximumValue
        self.uneditableMinimumValue = \
            self.uneditable_numericupdown._accessible.queryValue().minimumValue
        self.editableMaximumValue = \
            self.editable_numericupdown._accessible.queryValue().maximumValue
        self.editableMinimumValue = \
            self.editable_numericupdown._accessible.queryValue().minimumValue

    # set numericupdown's value
    def valueNumericUpDown(self, accessible, value):
        procedurelogger.action('set %s value to %s' % (accessible, value))
        accessible.value = value

    # enter Text Value for EditableText
    def enterTextValue(self, accessible, text):
        accessible.deleteText()
        sleep(config.SHORT_DELAY)

        procedurelogger.action('set %s text to "%s"' % (accessible, text))

        if accessible == self.editable_numericupdown:
            accessible.insertText(text)
        elif accessible == self.uneditable_numericupdown:
            # The editable interface is actually implemented for a readonly
            # NumericUpDown control.  This is fine, because it is also the
            # case for an uneditable Gtk spin button.  Therefore, calling
            # insertText will simply be ignored instead of raising a
            # NotImplementedError.  We will leave the try block here because
            # it should be acceptable for a NotImplementedError to be thrown
            # here.
            try:
                accessible.insertText(text)
            except NotImplementedError:
                pass

    # assign the current value of the accessible
    def assignValue(self, accessible, value):
        # use strongwind to assign to queryValue().currentValue
        procedurelogger.action('set %s value to "%s"' % (accessible, value))
        accessible.value = value

    # assert numericupdown's Text value
    def assertText(self, accessible, expected_text):
        procedurelogger.action('Ensure that %s\'s text is what we expect' % accessible)
        actual_text = accessible.text
        procedurelogger.expectedResult('%s\'s text is "%s"' % \
                                                (accessible, expected_text))
        assert actual_text == expected_text, \
               '%s text is "%s", expected "%s"' % \
               (accessible, actual_text, expected_text)

    # assert numericupdown's value
    def assertValue(self, accessible, expected_value):
        actual_value = accessible.value
        procedurelogger.action('Ensure that %s\'s value is what we expect' % accessible)
        procedurelogger.expectedResult('%s value is %d' % \
                                        (accessible, actual_value))
        assert actual_value == expected_value, \
                                "%s's value is %d, expected %d" % \
                                 (accessible, actual_value, expected_value)

    #close application window
    def quit(self):
        self.altF4()
