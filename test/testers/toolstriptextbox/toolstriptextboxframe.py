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

    def inputText(self, textbox, values):
        """
        Wrap strongwind typeText method, imitate key pressing to input values 
	in application 

        """
        textbox.typeText(values)

    def changeTextValue(self, textbox, values):
        """
        Change textbox's text to values to ensure EditableText is implemented

        """
        procedurelogger.action('in %s enter %s' % (textbox, values))
        textbox.text = values

    def assertLabel(self, newlabel):
        """
        assert label is changed to show input newlabel

        """
        procedurelogger.expectedResult("label shows Your input is:%s" % newlabel)

        assert self.label.text == "Your input:%s" % newlabel

    def assertText(self, accessible, textvalue):
        """
        Make sure accessible's text is textvalue

        """
        procedurelogger.expectedResult("the text of %s shows %s" % \
						(accessible,textvalue))
        assert accessible.text == textvalue

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
        expect = ['text/plain']
        result = accessible._accessible.queryStreamableContent().getContentTypes()

        procedurelogger.expectedResult("%s Contents is %s" % (accessible, expect))
        assert result == expect, "Contents %s not match the expected" % result

    # close application window
    def quit(self):
        self.altF4()
