##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        12/09/2008
# Description: toolstripdropdownbutton.py wrapper script
#              Used by the toolstripdropdownbutton-*.py tests
##############################################################################$

import sys
import os
import actions
import states

from strongwind import *
from toolstripdropdownbutton import *


# class to represent the main window.
class ToolStripDropDownButtonFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    LABEL = "Please Select one Color from ToolStripDropDownButton"
    DROPDOWN1 = "ToolStripDropDownButton1"
    DROPDOWN2 = "ToolStripDropDownButton2"
    RED = "Red"
    BLUE = "Blue"
    GREEN = "Green"
    ITEM1 = "Item1"
    ITEM2 = "Item2"
    ITEM3 = "Item3"

    def __init__(self, accessible):
        super(ToolStripDropDownButtonFrame, self).__init__(accessible)
        self.label = self.findLabel(self.LABEL)
        self.toolbar = self.findToolBar(None)
        self.menu1 = self.findMenu(self.DROPDOWN1)
        self.menu2 = self.findMenu(self.DROPDOWN2)
        # search menu items in ToolStripDropDownButton1
        self.red = self.menu1.findMenuItem(self.RED, checkShowing=False)
        self.blue = self.menu1.findMenuItem(self.BLUE, checkShowing=False)
        self.green = self.menu1.findMenuItem(self.GREEN, checkShowing=False)
        # search menu items in ToolStripDropDownButton2
        self.item1 = self.menu2.findMenuItem(self.ITEM1, checkShowing=False)
        self.item2 = self.menu2.findMenuItem(self.ITEM2, checkShowing=False)
        self.item3 = self.menu2.findMenuItem(self.ITEM3, checkShowing=False)

    # assert Text implementation for Menu and MenuItem
    def assertText(self, accessible, expected_text):
        """Make sure accessile's text is expected"""
        procedurelogger.action("check %s's text" % accessible)

        procedurelogger.expectedResult("%s's Text is \"%s\"" % \
                                                   (accessible, expected_text))
        actual_text = accessible.text
        assert actual_text == expected_text, \
                   'Text was "%s", epected "%s"' % (actual_text, expected_text)

    #close application window
    def quit(self):
        self.altF4()
