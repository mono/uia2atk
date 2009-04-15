
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        01/07/2009
# Description: tabpage.py wrapper script
#              Used by the tabpage-*.py tests
##############################################################################

import sys
import os
import actions
import states

from strongwind import *
from tabpage import *


# class to represent the main window.
class TabPageFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    TAB0 = "Tab 0"
    TAB1 = "Tab 1"
    TAB2 = "Tab 2"
    TAB3 = "Tab 3"
    LABEL0 = "I'm in tab page 0"
    BUTTON = "Button"

    def __init__(self, accessible):
        super(TabPageFrame, self).__init__(accessible)
        self.tabpage0 = self.findPageTab(self.TAB0)
        #self.tabpage1 = self.findPageTab(self.TAB1)
        #self.tabpage2 = self.findPageTab(self.TAB2)
        #self.tabpage3 = self.findPageTab(self.TAB3)
        self.label0 = self.tabpage0.findLabel(self.LABEL0)
        self.button = self.tabpage0.findPushButton(self.BUTTON)

    # give 'click' action
    def click(self,accessible):
        accessible.click()

    # enter Text Value for EditableText
    def enterTextValue(self, textbox, values):
        procedurelogger.action('in %s enter %s' % (textbox, values))
        textbox.text = values

    # assert Text implementation for TabPage
    def assertText(self, accessible, textvelue):
        procedurelogger.action("check TabPage Text Value")

        procedurelogger.expectedResult('text of %s is %s' \
                                    % (accessible,textvelue))
        assert accessible.text == textvelue
   
    def assertLabeChange(self,newlabel):
        """
        Assert newlabel can be found

        """
        procedurelogger.expectedResult("lable in statusbar is changed to %s" % \
                                           newlabel)

        assert self.findLabel(newlabel) 

    def assertTabChange(self, tabpage):
        """
        Make sure current selected Tab is changed by finding its children

        """
        procedurelogger.expectedResult("you enter to %s" % tabpage)

        self.label = tabpage.findLabel(None)
        if tabpage == self.tabpage1:
            self.textbox = tabpage.findText(None)
        elif tabpage == self.tabpage2:
            self.checkbox = tabpage.findCheckBox(None)
        elif tabpage == self.tabpage3:
            self.radiobutton = tabpage.findRadioButton(None)

    # close application main window after running test
    def quit(self):
        self.altF4()
