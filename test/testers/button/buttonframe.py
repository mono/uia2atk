
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        07/22/2008
# Description: button.py wrapper script
#              Used by the button-*.py tests
##############################################################################$

import sys
import os
import actions
import states

from strongwind import *
from button import *


# class to represent the main window.
class ButtonFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    LABEL = "there is nothing now."
    BUTTON_ONE = "button1"
    BUTTON_TWO = "button2"
    BUTTON_TREE = "button3"
    BUTTON_FOUR = "button4"

    def __init__(self, accessible):
        super(ButtonFrame, self).__init__(accessible)
        self.label = self.findLabel(self.LABEL)
        self.button1 = self.findPushButton(self.BUTTON_ONE)
        self.button2 = self.findPushButton(self.BUTTON_TWO)
        self.button3 = self.findPushButton(self.BUTTON_TREE)
        self.button4 = self.findPushButton(self.BUTTON_FOUR)
 
    def assertText(self, accessible, expected_text):
        """make sure accessible's text is expected"""
        procedurelogger.action("check %s's text" % accessible)
        procedurelogger.expectedResult('%s\'s text is "%s"' % \
                                            (accessible, expected_text))
        assert accessible.text == expected_text, \
                               'actual text is "%s", expected text is "%s"' % \
                               (accessible.text, expected_text)

    # rise message frame window after click button1
    def assertMessage(self):
        self.message = self.app.findDialog('message')

        self.message.findPushButton('OK').click()

    def assertImage(self, accessible, expected_width, expected_height):
        procedurelogger.action('assert the image size of "%s"' % accessible)
        actual_width, actual_height = \
                             accessible._accessible.queryImage().getImageSize()
        procedurelogger.expectedResult('"%s" image size is %s x %s' % \
                                 (accessible, expected_width, expected_height))

        assert actual_width == expected_width, "%s (%s), %s (%s)" % \
                                            ("expected width",
                                              expected_width,
                                             "does not match the actual width",
                                              actual_width)
        assert actual_height == expected_height, "%s (%s), %s (%s)" % \
                                           ("expected height",
                                            expected_height,
                                            "does not match the actual height",
                                            actual_height)

    #close application main window after running test
    def quit(self):
        self.altF4()
