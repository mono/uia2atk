##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        11/03/2009
# Description: textbox.py wrapper script
#              Used by the textbox-*.py tests
##############################################################################

# imports

from strongwind import *
from textbox import *


# class to represent the main window.
class TextBoxFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    BOX_ONE = "Read And Write"
    BOX_TWO = "Read Only"
    BOX_THREE = "Search"

    def __init__(self, accessible):
        super(TextBoxFrame, self).__init__(accessible)
        self.frame = self.findDocumentFrame("TextBoxSample")
        self.filler = self.frame.findFiller("Silverlight Control")

        self.textbox1 = self.filler.findText(self.BOX_ONE)
        self.scrollbars = self.textbox1.findAllScrollBars(None)
        assert len(self.scrollbars) == 2, \
                          "actual number of scrolbar is %s, expected %s" % \
                                 (len(self.scrollbars), 2)

        self.textbox2 = self.filler.findText(self.BOX_TWO)
        self.textbox3 = self.filler.findText(self.BOX_THREE)

    def assertEditableText(self, accessible, expected_text):
        """assert that the accessible text is equal to the expected text"""

        procedurelogger.action('Assert the text of %s' % accessible)
        procedurelogger.expectedResult('%s text is "%s"' % \
                                                (accessible, expected_text))

        eti = accessible._accessible.queryEditableText()
        actual_text = eti.getText(0, eti.characterCount)

        assert actual_text == expected_text, \
                    'Actual text "%s" does not match expected text "%s"' % \
                    (actual_text, expected_text)

    def assertStreamableContent(self, accessible):
        procedurelogger.action("Verify Streamable Content for %s" % accessible)

        expect = ['text/plain']
        result = accessible._accessible.queryStreamableContent().getContentTypes()
        procedurelogger.expectedResult("%s Contents is %s" % (accessible, expect))
        assert expect == result, "Contents %s not match the expected" % result

