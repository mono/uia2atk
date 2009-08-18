
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
#              Brian G. Merrell <bgmerrell@novell.com>
# Date:        08/07/2008
# Description: label.py wrapper script
#              Used by the label-*.py tests
##############################################################################

import sys
import os
import actions
import states

from strongwind import *
from label import *


# class to represent the main window.
class LabelFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    BUTTON1 = "button1"
    BUTTON2 = "button2"
    SENSTIVE_LABEL = "there is nothing now."
    INSENSITIVE_LABEL = "I'm so insensitive"

    def __init__(self, accessible):
        super(LabelFrame, self).__init__(accessible)
        self.button1 = self.findPushButton(self.BUTTON1)
        self.button2 = self.findPushButton(self.BUTTON2)
        self.sensitive_label = self.findLabel(self.SENSTIVE_LABEL)
        self.insensitive_label = self.findLabel(self.INSENSITIVE_LABEL)
        self.message_box = None # to be found later

    #check the Label text after click button2
    def assertLabel(self, labelText):
        procedurelogger.expectedResult('Label text has been changed to "%s"' % labelText)
        self.findLabel(labelText)

    #check label's text value
    def assertText(self, text_value):
        #initialize label again to get the new text value
        procedurelogger.expectedResult('Label\'s text value shows in accerciser is "%s"' % text_value)
        def resultMatches():
            return self.sensitive_label.text == text_value
        assert retryUntilTrue(resultMatches)

    def assertMessageBox(self):
        self.message_box = self.app.findDialog("message") 
    
    def assertMessageBoxText(self, text_value):
        procedurelogger.action("Check MessageBox label")
        self.message_box.findLabel(text_value)
        procedurelogger.expectedResult("MessageBox label reads: '%s'" %\
                                       text_value)
    
    #close application main window after running test
    def quit(self):
        self.altF4()
