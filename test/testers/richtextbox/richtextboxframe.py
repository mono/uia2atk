##############################################################################
# Written by:  Brian G. Merrell <bgmerrell@novell.com>
# Date:        01/20/2009
# Description: richtextbox.py wrapper script
#              Used by the richtextbox-*.py tests
##############################################################################$

import sys
import os
import actions
import states

from strongwind import *
from richtextbox import *

# class to represent the main window.
class RichTextBoxFrame(accessibles.Frame):

    N_TEXTS = 2

    def __init__(self, accessible):
        super(RichTextBoxFrame, self).__init__(accessible)
        self.richtextboxes = self.findAllTexts("")
        n_richtextboxes = len(self.richtextboxes)
        assert len(self.richtextboxes) == self.N_TEXTS, \
                "Exactly %d text accessibles should exist (found %d)" % \
                (self.N_TEXTS, n_richtextboxes)
        self.richtextbox_top = self.richtextboxes[0]
        self.richtextbox_bottom = self.richtextboxes[1]

    def assertEditableText(self, accessible, expected_text):
        '''
        Verify that the accessible's editable text matches what is
        expected
        '''
        
        eti = accessible._accessible.queryEditableText()
        actual_text = eti.getText(0, eti.characterCount)
        assert actual_text == expected_text, \
            'Actual text "%s" does not match expected text "%s"' % \
            (actual_text, expected_text)

    def verifyEditableRangeIndexes(self, start, end):
        pass

    #close application window
    def quit(self):
        self.altF4()
