# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        02/04/2009
# Description: Application wrapper for tablelayoutpanel.py
#              be called by ../tablelayoutpanel_basic_ops.py
##############################################################################

"""Application wrapper for tablelayoutpanel.py"""

from strongwind import *

class TableLayoutPanelFrame(accessibles.Frame):
    """the profile of the tablelayoutpanel sample"""

    def __init__(self, accessible):
        super(TableLayoutPanelFrame, self).__init__(accessible)
        self.panel = self.findPanel(None)
        self.labels = self.findAllLabels(None)
        self.buttons = self.findAllPushButtons(None)

        assert len(self.labels) == 4, "The numbers of label is wrong!"
        assert len(self.buttons) == 4, "The numbers of buttons is wrong!"
    
    def click(self, accessible):
        procedurelogger.action("click %s" % accessible)
        accessible.click()

    def assertText(self, accessible, expected_text):
        """assert text is equal to the expected text"""
        procedurelogger.action('Assert the text of %s is what we expect' % accessible)
        procedurelogger.expectedResult('%s text is "%s"' % \
                                                (accessible, expected_text))
        assert accessible.text == expected_text, \
                                             'text was "%s", expected "%s"' % \
                                             (accessible.text, expected_text)

    def quit(self):
        self.altF4()
