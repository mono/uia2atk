
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        02/04/2009
# Description: toolbar.py wrapper script
#              Used by the toolbar-*.py tests
##############################################################################

import sys
import os
import actions
import states

from strongwind import *
from toolbar import *


# class to represent the main window.
class ToolBarFrame(accessibles.Frame):

    def __init__(self, accessible):
        super(ToolBarFrame, self).__init__(accessible)

    #search for toolbar as toolbar role name
    def assertToolBar(self):
        procedurelogger.action('seach for "tool bar" children')

        procedurelogger.expectedResult('succeeded in finding "tool bar"')
        def resultMatches():
            self.toolbar = self.findToolBar(None)
            return self.toolbar
        
        assert retryUntilTrue(resultMatches)

    #search for items on toolbar
    def searchItems(self, itemname, number):
        procedurelogger.action("how many %s items on toolbar" % itemname)

        procedurelogger.expectedResult("%s %s on toolbar" % (number, itemname))

        if itemname == "push button":
            count = self.toolbar.findAllPushButtons(None)
            assert len(count) == number, "find out %s push buttons" % len(count)
        if itemname == "label":
            count = self.toolbar.findAllLabels(None)
            assert len(count) == number, "find out %s label" % len(count)
        if itemname == "separator":
            count = self.toolbar.findAllSeparators(None)
            assert len(count) == number, "find out %s separators" % len(count)
        if itemname == "combo box":
            count = self.toolbar.findAllComboBoxs(None)
            assert len(count) == number, "find out %s combobox" % len(count)
        if itemname == "menu":
            count = self.toolbar.findAllMenus(None)
            assert len(count) == number, "find out %s menu" % len(count)
        if itemname == "menu item":
            count = self.toolbar.findAllMenuItems(None)
            assert len(count) == number, "find out %s menuitem" % len(count)
    
    #close main window
    def quit(self):
        self.altF4()
