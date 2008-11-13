#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        11/10/2008
# Description: Test accessibility of combobox_dropdownlist widget 
#              Use the comboboxdropdownlistframe.py wrapper script
#              Test the samples/combobox_dropdownlist.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of combobox_dropdownlist widget
"""

# imports
import sys
import os

from strongwind import *
from combobox_dropdownlist import *
from helpers import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the combobox_dropdownlist sample application
try:
  app = launchComboBox(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
cbddlFrame = app.comboBoxDropDownListFrame

#check ComboBox's action 
actionsCheck(cbddlFrame.combobox, "ComboBox")

#check ComboBox item's actions list
actionsCheck(cbddlFrame.menuitem[0], "MenuItem")

#check ComboBox's states list
statesCheck(cbddlFrame.combobox, "ComboBox", add_states=["focused"])
statesCheck(cbddlFrame.menu, "Menu")

#check menuitem0,1's default states
statesCheck(cbddlFrame.menuitem[0], "MenuItem")
statesCheck(cbddlFrame.menuitem[1], "MenuItem")

#check menuitem's text implemented
cbddlFrame.assertItemText()

#mouse click menuitem to change label's text
cbddlFrame.press(cbddlFrame.combobox)
cbddlFrame.menuitem[1].mouseClick()
sleep(config.SHORT_DELAY)
cbddlFrame.assertLabel('1')

cbddlFrame.press(cbddlFrame.combobox)
cbddlFrame.menuitem[9].mouseClick()
sleep(config.SHORT_DELAY)
cbddlFrame.assertLabel('9')

#do click action to select menuitem0 but not change the states(the same as Gtk),
#also update text value
cbddlFrame.click(cbddlFrame.menuitem[0])
sleep(config.SHORT_DELAY)
cbddlFrame.assertText(cbddlFrame.textbox, 0)

statesCheck(cbddlFrame.menuitem[0], "MenuItem")

#check list selection implementation
#select item2 to rise focused and selected states
cbddlFrame.assertSelectionChild(cbddlFrame.menu, 2)
sleep(config.SHORT_DELAY)
statesCheck(cbddlFrame.menuitem[2], "MenuItem", add_states=["focused", "selected"])
#select item5 to rise focused and selected states
cbddlFrame.assertSelectionChild(cbddlFrame.menu, 5)
sleep(config.SHORT_DELAY)
statesCheck(cbddlFrame.menuitem[5], "MenuItem", add_states=["focused", "selected"])
#item2 get rid of focused and selected states
statesCheck(cbddlFrame.menuitem[2], "MenuItem")

#clear selection
cbddlFrame.assertClearSelection(cbddlFrame.ComboBox)
sleep(config.SHORT_DELAY)
statesCheck(cbddlFrame.menuitem[5], "MenuItem")

#check press action of combobox to rise a window. I am not sure if our SWF combobox
#would be implemented like GTK, if not, I will delete this test
cbddlFrame.press(cbddlFrame.combobox)
sleep(config.SHORT_DELAY)
cbddlFrame.app.findWindow(None)

#close application frame window
cbddlFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
