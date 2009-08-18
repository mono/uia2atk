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

    def assertURI(self, accessible, expected_uri):
        '''
        Ensure that the URI contained in the hypertext interface is what we
        expect.  This method should only be called if the IsLink property
        is set for the ToolStripLabel
        '''
        # This method has not been tested and will likely need some cleanup
        # once BUG501526 is fixed.
        hi = accessible._accessible.queryHyperlink()
        n_anchors = hi.nAnchors
        assert hi.nAnchors == 1, \
                            "Expected one anchor, got %s" % n_anchors
        # the entire ToolStripLabel should be a link, so we should ensure
        # that the startIndex is 0 and the endIndex matches the number of
        # characters in the text.  (One might think that the end index should
        # be the number of characters in the text minus one, but that doesn't
        # appear to be the case
        start_index = hi.startIndex
        assert hi.startIndex == 0, \
            "Expected the start index of the URI to be 0, got %s" % start_index
        end_index = hi.endIndex
        text_character_count = accessible._accessible.queryText().characterCount
        assert hi.endIndex == text_character_count, \
                       "Expected the end index of the URI to be %s, got %s" % \
                       (text_character_count, end_index)
        # finally make sure the expected URI matches the actual URI
        actual_uri = hi.getURI(0)
        assert actual_uri == expected_uri, \
                                       'Actual URI was "%s", expected "%s"' % \
                                       (actual_uri, expected_uri)

    def quit(self):
        self.altF4()
