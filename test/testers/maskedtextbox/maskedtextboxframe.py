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

    def assertCharacterCount(self, accessible, expected_char_count):
        'Ensure that the accessible character count is what we expect'

        eti = accessible._accessible.queryEditableText()
        char_count = eti.characterCount
        assert expected_char_count == char_count,\
                "Character count was %d instead of %d" %\
                                              (char_count, expected_char_count)

    def assertInsertRightOrder(self, accessible, expected_range):
        'Ensure that the accessible character inputted is follow the right order'
        procedurelogger.action(' Inserting characters one at a time into %s ,start from 0.' % accessible)
 
        procedurelogger.expectedResult('%s should between 0 and %s and in the ascending order like (860)-123-4567' %\
                                                   (accessible, expected_range-2))
        # insert num from 0 to 9 , the expected_text should not be (860)-123-4567,but the (860)-145-6891    
        # because of the "if you try to insert into a mask slot, forward to the next open spot" behavior. 
        expected_text='(860)-145-6891'
        eti = accessible._accessible.queryEditableText()
        #insert_value = ''
        j= 0
        for i in range(3, expected_range+3):
            #insert_value+=str(i) 
            eti.insertText(i, str(j), 1)
            j+=1
        sleep(config.SHORT_DELAY)
        etext = eti.getText(0, eti.characterCount)

        print "the extext is : %s" % etext
        print "the accessible.text is : %s" % accessible.text
        assert eti.getText(0, eti.characterCount) == expected_text,\
            "\n".join(["%s does not contain the expected text." % accessible,
                       "  Expected: %s" % expected_text,
                       "  Actual: %s" % accessible.text])



    #close application window
    def quit(self):
        self.altF4()
