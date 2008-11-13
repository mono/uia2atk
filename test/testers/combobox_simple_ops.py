#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        11/10/2008
# Description: Test accessibility of combobox_simple widget 
#              Use the comboboxsimpleframe.py wrapper script
#              Test the samples/combobox_simple.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of combobox_simple widget
"""

# imports
import sys
import os

from strongwind import *
from combobox_simple import *
from helpers import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the combobox_simple sample application
try:
  app = launchComboBox(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
cbFrame = app.comboBoxFrame

#check ComboBox's actions
actionsCheck(cbFrame.combobox, "ComboBox")

#check ComboBox item's actions list
actionsCheck(cbFrame.menuitem[0], "MenuItem")

#check ComboBox's states list
statesCheck(cbFrame.combobox, "ComboBox", add_states=["focused"])
statesCheck(cbFrame.menu, "Menu")

#check menuitem0,1's default states
statesCheck(cbFrame.menuitem[0], "MenuItem")
statesCheck(cbFrame.menuitem[1], "MenuItem")

#check menuitem's text implemented
cbFrame.assertItemText()

#test press action than using mouse click menuitem to change label's text
cbFrame.press(cbFrame.combobox)
cbFrame.menuitem[1].mouseClick()
sleep(config.SHORT_DELAY)
cbFrame.assertLabel('1')

cbFrame.press(cbFrame.combobox)
cbFrame.menuitem[9].mouseClick()
sleep(config.SHORT_DELAY)
cbFrame.assertLabel('9')

#do click action to select menuitem0 but not change the states(the same as Gtk),
#also update text value
cbFrame.click(cbFrame.menuitem[0])
sleep(config.SHORT_DELAY)
cbFrame.assertText(cbFrame.textbox, 0)

statesCheck(cbFrame.menuitem[0], "MenuItem")

#check list selection implementation
#select item2 to rise focused and selected states
cbFrame.assertSelectionChild(cbFrame.menu, 2)
sleep(config.SHORT_DELAY)
statesCheck(cbFrame.menuitem[2], "MenuItem", add_states=["focused", "selected"])
#select item5 to rise focused and selected states
cbFrame.assertSelectionChild(cbFrame.menu, 5)
sleep(config.SHORT_DELAY)
statesCheck(cbFrame.menuitem[5], "MenuItem", add_states=["focused", "selected"])
#item2 get rid of focused and selected states
statesCheck(cbFrame.menuitem[2], "MenuItem")

#clear selection
cbFrame.assertClearSelection(cbFrame.ComboBox)
sleep(config.SHORT_DELAY)
statesCheck(cbFrame.menuitem[5], "MenuItem")

#inter '6' to text box to check the text value, menuitem[6] would be selected
cbFrame.inputText(6)

statesCheck(cbFrame.menuitem[6], "MenuItem", add_states=["focused", "selected"])

#test editable Text by enter text value '8' without change the states
cbFrame.enterTextValue(8)

statesCheck(cbFrame.menuitem[8], "MenuItem")

#close application frame window
cbFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
