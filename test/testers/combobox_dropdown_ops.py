#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        11/14/2008
# Description: Test accessibility of combobox_dropdown widget 
#              Use the comboboxdropdownframe.py wrapper script
#              Test the samples/combobox_dropdown.py script
##############################################################################

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

#check ComboBox item's actions list
actionsCheck(cbddFrame.menuitem[0], "MenuItem")

#check ComboBox's states list
statesCheck(cbddFrame.combobox, "ComboBox", add_states=["focused"])
statesCheck(cbddFrame.menu, "Menu")

#check menuitem0,1's default states
statesCheck(cbddFrame.menuitem[0], "MenuItem")
statesCheck(cbddFrame.menuitem[1], "MenuItem")

#check menuitem's text is implemented
cbddFrame.assertItemText()

#test press action than using mouse click menuitem to change label's text
cbddFrame.press(cbddFrame.combobox)
cbddFrame.menuitem[1].mouseClick()
sleep(config.SHORT_DELAY)
cbddFrame.assertLabel('1')

cbddFrame.press(cbddFrame.combobox)
cbddFrame.menuitem[9].mouseClick()
sleep(config.SHORT_DELAY)
cbddFrame.assertLabel('9')

#do click action to select menuitem0 but not change the states(the same as Gtk),
#also update text value
cbddFrame.click(cbddFrame.menuitem[0])
sleep(config.SHORT_DELAY)
cbddFrame.assertText(cbddFrame.textbox, 0)

statesCheck(cbddFrame.menuitem[0], "MenuItem")

#check selection is implementation
#select item2 to rise focused and selected states
cbddFrame.assertSelectionChild(cbddFrame.menu, 2)
sleep(config.SHORT_DELAY)
statesCheck(cbddFrame.menuitem[2], "MenuItem", add_states=["focused", "selected"])
#select item5 to rise focused and selected states
cbddFrame.assertSelectionChild(cbddFrame.menu, 5)
sleep(config.SHORT_DELAY)
statesCheck(cbddFrame.menuitem[5], "MenuItem", add_states=["focused", "selected"])
#item2 get rid of focused and selected states
statesCheck(cbddFrame.menuitem[2], "MenuItem")

#clear selection
cbddFrame.assertClearSelection(cbddFrame.ComboBox)
sleep(config.SHORT_DELAY)
statesCheck(cbddFrame.menuitem[5], "MenuItem")

#inter '6' to text box to check the text value, menuitem[6] would be selected
cbddFrame.inputText(6)

statesCheck(cbddFrame.menuitem[6], "MenuItem", add_states=["focused", "selected"])

#test editable Text by enter text value '8' without change the states
cbddFrame.enterTextValue(8)

statesCheck(cbddFrame.menuitem[8], "MenuItem")

#close application frame window
cbddFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
