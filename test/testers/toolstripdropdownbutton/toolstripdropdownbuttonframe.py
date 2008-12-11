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
    LABEL = "Please Select one Color from ComboxBox"
    DROPDOWN1 = "ToolStripDropDownButton1"
    DROPDOWN2 = "ToolStripDropDownButton2"

    def __init__(self, accessible):
        super(ToolStripDropDownButtonFrame, self).__init__(accessible)
        self.label = self.findLabel(self.LABEL)
        self.menu1 = self.findMenu(self.DROPDOWN1)
        self.menu2 = self.findMenu(self.DROPDOWN2)

    #give 'click' action
    def click(self, button):
        procedurelogger.action("click %s" % button)
        button.click()
        #search menu items in ToolStripDropDownButton1
        self.red = self.menu1.findMenuItem("Red")
        self.blue = self.menu1.findMenuItem("Blue")
        self.green = self.menu1.findMenuItem("Green")
        #search menu items in ToolStripDropDownButton2
        items = ["Item1", "Item2", "Item3"]
        self.item = dict([(x, self.menu2.findMenuItem(y)) for x in range(3) for y in items])

    #assert the toolstripprogress's percent after click button
    def assertLabel(self, newlabel):
        procedurelogger.expectedResult('label shows "%s"' % newlabel)

        def resultMatches():
            return self.label.text == newlabel
        assert retryUntilTrue(resultMatches)

    #assert Text implementation for Menu and MenuItem
    def assertText(self, accessible, textValue):
        procedurelogger.action("check %s's Text" % accessible)

        procedurelogger.expectedResult("%s's Text is %s" % (accessible,textValue))
        assert accessible.text == textValue

    #assert Selection implementation for Menu
    def assertSelectionChild(self, accessible, childIndex):
        procedurelogger.action('selecte childIndex %s in "%s"' % (childIndex, accessible))

        accessible.selectChild(childIndex)

    def assertClearSelection(self, accessible):
        procedurelogger.action('clear selection in "%s"' % (accessible))

        accessible.clearSelection()

    #close application window
    def quit(self):
        self.altF4()
