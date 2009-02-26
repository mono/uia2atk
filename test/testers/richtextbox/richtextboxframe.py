# -*- coding: utf8 -*-
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
    TEXT_TOP = '''string c h a r a c t e r s 1 2 3 4 5 6 7 8 9 0 ! @ # $ % ^ & * ( )
- + ç l á ó ô ê â A B C D E F G H I J K L M N O P Q R S T U V W X'''
    TEXT_BOTTOM = "This is some text."

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

    def assertOriginalText(self, accessible):
        procedurelogger.action("Assert that the accesible has it's original text value")
        if accessible is self.richtextbox_top:
            expected_text = "%s" % (self.TEXT_TOP)
        elif accessible is self.richtextbox_bottom:
            expected_text = "%s" % (self.TEXT_BOTTOM)
        else:
            raise ValueError, \
                " ".join(["accessible must be one of the RichTextBox controls",
                          "from the sample application"])
        procedurelogger.expectedResult('%s contains the text: "%s"' % \
                                        (accessible, expected_text))
        actual_text = accessible.text
        assert expected_text == actual_text, \
            'Actual text "%s" does not match expected text "%s"' % \
            (actual_text, expected_text)

    def verifyEditableRangeIndexes(self, start, end):
        pass

    def appendTextTest(self, accessible, text, should_reset=True):
        procedurelogger.action('Append %s with the text "%s"' % \
                                (accessible, text))
        offset = len(accessible.text)
        accessible.insertText(text, offset)
        actual_text = accessible.text
        if accessible is self.richtextbox_top:
            expected_text = "%s%s" % (self.TEXT_TOP, text)
        elif accessible is self.richtextbox_bottom:
            expected_text = "%s%s" % (self.TEXT_BOTTOM, text)
        else:
            raise ValueError, \
                " ".join(["accessible must be one of the RichTextBox controls",
                          "from the sample application"])
        procedurelogger.expectedResult('%s contains the text: "%s"' % \
                                        (accessible, expected_text))
        assert expected_text == actual_text, \
            'Actual text "%s" does not match expected text "%s"' % \
            (actual_text, expected_text)

        if should_reset:
            self.resetText(accessible)
            self.assertOriginalText(accessible)

    def prefixTextTest(self, accessible, text, should_reset=True):
        procedurelogger.action('Prefix %s with the text "%s"' % \
                                (accessible, text))
        character_count = len(accessible.text)
        offset = 0
        accessible.insertText(text, offset)
        actual_text = accessible.text
        if accessible is self.richtextbox_top:
            expected_text = "%s%s" % (text, self.TEXT_TOP)
        elif accessible is self.richtextbox_bottom:
            expected_text = "%s%s" % (text, self.TEXT_BOTTOM)
        else:
            raise ValueError, \
                " ".join(["accessible must be one of the RichTextBox controls",
                          "from the sample application"])
        procedurelogger.expectedResult('%s contains the text: "%s"' % \
                                        (accessible, expected_text))
        assert expected_text == actual_text, \
            'Actual text "%s" does not match expected text "%s"' % \
            (actual_text, expected_text)

        if should_reset:
            self.resetText(accessible)
            self.assertOriginalText(accessible)

    def resetText(self, accessible):
        '''Set RichTextBox controls to their original text'''
        # set the text back to how it was
        if accessible is self.richtextbox_top:
            accessible.text = self.TEXT_TOP
        elif accessible is self.richtextbox_bottom:
            accessible.text = self.TEXT_BOTTOM
        else:
            raise ValueError, \
                " ".join(["accessible must be one of the RichTextBox controls",
                          "from the sample application"]) 

    def insertTextTest(self, accessible, text, offset, should_reset=True):
        '''Insert text starting at offset'''
        procedurelogger.action('Insert text beginning at index %d' % offset)
        accessible.insertText(text, offset)
        actual_text = accessible.text
        if accessible is self.richtextbox_top:
            expected_text = "%s%s%s" % \
                (self.TEXT_TOP[:offset], text, self.TEXT_TOP[offset:])
        elif accessible is self.richtextbox_bottom:
            expected_text = "%s%s%s" % \
                (self.TEXT_BOTTOM[:offset], text, self.TEXT_BOTTOM[offset:])
        else:
            raise ValueError, \
                " ".join(["accessible must be one of the RichTextBox controls",
                          "from the sample application"])
        procedurelogger.expectedResult('%s contains the text: "%s"' % \
                                        (accessible, expected_text))
        assert expected_text == actual_text, \
            'Actual text "%s" does not match expected text "%s"' % \
            (actual_text, expected_text)

        if should_reset:
            self.resetText(accessible)
            self.assertOriginalText(accessible)
        
    def deleteFromEndTest(self, accessible, n, should_reset=True):
        '''Delete n characters from the end of the editable text'''
        procedurelogger.action("Delete n character from the end of the editable test and make sure it actually happens")        
        character_count = len(accessible.text)
        accessible.deleteText(character_count - n)
        actual_text = accessible.text
        if accessible is self.richtextbox_top:
            expected_text = self.TEXT_TOP[:-n]
        elif accessible is self.richtextbox_bottom:
            expected_text = self.TEXT_BOTTOM[:-n]
        else:
            raise ValueError, \
                " ".join(["accessible must be one of the RichTextBox controls",
                          "from the sample application"])
        procedurelogger.expectedResult('%s contains the text: "%s"' % \
                                        (accessible, expected_text))
        assert expected_text == actual_text, \
            'Actual text "%s" does not match expected text "%s"' % \
            (actual_text, expected_text)

        if should_reset:
            self.resetText(accessible)
            self.assertOriginalText(accessible)

    def deleteFromBeginningTest(self, accessible, n, should_reset=True):
        '''Delete n characters from the beginning of the editable text'''
        procedurelogger.action("Delete n character from the beginning of the editable test and make sure it actually happens")        
        accessible.deleteText(0, n)
        actual_text = accessible.text
        if accessible is self.richtextbox_top:
            expected_text = self.TEXT_TOP[n:]
        elif accessible is self.richtextbox_bottom:
            expected_text = self.TEXT_BOTTOM[n:]
        else:
            raise ValueError, \
                " ".join(["accessible must be one of the RichTextBox controls",
                          "from the sample application"])
        procedurelogger.expectedResult('%s contains the text: "%s"' % \
                                        (accessible, expected_text))
        assert expected_text == actual_text, \
            'Actual text "%s" does not match expected text "%s"' % \
            (actual_text, expected_text)

        if should_reset:
            self.resetText(accessible)
            self.assertOriginalText(accessible)

    def deleteFromMiddleTest(self, accessible, start, end, should_reset=True):
        '''Delete range start to end from the middle of the editable text'''
        assert end > start, \
                    "End index (%d) must be greated than start index (%d)" % \
                    (end, start)
        procedurelogger.action("Delete the characters from the start index to the end index, then make sure it actually happens")
        accessible.deleteText(start, end)
        actual_text = accessible.text
        if accessible is self.richtextbox_top:
            expected_text = "%s%s" % \
                                   (self.TEXT_TOP[:start], self.TEXT_TOP[end:])
        elif accessible is self.richtextbox_bottom:
            expected_text = "%s%s" % \
                             (self.TEXT_BOTTOM[:start], self.TEXT_BOTTOM[end:])
        else:
            raise ValueError, \
                " ".join(["accessible must be one of the RichTextBox controls",
                          "from the sample application"])
        procedurelogger.expectedResult('%s contains the text: "%s"' % \
                                        (accessible, expected_text))
        assert expected_text == actual_text, \
            'Actual text "%s" does not match expected text "%s"' % \
            (actual_text, expected_text)

        if should_reset:
            self.resetText(accessible)
            self.assertOriginalText(accessible)

    #close application window
    def quit(self):
        self.altF4()
