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

#check ComboBox's actions
actionsCheck(cbddFrame.combobox, "ComboBox")

#do press action to show menu item list
cbddFrame.press(cbddFrame.combobox)

#Menu without action, MenuItem with click action
cbddFrame.menuAction(cbddFrame.menu)
actionsCheck(cbddFrame.menuitem[0], "MenuItem")

#check default states of ComboBox, menu and text
statesCheck(cbddFrame.combobox, "ComboBox")
statesCheck(cbddFrame.menu, "Menu")
statesCheck(cbddFrame.textbox, "Text", add_states=["focused", "selected"])

#check menuitem0,1's default states
statesCheck(cbddFrame.menuitem[0], "MenuItem")
statesCheck(cbddFrame.menuitem[1], "MenuItem", \
                                add_states=["focused", "selected"])

#check menuitem's text is implemented
cbddFrame.assertItemText()

#keyCombo down to select menuitem2
cbddFrame.keyCombo("Down", grabFocus = False)
sleep(config.SHORT_DELAY)
#change label's text to You select 2
cbddFrame.assertLabel("You select 2")
#menuitem2 up focused and selected states
statesCheck(cbddFrame.menuitem[2], "MenuItem", \
                                add_states=["focused", "selected"])
#menuitem1 get rid of visible states
statesCheck(cbddFrame.menuitem[1], "MenuItem")

#keyCombo down to select menuitem3
cbddFrame.keyCombo("Down", grabFocus = False)
sleep(config.SHORT_DELAY)
#change label's text to You select 3
cbddFrame.assertLabel("You select 3")
#menuitem3 up focused and selected states
statesCheck(cbddFrame.menuitem[3], "MenuItem", \
                                add_states=["focused", "selected"])
#menuitem2 get rid of showing states
statesCheck(cbddFrame.menuitem[2], "MenuItem")

#keyCombo up to select menuitem2
cbddFrame.keyCombo("Up", grabFocus = False)
sleep(config.SHORT_DELAY)
#change label's text to You select 2
cbddFrame.assertLabel("You select 2")
#menuitem2 up focused and selected states
statesCheck(cbddFrame.menuitem[2], "MenuItem", \
                                add_states=["focused", "selected"])
#menuitem3 get rid of focused and selected states
statesCheck(cbddFrame.menuitem[3], "MenuItem")

#do click action to select menuitem0 to update text value and label
cbddFrame.click(cbddFrame.menuitem[0])
sleep(config.SHORT_DELAY)
#change label's text to You select 0
cbddFrame.assertLabel("You select 0")
#change textbox value to 0
cbddFrame.assertText(cbddFrame.textbox, 0)
#menuitem0 up selected state
statesCheck(cbddFrame.menuitem[0], "MenuItem", add_states=["focused","selected"])

#do click action to select menuitem9 to update text value and label
cbddFrame.click(cbddFrame.menuitem[9])
sleep(config.SHORT_DELAY)
#change label's text to You select 9
cbddFrame.assertLabel("You select 9")
#change textbox value to 9
cbddFrame.assertText(cbddFrame.textbox, 9)
#menuitem9 up selected state
statesCheck(cbddFrame.menuitem[9], "MenuItem", add_states=["focused", "selected"])
#menuitem0 get rid of selected, showing states
statesCheck(cbddFrame.menuitem[0], "MenuItem")

#enter value to textbox
#inter '6' to text box to check the text value
cbddFrame.inputText(cbddFrame.textbox, "6")
sleep(config.SHORT_DELAY)
#label's text is changed
cbddFrame.assertLabel("You select 6")
#the text of textbox is changed to 6
cbddFrame.assertText(cbddFrame.textbox, "6")
#menuitem6 would be selected
statesCheck(cbddFrame.menuitem[6], "MenuItem")

#test editable Text by enter text value '8' 
cbddFrame.enterTextValue(cbddFrame.textbox,"8")
sleep(config.SHORT_DELAY)
#label's text is changed
cbddFrame.assertLabel("You select 8")
#the text of textbox is changed to 8
cbddFrame.assertText(cbddFrame.textbox, 8)
#menuitem8 would be selected
statesCheck(cbddFrame.menuitem[8], "MenuItem")

#check combo box selection is implemented
#select menu to rise selected
##selectChild cause crash BUG456319
cbddFrame.assertSelectionChild(cbddFrame.combobox, 0)
sleep(config.SHORT_DELAY)
statesCheck(cbddFrame.menu, "Menu", add_states=["selected"])
statesCheck(cbddFrame.textbox, "Text")
#select text to rise selected
cbddFrame.assertSelectionChild(cbddFrame.combobox, 1)
sleep(config.SHORT_DELAY)
statesCheck(cbddFrame.menu, "Menu")
statesCheck(cbddFrame.textbox, "Text", add_states=["selected"])
#clear selection to get rid of selected
##clearSelection doesn't get rid of selected BUG468456
cbddFrame.assertClearSelection(cbddFrame.combobox)
sleep(config.SHORT_DELAY)
statesCheck(cbddFrame.menu, "Menu")
statesCheck(cbddFrame.textbox, "Text")

#check menu selection is implemented
#select item3 to rise focused and selected states
cbddFrame.assertSelectionChild(cbddFrame.menu, 3)
sleep(config.SHORT_DELAY)
statesCheck(cbddFrame.menuitem[3], "MenuItem", add_states=["focused", "selected"])
#select item5 to rise focused and selected states
cbddFrame.assertSelectionChild(cbddFrame.menu, 5)
sleep(config.SHORT_DELAY)
statesCheck(cbddFrame.menuitem[5], "MenuItem", add_states=["focused", "selected"])
#item3 get rid of focused and selected states
statesCheck(cbddFrame.menuitem[3], "MenuItem")

#clear selection
cbddFrame.assertClearSelection(cbddFrame.menu)
sleep(config.SHORT_DELAY)
statesCheck(cbddFrame.menuitem[5], "MenuItem")

#close application frame window
cbddFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
