
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        11/10/2008
# Description: combobox_simple.py wrapper script
#              Used by the combobox_simple-*.py tests
##############################################################################

import sys
import os
import actions
import states
import pyatspi

from strongwind import *
from combobox_dropdownlist import *


# class to represent the main window.
class ComboBoxDropDownListFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    LABEL1 = "You select "

    def __init__(self, accessible):
        super(ComboBoxDropDownListFrame, self).__init__(accessible)
        self.label1 = self.findLabel(self.LABEL1)
        self.combobox = self.findComboBox(None)
        self.menu = self.findMenu("", checkShowing=False)
        self.vscrollbar = None

        self.menu_items = {}
        # XXX: This causes bug 462447
        for i in range(10):
            self.menu_items[i] = self.findMenuItem(str(i), checkShowing=False)
        self.menu_items[0] = self.findMenuItem("0", checkShowing=False) 

    #check the label after click listitem
    def assertLabel(self, selected_item):
        expected_text = "You select %s" % selected_item
        procedurelogger.expectedResult('label1 reads: %s' % expected_text)
        assert self.label1.text == expected_text

    #assert Text implementation for MenuItem
    def assertItemText(self, textValue=None):
        procedurelogger.action('check MenuItem\'s Text Value')

        for i in range(10):
            procedurelogger.expectedResult('Menu item %s\'s text is "%s"' % \
                                                        (self.menu_items[i],i))
            assert self.menu_items[i].text == str(i)

    #assert Text value after do click action
    def assertText(self, accessible, value):
        procedurelogger.expectedResult('the text value of %s is %s' % (accessible,value))
        assert accessible.text == str(value)

    #assert Selection implementation for Menu
    def assertSelectionChild(self, accessible, childIndex):
        procedurelogger.action('select childIndex %s in "%s"' % \
                                                      (childIndex, accessible))
        accessible.selectChild(childIndex)

    def assertClearSelection(self, accessible):
        procedurelogger.action('clear selection in "%s"' % (accessible))

        accessible.clearSelection()

    def scrollToBottom(self):
        self.vscrollbar = self.app.findScrollBar("Vertical Scroll Bar")
        # this should be a vertical scroll bar
        assert self.vscrollbar.vertical, "The scroll bar should be vertical"
        procedurelogger.action('Scroll to the bottom of the window')

        maxVal = self.vscrollbar._accessible.queryValue().maximumValue

        procedurelogger.expectedResult(" ".join(["Scroll bar scrolls to the",
                                                 "bottom of the window and",
                                                 "the scroll bar value is %s" \
                                                                    % maxVal]))
        self.vscrollbar.value = maxVal
        sleep(config.SHORT_DELAY)
        assert self.menu_items[9].showing, "Menu item 9 should be showing"
        assert self.vscrollbar.value == maxVal, \
             "%s's value should equal the maximum value (%d)" % \
             (self.vscrollbar, maxVal)

    def scrollToTop(self):
        self.vscrollbar = self.app.findScrollBar("Vertical Scroll Bar")
        # this should be a vertical scroll bar
        assert self.vscrollbar.vertical, "The scroll bar should be vertical"
        procedurelogger.action('Scroll to the top of the window')

        minVal = self.vscrollbar._accessible.queryValue().minimumValue

        procedurelogger.expectedResult(" ".join(["Scroll bar scrolls to the",
                                                 "top of the window and",
                                                 "the scroll bar value is %s" \
                                                                    % minVal]))
        self.vscrollbar.value = minVal
        sleep(config.SHORT_DELAY)
        assert self.menu_items[0].showing, "Menu item 0 should be showing"
        assert self.vscrollbar.value == minVal, \
             "%s's value should equal the minimum value (%d)" % \
             (self.vscrollbar, minVal)

    def findVerticalScrollBar(self):
        self.vscrollbar = self.app.findFrame("ComboBox control")
        # this should be a vertical scroll bar
        assert self.vscrollbar.vertical, "The scroll bar should be vertical"
    
    #close application main window after running test
    def quit(self):
        self.altF4()
