# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        12/10/2008
# Description: Application wrapper for toolstriplabel.py
#              be called by ../toolstriplabel_basic_ops.py
##############################################################################

"""Application wrapper for toolstriplabel.py"""

from strongwind import *

class ToolStripLabelFrame(accessibles.Frame):
    """the profile of the toolstriplabel sample"""

    def __init__(self, accessible):
        super(ToolStripLabelFrame, self).__init__(accessible)
        self.toolstriplabel = self.findLabel("Mono\nAccessibility")
        self.toolstriplabel_image = self.findLabel("ToolStripLabel with image")

    def assertText(self, accessible, text=None):
        """assert text is equal to the input"""
        procedurelogger.action('Assert the text of %s' % accessible)
        procedurelogger.expectedResult('%s text is "%s"' % \
                                                (accessible, accessible.text))
        assert accessible.text == text, '%s text is not match with "%s"' % \
                                                (accessible, accessible.text)

    def quit(self):
        self.altF4()
