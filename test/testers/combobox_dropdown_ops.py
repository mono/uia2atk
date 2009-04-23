#!/usr/bin/env python
###############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        11/14/2008
# Description: Test accessibility of combobox_dropdown widget 
#              Use the comboboxdropdownframe.py wrapper script
#              Test the samples/combobox_dropdown.py script
###############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of combobox_dropdown widget
"""

# imports
import sys
import os

from strongwind import *
from combobox_dropdown import *
from helpers import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the combobox_dropdown sample application
try:
  app = launchComboBox(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
cbddFrame = app.comboBoxDropDownFrame

###############
# Actions check
###############

# check ComboBox's actions
actionsCheck(cbddFrame.combobox, "ComboBox")

# Ensure that the action interface is not implemented for the menu accessible
cbddFrame.assertUnimplementedActionInterface(cbddFrame.menu)

# Ensure that each menu item as the appropriate action(s)
for item in cbddFrame.menu_items:
    actionsCheck(item, "MenuItem")

#######################
# Perform action for all menu items and check the states of each menu item
# after performing an action
#######################
for item in cbddFrame.menu_items:
    procedurelogger.action("Perform click action for %s" % item)
    item.click()
    sleep(config.SHORT_DELAY)
    cbddFrame.assertText(cbddFrame.textbox, item.text)
    cbddFrame.checkAllStates(item)

# click item 1 to set the GUI back to how it was originally
assert cbddFrame.menu_items[1].name == "1"
cbddFrame.menu_items[1].click()

# do press action to show menu item list
cbddFrame.press(cbddFrame.combobox)

#######################
# States check
#######################

# check default states of ComboBox, menu and text
statesCheck(cbddFrame.combobox, "ComboBox")
statesCheck(cbddFrame.menu, "Menu", add_states=["focused", "showing", "visible"])
statesCheck(cbddFrame.textbox, "Text", add_states=["focused", "selectable"])

# check menu_items0,1's default states
statesCheck(cbddFrame.menu_items[0], "MenuItem", invalid_states=["showing"])
statesCheck(cbddFrame.menu_items[1], "MenuItem", \
                                add_states=["focused", "selected"])

###############################################################
# Text and EditableText test
# Key navigate and doing Action to change Label and States test
###############################################################

# check menu_items's text is implemented
cbddFrame.assertAllItemTexts()

# keyCombo down to select menu_items2 to raise focused and selected 
# and change label, menu_items1 turn to default states
cbddFrame.keyCombo("Down", grabFocus = False)
sleep(config.SHORT_DELAY)
cbddFrame.assertLabel("You select 2")
statesCheck(cbddFrame.menu_items[2], "MenuItem", \
                                add_states=["focused", "selected"])
statesCheck(cbddFrame.menu_items[1], "MenuItem")

# keyCombo down to select menu_items3 to raise focused and selected 
# and change label, menu_items2 turn to default states
cbddFrame.keyCombo("Down", grabFocus = False)
sleep(config.SHORT_DELAY)
cbddFrame.assertLabel("You select 3")
statesCheck(cbddFrame.menu_items[3], "MenuItem", \
                                add_states=["focused", "selected"])
statesCheck(cbddFrame.menu_items[2], "MenuItem")

# keyCombo up to select menu_items2 again to raise focused and selected 
# and change label, menu_items3 turn to default states
cbddFrame.keyCombo("Up", grabFocus = False)
sleep(config.SHORT_DELAY)
cbddFrame.assertLabel("You select 2")
statesCheck(cbddFrame.menu_items[2], "MenuItem", \
                                add_states=["focused", "selected"])
statesCheck(cbddFrame.menu_items[3], "MenuItem")

# click menu_items0 to update text value and label, raise selected and focused
cbddFrame.click(cbddFrame.menu_items[0])
sleep(config.SHORT_DELAY)
cbddFrame.assertLabel("You select 0")
cbddFrame.assertText(cbddFrame.textbox, "0")
statesCheck(cbddFrame.menu_items[0], "MenuItem", add_states=["focused","selected"])

# click menu_items9 to update text value and label and raise selected and focused
# menu_items0 get rid of selected, showing states
cbddFrame.click(cbddFrame.menu_items[9])
sleep(config.SHORT_DELAY)
cbddFrame.assertLabel("You select 9")
cbddFrame.assertText(cbddFrame.textbox, "9")
statesCheck(cbddFrame.menu_items[9], "MenuItem", add_states=["focused", "selected"])
statesCheck(cbddFrame.menu_items[0], "MenuItem", invalid_states=["showing"])

# close menu, then type '6' into text box to change text and label
cbddFrame.typeMenuItemTest("6")

# colse menu, then test editable Text by insert '8' 
##BUG491409: insert menu_items's name doesn't rise focused and selected
#cbddFrame.insertMenuItemTest("8")

##############################
# Selection test
##############################

# check combo box selection is implemented
# set index 0 to select MenuItem 0
#cbddFrame.assertSelectChild(cbddFrame.combobox, 0)
#sleep(config.SHORT_DELAY)
##BUG488474, assertSelectChild called the selection interface's selectChild
#method, which is not working.
#cbddFrame.assertText(cbddFrame.textbox, "0")
##doesn't rise 'selected' state for Menu and Text due to BUG456341
#statesCheck(cbddFrame.menu, "Menu", add_states=["focused", "selected"])

# set index 1 to select MenuItem 1
#cbddFrame.assertSelectChild(cbddFrame.combobox, 1)
#sleep(config.SHORT_DELAY)
#cbddFrame.assertText(cbddFrame.textbox, 1)
#statesCheck(cbddFrame.menu, "Menu", add_states=["focused"])
#statesCheck(cbddFrame.textbox, "Text", add_states=["focused", "selected"])

# set index 3 to select MenuItem 3
#cbddFrame.assertSelectChild(cbddFrame.combobox, 3)
#sleep(config.SHORT_DELAY)
#cbddFrame.assertText(cbddFrame.textbox, "3")

# check menu selection is implemented
# select item3 to rise focused and selected states
cbddFrame.assertSelectChild(cbddFrame.menu, 3)
sleep(config.SHORT_DELAY)
cbddFrame.assertText(cbddFrame.textbox, "3")
statesCheck(cbddFrame.menu_items[3], "MenuItem", add_states=["focused", "selected"])

# select item5 to rise focused and selected states, 
# item3 get rid of focused and selected states
cbddFrame.assertSelectChild(cbddFrame.menu, 5)
sleep(config.SHORT_DELAY)
cbddFrame.assertText(cbddFrame.textbox, "5")
statesCheck(cbddFrame.menu_items[5], "MenuItem", add_states=["focused", "selected"])
# BUG493308 - MaxDropDownItems property of a DropDown is ignored.  The sample
# should only be showing 3 menu items at a time, if this was the case, this
# menu item below would definitely not be showing.  Currently, it shows on some
# machines and not on others depending on font sizes / themes).
#statesCheck(cbddFrame.menu_items[3], "MenuItem")

##############################################
# test AtkStreamableContent for text
##############################################

cbddFrame.assertContent(cbddFrame.textbox)

# close application frame window
cbddFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
