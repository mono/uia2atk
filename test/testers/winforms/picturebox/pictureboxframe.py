##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        08/14/2008
# Description: picturebox.py wrapper script
#              Used by the picturebox-*.py tests
##############################################################################$

import sys
import os
import actions
import states

from strongwind import *
from picturebox import *

# class to represent the main window.
class PictureBoxFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    BUTTON = "openSUSE"
    LABEL = "show desktop-blue_soccer400x500.jpg"

    def __init__(self, accessible):
        super(PictureBoxFrame, self).__init__(accessible)
        self.button = self.findPushButton(self.BUTTON)
        self.label = self.findLabel(self.LABEL)
        self.icon = self.findIcon(self.LABEL)

    def assertName(self, accessible, expected_name):
        """Make sure accessible's name is expected"""
        procedurelogger.action("check %s's name" % accessible.roleName)
        procedurelogger.expectedResult("%s's name is: %s" % \
                                       (accessible.roleName, expected_name))

        actual_name = accessible.name
        assert actual_name == expected_name, \
                "actual name is:%s, expected:%s" % (actual_name, expected_name)    

    def assertImageSize(self, accessible, expected_width=0, expected_height=0):
        """Make sure accessible's image size is expected"""
        procedurelogger.action("assert %s's image size" % accessible)
        size = accessible._accessible.queryImage().getImageSize()

        procedurelogger.expectedResult('"%s" image size is %s x %s' %
                                 (accessible, expected_width, expected_height))

        assert expected_width == size[0], "%s (%s), %s (%s)" %\
                                            ("expected width",
                                              expected_width,
                                             "does not match actual width",
                                              size[0])
        assert expected_height == size[1], "%s (%s), %s (%s)" %\
                                            ("expected height",
                                              expected_height,
                                             "does not match actual height",
                                              size[1])
    
    
    # close application main window after running test
    def quit(self):
        self.altF4()
