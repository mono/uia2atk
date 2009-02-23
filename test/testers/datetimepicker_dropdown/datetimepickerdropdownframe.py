# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        01/13/2008
# Description: Application wrapper for datetimepicker_dropdown.py
#              be called by ../datetimepicker_dropdown_ops.py
##############################################################################

"""Application wrapper for datetimepicker_dropdown.py"""

from strongwind import *
import time

class DateTimePickerDropDownFrame(accessibles.Frame):
    """the profile of the datetimepicker_dropdown sample"""

    LABEL = 'The date you select is:'
    LABEL_SPACE = ' '
    LABEL_COMMA = ','

    def __init__(self, accessible):
        super(DateTimePickerDropDownFrame, self).__init__(accessible)
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
        self.dropdownbutton = self.findPushButton(None)
        self.label = self.findLabel(self.LABEL)

    def click(self, button):
        procedurelogger.action("click %s" % button)
        button.click()

    def assertText(self, accessible, text=None):
        """assert text is equal to the input"""

        procedurelogger.action('Assert the text of %s' % accessible)
        procedurelogger.expectedResult('%s text is "%s"' % \
                                                (accessible, accessible.text))
        assert accessible.text == text, '%s is not match with "%s"' % \
                                                (accessible, accessible.text)

    def inputText(self, accessible, text):
        procedurelogger.action('set %s text to "%s"' % (accessible, text))
        try:
            accessible.text = text
        except NotImplementedError:
            pass

    def inputValue(self, accessible, value):
        procedurelogger.action('set %s value to "%s"' % (accessible, value))
        try:
            accessible.value = value
        except NotImplementedError:
            pass

    def assertSelectChild(self, accessible, index):
        """assert Selection implementation"""
        procedurelogger.action('select index %s in "%s"' % \
                                        (index, accessible))
        accessible.selectChild(index)    

    def quit(self):
        self.altF4()
