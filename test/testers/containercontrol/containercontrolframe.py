# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        02/09/2009
# Description: Application wrapper for containercontrol.py
#              be called by ../containercontrol_basic_ops.py
##############################################################################

"""Application wrapper for containercontrol.py"""

from strongwind import *

class ContainerControlFrame(accessibles.Frame):
    """the profile of the containercontrol sample"""

    def __init__(self, accessible):
        super(ContainerControlFrame, self).__init__(accessible)
        self.panels = self.findAllPanels(None)
        self.labels = self.findAllLabels(None)

        self.panel_top = self.panels[1]
        self.label_top = self.labels[1]

        self.panel_bottom = self.panels[0]
        self.label_bottom = self.labels[0]

        assert len(self.panels) == 2, "The numbers of panel is wrong!"
        assert len(self.labels) == 2, "The numbers of label is wrong!"
    
    def assertText(self, accessible, text=None):
        """assert text is equal to the input"""

        procedurelogger.action('Assert the text of %s' % accessible)
        procedurelogger.expectedResult('%s text is "%s"' % \
                                                (accessible, accessible.text))
        assert accessible.text == text, '%s is not match with "%s"' % \
                                                (accessible, accessible.text)

    def quit(self):
        self.altF4()
