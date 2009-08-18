##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        12/11/2008
# Description: toolstriptextbox.py wrapper script
#              Used by the toolstriptextbox-*.py tests
##############################################################################$

import sys
import os
import actions
import states

from strongwind import *
from toolstriptextbox import *


# class to represent the main window.
class ToolStripTextBoxFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    LABEL = "Your input:"
    TEXTBOX1 = "ToolStripTextBox1"
    TEXTBOX2 = "ToolStripTextBox2"
    TEXTBOX2 = "ToolStripTextBox2"

    def __init__(self, accessible):
        super(ToolStripTextBoxFrame, self).__init__(accessible)
        self.label = self.findLabel(self.LABEL)
        self.textboxes = self.findAllTexts(None)
        self.singleline = self.textboxes[0]
        self.multiline = self.textboxes[1]
        self.readonly = self.textboxes[2]

    def setTextValue(self, textbox, value):
        """
        Change textbox's text to value to ensure EditableText is implemented
        """
        procedurelogger.action('in %s enter %s' % (textbox, value))
        textbox.text = value

    def assertLabel(self, text):
        """
        assert label text matches the expected text
        """
        expected_text = "Your input:%s" % text
        procedurelogger.expectedResult("Label text is \"%s\"" % expected_text)
        actual_text = self.label.text
        assert actual_text == expected_text,\
            'Label text was "%s", expected "%s"' % (actual_text, expected_text)

    def assertText(self, accessible, expected_text):
        """
        Make sure accessible's text is expected_text
        """
        procedurelogger.expectedResult("the text of %s is \"%s\"" % \
						                              (accessible, expected_text))
        actual_text = accessible.text
        assert actual_text == expected_text, 'Text was "%s", expected "%s"' % \
                                                   (actual_text, expected_text)

    def assertNameAndDescription(self, accessible, name, description):
        """
        Make sure accessible's AccessibleName and AccessibleDescription are 
	expected
        """
        procedurelogger.action("Verify %s AccessibleName and AccessibleDescription" % accessible)
 
        procedurelogger.expectedResult('name is "%s", description is "%s"' \
                                               % (name, description))
        assert accessible.name == name and accessible.description == description, \
                               "name is: %s, description is: %s" \
                                % (accessible.name, accessible.description)

    def assertStreamableContent(self, accessible):
        procedurelogger.action("Verify Streamable Content for %s" % accessible)
        expected_types = ['text/plain']
        actual_types = \
              accessible._accessible.queryStreamableContent().getContentTypes()
        procedurelogger.expectedResult("%s Contents is %s" % \
                                                  (accessible, expected_types))
        assert actual_types == expected_types, \
                                'Content types was "%s", expected "%s"' % \
                                (actual_types, expected_types)

    # close application window
    def quit(self):
        self.altF4()
