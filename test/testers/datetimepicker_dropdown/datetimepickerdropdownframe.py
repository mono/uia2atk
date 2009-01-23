# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        01/13/2008
# Description: Application wrapper for datetimepicker_dropdown.py
#              be called by ../datetimepicker_dropdown_basic_ops.py
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
        self.lists = self.findAllLists(None)
        self.spinbuttons = self.findAllSpinButtons(None)
        self.listitems = self.findAllListItems(None, checkShowing=False)
        self.weekdays = self.listitems[0:7]
        self.months = self.listitems[7:]
        self.spaces = self.findAllLabels(self.LABEL_SPACE)
        self.commas = self.findAllLabels(self.LABEL_COMMA)

        self.checkbox = self.findCheckBox(None)
        self.weekday = self.lists[1]
        self.month = self.lists[0]
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

    def quit(self):
        self.altF4()
