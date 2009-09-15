#!/usr/bin/env python
# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        01/08/2008
# Description: main test script of toolstripsplitbutton
#              ../samples/winforms/toolstripsplitbutton.py is the test sample script
#              toolstripsplitbutton/* are the wrappers of toolstripsplitbutton test sample 
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of ToolStripSplitButton widget
"""

# imports
import sys
from toolstripsplitbutton import *
from helpers import *
from actions import *
from states import *
from sys import argv

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the toolstripsplitbutton sample application
try:
  app = launchToolStripSplitButton(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
tssbFrame = app.toolStripSplitButtonFrame

# make sure the toggle button and the push button don't have the same absolute
# position.
# BUG473795 ToolStripSplitButton: the position size of its children are
# incorrect
#tssbFrame.assertDifferentAbsolutePositions(tssbFrame.push_button,
#                                           tssbFrame.toggle_button)

tssbFrame.click(tssbFrame.push_button)
sleep(config.SHORT_DELAY)
tssbFrame.assertText(tssbFrame.click_label, "1 clicks")

##############################
# check toolstripsplitbutton children's AtkAction
##############################
actionsCheck(tssbFrame.push_button, "Button")
actionsCheck(tssbFrame.toggle_button, "ToggleButton")
actionsCheck(tssbFrame.menuitem_10, "MenuItem")
actionsCheck(tssbFrame.menuitem_12, "MenuItem")
actionsCheck(tssbFrame.menuitem_14, "MenuItem")

##############################
# check toolstripsplitbutton's AtkAccessible
##############################
statesCheck(tssbFrame.push_button, "Button", invalid_states=["focusable"])
# BUG504335 Toggle button accessible of a ToolStripSplitButton has extraneous
# "focusable" state
#statesCheck(tssbFrame.toggle_button, "ToggleButton", invalid_states=["focusable"])
# BUG486335 MenuItem, ToolStripMenuItem: extraneous "showing" state of menu
# item when it is not showing
#statesCheck(tssbFrame.menuitem_10, "MenuItem", invalid_states=["showing"])
#statesCheck(tssbFrame.menuitem_12, "MenuItem", invalid_states=["showing"])
#statesCheck(tssbFrame.menuitem_14, "MenuItem", invalid_states=["showing"])

# click on the toggle button and check the menu item states
tssbFrame.click(tssbFrame.toggle_button)
sleep(config.SHORT_DELAY)
statesCheck(tssbFrame.menuitem_10, "MenuItem")
statesCheck(tssbFrame.menuitem_12, "MenuItem")
statesCheck(tssbFrame.menuitem_14, "MenuItem")

# press Up/Down
tssbFrame.keyCombo("Down", grabFocus=False)
sleep(config.SHORT_DELAY)
# BUG503725 - Menu item loses "focusable" state when that item becomes focused
#statesCheck(tssbFrame.menuitem_10, "MenuItem", add_states=["selected", "focused"])
statesCheck(tssbFrame.menuitem_14, "MenuItem")

tssbFrame.keyCombo("Up", grabFocus=False)
sleep(config.SHORT_DELAY)
# BUG503725 - Menu item loses "focusable" state when that item becomes focused
#statesCheck(tssbFrame.menuitem_14, "MenuItem", add_states=["selected", "focused"])
statesCheck(tssbFrame.menuitem_10, "MenuItem")

# mouseClick
tssbFrame.menuitem_10.mouseClick()
sleep(config.SHORT_DELAY)
tssbFrame.assertText(tssbFrame.font_size_label, "The current font size is 10")
# BUG503725 - Menu item loses "focusable" state when that item becomes focused
# BUG505493 - ToolStripSplitButton: Extraneous "selected" and "focused" states
# after performing 'click' action on a menu item
#statesCheck(tssbFrame.menuitem_10, "MenuItem")
# BUG486335 - Extraneous "showing" state of menu item when it is not showing
#statesCheck(tssbFrame.menuitem_12, "MenuItem", invalid_states=["showing"])

# select item from splitbutton
tssbFrame.click(tssbFrame.menuitem_12)
sleep(config.SHORT_DELAY)
tssbFrame.assertText(tssbFrame.font_size_label, "The current font size is 12")
# BUG503725 - Menu item loses "focusable" state when that item becomes focused
# BUG505493 - ToolStripSplitButton: Extraneous "selected" and "focused" states
# after performing 'click' action on a menu item
#statesCheck(tssbFrame.menuitem_12, "MenuItem")
# BUG486335 - Extraneous "showing" state of menu item when it is not showing
#statesCheck(tssbFrame.menuitem_10, "MenuItem", invalid_states=["showing"])

# select the last item from splitbutton
tssbFrame.click(tssbFrame.menuitem_14)
sleep(config.SHORT_DELAY)
tssbFrame.assertText(tssbFrame.font_size_label, "The current font size is 14")
# BUG503725 - Menu item loses "focusable" state when that item becomes focused
# BUG505493 - ToolStripSplitButton: Extraneous "selected" and "focused" states
# after performing 'click' action on a menu item
#statesCheck(tssbFrame.menuitem_14, "MenuItem")
# BUG486335 - Extraneous "showing" state of menu item when it is not showing
#statesCheck(tssbFrame.menuitem_12, "MenuItem", invalid_states=["showing"])

##############################
# check toolstripsplitbutton's AtkSelection
##############################
# this whole AtkSelection section is blocked by BUG505759
# BUG505759 - Erroneous selectChild behavior for ToolStripSplitButton
#tssbFrame.selectChild(tssbFrame.toggle_button, 0)
#sleep(config.SHORT_DELAY)
# the label should not change to "...font size is 10" as if item 0 was clicked
#tssbFrame.assertText(tssbFrame.font_size_label, "The current font size is 12")
#statesCheck(tssbFrame.menuitem_10, "MenuItem", add_states=["selected"]
# BUG486335 - Extraneous "showing" state of menu item when it is not showing
#statesCheck(tssbFrame.menuitem_12, "MenuItem")

# click to open the drop down menu (this test might change depending on
#tssbFrame.click(tssbFrame.toggle_button)
#sleep(config.SHORT_DELAY)
# Perform selectChild action on "14" (item 2) while the drop down menu is open
# and then check the text of the label and the states of the accessibles. This
# test may need to change depending on how the "focused" state issue in
# BUG505759 is resolved)
# BUG505759 - Erroneous selectChild behavior for ToolStripSplitButton
# select the last item from combobox
#tssbFrame.selectChild(tssbFrame.toggle_button, 2)
#sleep(config.SHORT_DELAY)
# the label should not change to "...font size is 14" as if item 2 was clicked
#tssbFrame.assertText(tssbFrame.font_size_label, "The current font size is 12")
#statesCheck(tssbFrame.menuitem_14,
#            "MenuItem",
#            add_states=["selected", "focused"])
#statesCheck(tssbFrame.menuitem_10, "MenuItem")

# Press Enter while "14" (item 2) is selected, then check the label (which
# should change at this point) and the accessibles' states
tssbFrame.keyCombo("Enter", grabFocus=False)
sleep(config.SHORT_DELAY)
#tssbFrame.assertText(tssbFrame.font_size_label, "The current font size is 14")
#statesCheck(tssbFrame.menuitem_10, "MenuItem")
#statesCheck(tssbFrame.menuitem_12, "MenuItem")
#statesCheck(tssbFrame.menuitem_14, "MenuItem")
# end BUG505759 woes

##############################
# check toolstripsplitbutton's AtkImage
##############################
tssbFrame.assertImage(tssbFrame.push_button, 16, 16)

##############################
# End
##############################
# close application frame window
tssbFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
