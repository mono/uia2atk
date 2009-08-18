# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        02/16/2008
# Description: Application wrapper for datetimepicker_showupdown.py
#              be called by ../datetimepicker_showupdown_ops.py
##############################################################################

"""Application wrapper for datetimepicker_showupdown.py"""

from strongwind import *
import time

class DateTimePickerShowUpDownFrame(accessibles.Frame):
    """the profile of the datetimepicker_showupdown sample"""

    LABEL = 'The date you select is:'
    LABEL_SPACE = ' '
    LABEL_COMMA = ','

    def __init__(self, accessible):
        super(DateTimePickerShowUpDownFrame, self).__init__(accessible)
        self.localtime = time.localtime()
        self.panel = self.findPanel(None)
        self.treetables = self.findAllTreeTables(None)
        self.spinbuttons = self.findAllSpinButtons(None)
        self.items = self.findAllTableCells(None, checkShowing=False)
        self.weekdays = self.items[0:7]
        self.months = self.items[7:]
        self.spaces = self.findAllLabels(self.LABEL_SPACE)
        self.commas = self.findAllLabels(self.LABEL_COMMA)

        self.checkbox = self.findCheckBox(None)
        self.weekday = self.treetables[0]
        self.month = self.treetables[1]
        self.day = self.spinbuttons[0]
        self.year = self.spinbuttons[1]
        self.label = self.findLabel(self.LABEL)

    def click(self, button):
        procedurelogger.action("click %s" % button)
        button.click()

    def assertText(self, accessible, expected_text):
        """assert the accessible's text is equal to the expected text"""

        procedurelogger.action('Check the text of: %s' % accessible)
        actual_text = accessible.text
        procedurelogger.expectedResult('Text is "%s"' % actual_text)
        assert actual_text == expected_text, 'Text was "%s", expected "%s"' % \
                                                (actual_text, expected_text)

    def assertUneditableText(self, accessible, text):
        '''
        Ensure that the EditableText interface is not implemented for the
        accessible
        '''
        procedurelogger.action('Attempt to set %s text to "%s"' % \
                                                            (accessible, text))
        try:
            # this uses the EditableText interface
            accessible.text = text
        except NotImplementedError:
            return
        assert False, "The Text interface should not be implemented for %s" % \
                                                                   (accessible)

    def assignValue(self, accessible, value):
        procedurelogger.action('set "%s" value to "%s"' % (accessible, value))
        accessible.value = value

    def selectChild(self, accessible, index):
        """
        Simply call strongwind's selectChild method but add some logging
        information
        """
        procedurelogger.action('Select index %s of "%s"' % (index, accessible))
        accessible.selectChild(index)    

    def quit(self):
        self.altF4()
