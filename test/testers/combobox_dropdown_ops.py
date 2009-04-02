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

# do press action to show menu item list
cbddFrame.press(cbddFrame.combobox)

# Menu without action, MenuItems have click action
cbddFrame.menuAction(cbddFrame.menu)
actionsCheck(cbddFrame.menuitem[0], "MenuItem")

#######################
# States check
#######################

# check default states of ComboBox, menu and text
statesCheck(cbddFrame.combobox, "ComboBox")
statesCheck(cbddFrame.menu, "Menu", add_states=["focused", "showing", "visible"])
statesCheck(cbddFrame.textbox, "Text", add_states=["focused", "selectable"])

# check menuitem0,1's default states
statesCheck(cbddFrame.menuitem[0], "MenuItem", invalid_states=["showing"])
statesCheck(cbddFrame.menuitem[1], "MenuItem", \
                                add_states=["focused", "selected"])

###############################################################
# Text and EditableText test
# Key navigate and doing Action to change Label and States test
###############################################################

# check menuitem's text is implemented
cbddFrame.assertItemText()

# keyCombo down to select menuitem2 to raise focused and selected 
# and change label, menuitem1 turn to default states
cbddFrame.keyCombo("Down", grabFocus = False)
sleep(config.SHORT_DELAY)
cbddFrame.assertLabel("You select 2")
statesCheck(cbddFrame.menuitem[2], "MenuItem", \
                                add_states=["focused", "selected"])
statesCheck(cbddFrame.menuitem[1], "MenuItem")

# keyCombo down to select menuitem3 to raise focused and selected 
# and change label, menuitem2 turn to default states
cbddFrame.keyCombo("Down", grabFocus = False)
sleep(config.SHORT_DELAY)
cbddFrame.assertLabel("You select 3")
statesCheck(cbddFrame.menuitem[3], "MenuItem", \
                                add_states=["focused", "selected"])
statesCheck(cbddFrame.menuitem[2], "MenuItem")

# keyCombo up to select menuitem2 again to raise focused and selected 
# and change label, menuitem3 turn to default states
cbddFrame.keyCombo("Up", grabFocus = False)
sleep(config.SHORT_DELAY)
cbddFrame.assertLabel("You select 2")
statesCheck(cbddFrame.menuitem[2], "MenuItem", \
                                add_states=["focused", "selected"])
statesCheck(cbddFrame.menuitem[3], "MenuItem")

# click menuitem0 to update text value and label, raise selected and focused
cbddFrame.click(cbddFrame.menuitem[0])
sleep(config.SHORT_DELAY)
cbddFrame.assertLabel("You select 0")
cbddFrame.assertTextChanged(cbddFrame.textbox, "0")
statesCheck(cbddFrame.menuitem[0], "MenuItem", add_states=["focused","selected"])

# click menuitem9 to update text value and label and raise selected and focused
# menuitem0 get rid of selected, showing states
cbddFrame.click(cbddFrame.menuitem[9])
sleep(config.SHORT_DELAY)
cbddFrame.assertLabel("You select 9")
cbddFrame.assertTextChanged(cbddFrame.textbox, "9")
statesCheck(cbddFrame.menuitem[9], "MenuItem", add_states=["focused", "selected"])
statesCheck(cbddFrame.menuitem[0], "MenuItem", invalid_states=["showing"])

# close menu, then type '6' into text box to change text and label
cbddFrame.typeMenuItem("6")

# colse menu, then test editable Text by insert '8' 
##BUG491409: insert menuitem's name doesn't rise focused and selected
#cbddFrame.insertMenuItem("8")

##############################
# Selection test
##############################

# check combo box selection is implemented
# set index 0 to select MenuItem 0
#cbddFrame.assertSelectionChild(cbddFrame.combobox, 0)
#sleep(config.SHORT_DELAY)
##BUG488474, assertSelectionChild called the selection interface's selectChild
#method, which is not working.
#cbddFrame.assertTextChanged(cbddFrame.textbox, "0")
##doesn't rise 'selected' state for Menu and Text due to BUG456341
#statesCheck(cbddFrame.menu, "Menu", add_states=["focused", "selected"])

# set index 1 to select MenuItem 1
#cbddFrame.assertSelectionChild(cbddFrame.combobox, 1)
#sleep(config.SHORT_DELAY)
#cbddFrame.assertTextChanged(cbddFrame.textbox, 1)
#statesCheck(cbddFrame.menu, "Menu", add_states=["focused"])
#statesCheck(cbddFrame.textbox, "Text", add_states=["focused", "selected"])

# set index 3 to select MenuItem 3
#cbddFrame.assertSelectionChild(cbddFrame.combobox, 3)
#sleep(config.SHORT_DELAY)
#cbddFrame.assertTextChanged(cbddFrame.textbox, "3")

# check menu selection is implemented
# select item3 to rise focused and selected states
cbddFrame.assertSelectionChild(cbddFrame.menu, 3)
sleep(config.SHORT_DELAY)
cbddFrame.assertTextChanged(cbddFrame.textbox, "3")
statesCheck(cbddFrame.menuitem[3], "MenuItem", add_states=["focused", "selected"])

# select item5 to rise focused and selected states, 
# item3 get rid of focused and selected states
cbddFrame.assertSelectionChild(cbddFrame.menu, 5)
sleep(config.SHORT_DELAY)
cbddFrame.assertTextChanged(cbddFrame.textbox, "5")
statesCheck(cbddFrame.menuitem[5], "MenuItem", add_states=["focused", "selected"])
statesCheck(cbddFrame.menuitem[3], "MenuItem", invalid_states=["showing"])

# clearSelection may not get rid of selected, see bug468456
cbddFrame.assertClearSelection(cbddFrame.menu)
sleep(config.SHORT_DELAY)
statesCheck(cbddFrame.menuitem[5], "MenuItem", add_states=["focused", "selected"])

##############################################
# test AtkStreamableContent for text
##############################################

cbddFrame.assertContent(cbddFrame.textbox)

# close application frame window
cbddFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
