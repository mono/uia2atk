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
        self.textboxs = self.findAllTexts(None)
        self.singleline = self.textboxs[0]
        self.multiline = self.textboxs[1]
        self.readonly = self.textboxs[2]

    #input value into text box
    def inputText(self, textbox, values):
        textbox.typeText(values)

    #enter Text Value for EditableText
    def enterTextValue(self, textbox, values):
        procedurelogger.action('in %s enter %s' % (textbox, values))
        textbox.text = values

    #assert TextChanged event, label would show what is your input
    def assertLabel(self, newlabel):
        procedurelogger.expectedResult("label shows Your input:%s" % newlabel)

        def resultMatches():
            return self.label.text == "Your input:" + newlabel
        assert retryUntilTrue(resultMatches)

    #assert Text implementation, the Text would be the same as your input
    def assertText(self, accessible, textValue):
        procedurelogger.expectedResult("%s shows %s" % (accessible,textValue))
        assert accessible.text == textValue

    #assert Name and Description
    def assertNameDescription(self, accessible, name, description):
        procedurelogger.action("Verify %s AccessibleName and AccessibleDescription" % accessible)
 
        procedurelogger.expectedResult('name is "%s", description is "%s"' \
                                               % (name, description))
        assert accessible.name == name and accessible.description == description, \
                               "name is: %s, description is: %s" \
                                % (accessible.name, accessible.description)

    #assert Streamable Content implementation
    def assertContent(self, accessible):
        procedurelogger.action("Verify Streamable Content for %s" % accessible)
        #text in gtk.textview shows the expected contents
        expect = ['application/x-gtk-text-buffer-rich-text', 'text/plain']
        result = accessible._accessible.queryStreamableContent().getContentTypes()

        procedurelogger.expectedResult("%s Contents is %s" % (accessible, expect))
        assert result == expect, "Contents %s not match the expected" % result

    #close application window
    def quit(self):
        self.altF4()
