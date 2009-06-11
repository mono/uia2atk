# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        02/20/2008
# Description: Application wrapper for textbox.py
#              be called by ../textbox_basic_ops.py
##############################################################################

"""Application wrapper for textbox.py"""

from strongwind import *

class TextBoxFrame(accessibles.Frame):
    """the profile of the textbox sample"""

    LABEL_NORMAL = "explicitly set name for label"
    LABEL_MLINE = "Multi-Line TextBox"
    LABEL_PASSWD = "Password TextBox"
    LABEL_NONEDIT = "non-Editable TextBox"

    def __init__(self, accessible):
        super(TextBoxFrame, self).__init__(accessible)
        self.label_normal = self.findLabel(self.LABEL_NORMAL)
        self.label_mline = self.findLabel(self.LABEL_MLINE)
        self.label_passwd = self.findLabel(self.LABEL_PASSWD)
        self.label_nonedit = self.findLabel(self.LABEL_NONEDIT)

        self.textboxes = self.findAllTexts(None)
        assert len(self.textboxes) == 4, "the number of textboxes is incorrect"

        self.textbox_normal = self.textboxes[0]
        self.textbox_mline = self.textboxes[1]
        self.textbox_passwd = self.textboxes[2]
        self.textbox_nonedit = self.textboxes[3]

        # the scroll bar accessibles shouldn't exist yet
        scrollbars = accessible.findAllScrollBars(None)
        assert len(scrollbars) == 0, \
                                   "No scroll bar accessibles should exist yet"

    def assertEditableText(self, accessible, expected_text):
        """assert that the accessible text is equal to the expected text"""

        procedurelogger.action('Assert the text of %s' % accessible)
        procedurelogger.expectedResult('%s text is "%s"' % \
                                                (accessible, accessible.text))

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

    def findScrollBars(self, accessible):
        procedurelogger.action("find the scroll bars of multiline textbox" % \
                                                                accessible)
        self.scrollbars = accessible.findAllScrollBars(None)
        assert len(self.scrollbars) == 2, \
                                    "the number of Scroll Bars is not correct."
        self.horizontal_scroll_bar, self.vertical_scroll_bar = self.scrollbars

    def quit(self):
        self.altF4()
