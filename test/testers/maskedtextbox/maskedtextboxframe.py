##############################################################################
# Written by:  Brian G. Merrell <bgmerrell@novell.com>
# Date:        01/13/2009
# Description: maskedtextbox.py wrapper script
#              Used by the maskedtextbox-*.py tests
##############################################################################$

import sys
import os
import actions
import states

from strongwind import *
from maskedtextbox import *

# class to represent the main window.
class MaskedTextBoxFrame(accessibles.Frame):

    def __init__(self, accessible):
        super(MaskedTextBoxFrame, self).__init__(accessible)
        masked_text_boxes = self.findAllTexts(None)
        assert len(masked_text_boxes) == 4,\
                                "There should be exactly four text accessibles"
        self.date_text = masked_text_boxes[0]
        self.phone_text = masked_text_boxes[1]
        self.money_text = masked_text_boxes[2]
        self.blank_text = masked_text_boxes[3]

    def insertText(self, accessible, text, offset=0):
        """Insert text into MaskedTextBox using EditableText::insertText

        Keyword arguments:
        acc -- the MaskedTextBox where the text should be inserted
        offset -- integer offset from the beginning of the MaskedTextBox
                  describing where the text should be inserted.
                  (default 0)
        text -- the text that should be inserted into the MaskedTextBox
        """
        procedurelogger.action('Enter "%s" into %s.' % (text, accessible))
        acc = accessible._accessible
        eti = acc.queryEditableText()
        eti.insertText(offset, text, len(text))

    def assertText(self, accessible, expected_text):
        'Ensure that the accessible contains the text we expect'

        procedurelogger.expectedResult('%s contains the text: %s' %\
                                                   (accessible, expected_text))
        assert accessible.text == expected_text,\
            "\n".join(["%s does not contain the expected text." % accessible,
                       "  Expected: %s" % expected_text,
                       "  Actual: %s" % accessible.text])

        # just for fun, let's make sure the text matches the editable text
        # (this could change if we ever mix editable text with non-editable
        # text for the MaskedTextBox control
        eti = accessible._accessible.queryEditableText()
        etext = eti.getText(0, eti.characterCount)
        assert eti.getText(0, eti.characterCount) == accessible.text,\
                            "\n".join(["Text does not match EditableText.",
                                       "  Text: %s" % accessible.text,
                                       "  EditableText: %s" % etext])

    #close application window
    def quit(self):
        self.altF4()
