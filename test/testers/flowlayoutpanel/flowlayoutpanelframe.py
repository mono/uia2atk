# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        02/02/2009
# Description: Application wrapper for flowlayoutpanel.py
#              be called by ../flowlayoutpanel_basic_ops.py
##############################################################################

"""Application wrapper for flowlayoutpanel.py"""

from strongwind import *

class FlowLayoutPanelFrame(accessibles.Frame):
    """the profile of the flowlayoutpanel sample"""

    def __init__(self, accessible):
        super(FlowLayoutPanelFrame, self).__init__(accessible)
        self.panels = self.findAllPanels(None)
        self.labels = self.findAllLabels(None)
        self.buttons = self.findAllPushButtons(None)

        assert len(self.panels) == 4, "The numbers of panel is wrong!"
        assert len(self.labels) == 4, "The numbers of label is wrong!"
        assert len(self.buttons) == 4, "The numbers of buttons is wrong!"
    
    def click(self, accessible):
        procedurelogger.action("click %s" % accessible)
        accessible.click()

    def assertText(self, accessible, text=None):
        """assert text is equal to the input"""

        procedurelogger.action('Assert the text of %s' % accessible)
        procedurelogger.expectedResult('%s text is "%s"' % \
                                                (accessible, accessible.text))
        assert accessible.text == text, '%s is not match with "%s"' % \
                                                (accessible, accessible.text)

    def quit(self):
        self.altF4()
