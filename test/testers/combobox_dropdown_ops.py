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
statesCheck(cbddFrame.menuitem[1], "MenuItem", \
                                add_states=["focused", "selected"])

#check menuitem's text is implemented
cbddFrame.assertItemText()

#test press action than using mouse click menuitem to change label's text
cbddFrame.press(cbddFrame.combobox)
sleep(config.SHORT_DELAY)
cbddFrame.keyCombo("Down", grabFocus = False)
sleep(config.SHORT_DELAY)
cbddFrame.assertLabel('2')

cbddFrame.press(cbddFrame.combobox)
sleep(config.SHORT_DELAY)
cbddFrame.keyCombo("Down", grabFocus = False)
sleep(config.SHORT_DELAY)
cbddFrame.assertLabel('3')

#do click action to select menuitem0 but not change the states(the same as Gtk),
#also update text value
cbddFrame.click(cbddFrame.menuitem[0])
sleep(config.SHORT_DELAY)
cbddFrame.assertText(cbddFrame.textbox, 0)

statesCheck(cbddFrame.menuitem[0], "MenuItem")
#if you press menu to show the menu list, menuitem0 would rise "focused" 
#and "selected"(the same as Gtk)
cbddFrame.combobox.mouseClick()
statesCheck(cbddFrame.menuitem[0], "MenuItem", add_states=["focused", "selected"])

#do click action to select menuitem also may change label's text
cbddFrame.click(cbddFrame.menuitem[2])
sleep(config.SHORT_DELAY)
cbddFrame.assertLabel('2')

#check selection is implementation
#select item2 to rise focused and selected states
cbddFrame.assertSelectionChild(cbddFrame.menu, 3)
sleep(config.SHORT_DELAY)
statesCheck(cbddFrame.menuitem[3], "MenuItem", add_states=["focused", "selected"])
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

#inter '6' to text box to check the text value
cbddFrame.inputText(6)
sleep(config.SHORT_DELAY)
#label's text is changed
cbddFrame.assertLabel('6')
#the text of textbox is changed to 6
cbddFrame.assertText(cbddFrame.textbox, 6)
# menuitem[6] would be selected and focused
statesCheck(cbddFrame.menuitem[6], "MenuItem", add_states=["focused", "selected"])

#test editable Text by enter text value '8' 
cbddFrame.enterTextValue(8)
sleep(config.SHORT_DELAY)
#label's text is changed
cbddFrame.assertLabel('8')
#the text of textbox is changed to 8
cbddFrame.assertText(cbddFrame.textbox, 8)
#without change the states
statesCheck(cbddFrame.menuitem[8], "MenuItem")

#close application frame window
cbddFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
