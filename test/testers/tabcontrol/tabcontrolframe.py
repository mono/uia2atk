
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        01/09/2009
# Description: tabcontrol.py wrapper script
#              Used by the tabcontrol-*.py tests
##############################################################################

import sys
import os
import actions
import states

from strongwind import *
from tabcontrol import *


# class to represent the main window.
class TabControlFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    TAB0 = "Tab 0"
    TAB1 = "Tab 1"
    TAB2 = "Tab 2"
    TAB3 = "Tab 3"
    LABEL0 = "I'm in tab page 0"
    LABEL1 = "I'm in tab page 1"
    LABEL2 = "I'm in tab page 2"
    LABEL3 = "I'm in tab page 3"
    BUTTON = "Button"
    CHECKBOX = "CheckBox"
    RADIOBUTTON = "RadioButton"

    def __init__(self, accessible):
        super(TabControlFrame, self).__init__(accessible)
        self.tabcontrol = self.findPageTabList(None)
        self.tabpage0 = self.findPageTab(self.TAB0)
        self.tabpage1 = self.findPageTab(self.TAB1)
        self.tabpage2 = self.findPageTab(self.TAB2)
        self.tabpage3 = self.findPageTab(self.TAB3)
        self.label0 = self.tabpage0.findLabel(self.LABEL0)
        self.label1 = self.tabpage1.findLabel(self.LABEL1, checkShowing=False)
        self.label2 = self.tabpage2.findLabel(self.LABEL2, checkShowing=False)
        self.label3 = self.tabpage3.findLabel(self.LABEL3, checkShowing=False)
        self.button = self.tabpage0.findPushButton(self.BUTTON)
        self.checkbox = self.tabpage2.findCheckBox(self.CHECKBOX, checkShowing=False)
        self.radiobutton = self.tabpage3.findRadioButton(self.RADIOBUTTON, checkShowing=False)
        self.textbox = self.tabpage1.findText(None, checkShowing=False)
   
    #assert AtkSelection implementation
    def assertSelectionChild(self, accessible, childIndex):
        procedurelogger.action('selecte childIndex %s in "%s"' % (childIndex, accessible))

        accessible.selectChild(childIndex)

    #assert if enter to a new tab page after press keyLeft/Right by checking 
    #label that shows in statusbar
    def assertTabChange(self, tabname):
        procedurelogger.expectedResult("you enter to %s" % tabname)

        assert self.findLabel("The current tab is: %s" % newlabel)

    #close application main window after running test
    def quit(self):
        self.altF4()
