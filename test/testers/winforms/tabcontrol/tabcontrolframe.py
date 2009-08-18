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

    def __init__(self, accessible):
        super(TabControlFrame, self).__init__(accessible)
        self.tabcontrol = self.findPageTabList(None)
        self.tabpage0 = self.tabcontrol.findPageTab(self.TAB0)
        self.tabpage1 = self.tabcontrol.findPageTab(self.TAB1)
        self.tabpage2 = self.tabcontrol.findPageTab(self.TAB2)
        self.tabpage3 = self.tabcontrol.findPageTab(self.TAB3)
   
    def selectChild(self, accessible, child_index):
        """
        Use the AtkSelection interface to select the child of the accessible at
        child_index
        """
        procedurelogger.action('selected child_index %s in "%s"' % \
                                                     (child_index, accessible))
        accessible.selectChild(child_index)

    def assertLabelText(self, tabname):
        """
        Make sure label shows the current tab is expected tabname
        """
        procedurelogger.expectedResult("you enter to %s" % tabname)

        self.findLabel("The current tab is: %s" % tabname)

    # close application main window after running test
    def quit(self):
        self.altF4()
