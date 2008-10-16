
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

    def __init__(self, accessible):
        super(CheckBoxFrame, self).__init__(accessible)
        self.check1 = self.findCheckBox(self.CHECK_ONE)
        self.check2 = self.findCheckBox(self.CHECK_TWO)
        self.check3 = self.findCheckBox(self.CHECK_THREE)
        self.check4 = self.findCheckBox(self.CHECK_FORE)

    #give 'click' action
    def click(self,button):
        button.click()

    #check the state after click checkbox
    def assertChecked(self, accessible):
        'Raise exception if the accessible does not match the given result'   
        procedurelogger.expectedResult('"%s" is %s' % (accessible, 'checked'))

        def resultMatches():
            return accessible.checked
	
        assert retryUntilTrue(resultMatches)

    def assertUnchecked(self, accessible):
        'Raise exception if the accessible does not match the given result'   
        procedurelogger.expectedResult('%s is %s.' % (accessible, "unchecked"))

        def resultMatches():
            return not accessible.checked
	
        assert retryUntilTrue(resultMatches)

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
