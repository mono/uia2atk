
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        07/24/2008
# Description: checkbox.py wrapper script
#              Used by the checkbox-*.py tests
##############################################################################$

import sys
import os
import actions
import states

from strongwind import *
from checkbox import *


# class to represent the main window.
class CheckBoxFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    CHECK_ONE = "Bananas"
    CHECK_TWO = "Chicken"
    CHECK_THREE = "Stuffed Peppers"
    CHECK_FORE = "Beef"
    CHECK_FIVE = "Fried Lizard"
    CHECK_SIX = "Soylent Green"

    def __init__(self, accessible):
        super(CheckBoxFrame, self).__init__(accessible)
        self.check1 = self.findCheckBox(self.CHECK_ONE)
        self.check2 = self.findCheckBox(self.CHECK_TWO)
        self.check3 = self.findCheckBox(self.CHECK_THREE)
        self.check4 = self.findCheckBox(self.CHECK_FORE)
        self.check5 = self.findCheckBox(self.CHECK_FIVE)
        self.check6 = self.findCheckBox(self.CHECK_SIX)

    #give 'click' action
    def click(self,button):
        button.click()

    # assert the size of an image in the CheckBox
    def assertImageSize(self, button, width=-1, height=-1):
        procedurelogger.action("assert %s's image size" % button)
        size = button._accessible.queryImage().getImageSize()

        procedurelogger.expectedResult('"%s" image size is %s x %s' %
                                                  (button, width, height))

        assert width == size[0], "%s (%s), %s (%s)" %\
                                            ("expected width",
                                              width,
                                             "does not match actual width",
                                              size[0])
        assert height == size[1], "%s (%s), %s (%s)" %\
                                            ("expected height",
                                              height,
                                             "does not match actual height",
                                              size[1]) 
    
    #close application main window after running test
    def quit(self):
        self.altF4()
