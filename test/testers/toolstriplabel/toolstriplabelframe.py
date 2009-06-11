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

    def assertText(self, accessible, expected_text):
        """assert the accessible's text is equal to the expected text"""

        procedurelogger.action('Check the text of: %s' % accessible)
        actual_text = accessible.text
        procedurelogger.expectedResult('Text is "%s"' % actual_text)
        assert actual_text == expected_text, 'Text was "%s", expected "%s"' % \
                                                (actual_text, expected_text)

    def assertImage(self, accessible, expected_width, expected_height):
        """assert the accessible's image size is equal to the expected size"""

        procedurelogger.action('assert the image size of "%s"' % accessible)
        actual_width, actual_height = \
                             accessible._accessible.queryImage().getImageSize()
        procedurelogger.expectedResult('"%s" image size is %s x %s' % \
                                (accessible, expected_width, expected_height))

        assert actual_width == expected_width, "%s (%s), %s (%s)" % \
                                        ("expected width",
                                         expected_width,
                                        "does not match the actual width",
                                         actual_width)
        assert actual_height == expected_height, "%s (%s), %s (%s)" % \
                                        ("expected height",
                                        expected_height,
                                        "does not match the actual height",
                                        actual_height)

    def quit(self):
        self.altF4()
