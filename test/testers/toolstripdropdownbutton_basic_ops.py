#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        12/09/2008
# Description: Test accessibility of toolstripdropdownbutton widget 
#              Use the toolstripdropdownbuttonframe.py wrapper script
#              Test the samples/toolstripdropdownbutton.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of toolstripdropdownbutton widget
"""

# imports
import sys
import os

from strongwind import *
from toolstripdropdownbutton import *
from helpers import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the toolstripdropdownbutton sample application
try:
  app = launchToolStripDropDownButton(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
tsddbFrame = app.toolStripDropDownButtonFrame

#check menu's action
actionsCheck(tsddbFrame.menu1, "Menu")
actionsCheck(tsddbFrame.menu2, "Menu")

#check states list for menu without click
statesCheck(tsddbFrame.menu1, "Menu")
statesCheck(tsddbFrame.menu2, "Menu")
#move mouse to menu1 doesn't change states
tsddbFrame.menu1.mouseMove()
sleep(config.SHORT_DELAY)
statesCheck(tsddbFrame.menu1, "Menu")
statesCheck(tsddbFrame.menu2, "Menu")
#move mouse to menu2 doesn't change states
tsddbFrame.menu2.mouseMove()
sleep(config.SHORT_DELAY)
statesCheck(tsddbFrame.menu2, "Menu")
statesCheck(tsddbFrame.menu1, "Menu")

#click menu1 to rise selected
tsddbFrame.click(tsddbFrame.menu1)
sleep(config.SHORT_DELAY)
statesCheck(tsddbFrame.menu1, "Menu", add_states=["selected"])
statesCheck(tsddbFrame.menu2, "Menu")
#click menu1 again to get rid of selected
tsddbFrame.click(tsddbFrame.menu1)
sleep(config.SHORT_DELAY)
statesCheck(tsddbFrame.menu1, "Menu")
statesCheck(tsddbFrame.menu2, "Menu")

#click menu2 may move selection to menu2
tsddbFrame.click(tsddbFrame.menu2)
sleep(config.SHORT_DELAY)
statesCheck(tsddbFrame.menu2, "Menu", add_states=["selected"])
statesCheck(tsddbFrame.menu1, "Menu")
#click menu2 again to get rid of selected
tsddbFrame.click(tsddbFrame.menu2)
sleep(config.SHORT_DELAY)
statesCheck(tsddbFrame.menu2, "Menu")
statesCheck(tsddbFrame.menu1, "Menu")

#check states list for menuitems
tsddbFrame.click(tsddbFrame.menu1)
sleep(config.SHORT_DELAY)
statesCheck(tsddbFrame.red, "MenuItem")
#check menuitem's actions
actionsCheck(tsddbFrame.red, "MenuItem")
actionsCheck(tsddbFrame.blue, "MenuItem")
actionsCheck(tsddbFrame.green, "MenuItem")
#move mouse to blue item may rise focused and selected
tsddbFrame.blue.mouseMove()
sleep(config.SHORT_DELAY)
statesCheck(tsddbFrame.blue, "MenuItem", add_states=["focused","selected"])
#press "down" would move focus and selection to green item
tsddbFrame.keyCombo("Down", grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(tsddbFrame.green, "MenuItem", add_states=["focused", "selected"])

#click menuitems to select color, label would shows you slected which color
tsddbFrame.click(tsddbFrame.red)
sleep(config.SHORT_DELAY)
tsddbFrame.assertLabel("You selected Red")
statesCheck(tsddbFrame.red, "MenuItem", add_states=["selected"])

tsddbFrame.click(tsddbFrame.blue)
sleep(config.SHORT_DELAY)
tsddbFrame.assertLabel("You selected Blue")
statesCheck(tsddbFrame.blue, "MenuItem", add_states=["selected"])
statesCheck(tsddbFrame.red, "MenuItem")

tsddbFrame.click(tsddbFrame.green)
sleep(config.SHORT_DELAY)
tsddbFrame.assertLabel("You selected Green")
statesCheck(tsddbFrame.green, "MenuItem", add_states=["selected"])
statesCheck(tsddbFrame.red, "MenuItem")
statesCheck(tsddbFrame.blue, "MenuItem")

#test Selection implementation for Menu
#select menuitem red to rise selected and focused
tsddbFrame.assertSelectionChild(tsddbFrame.menu1, 0)
sleep(config.SHORT_DELAY)
statesCheck(tsddbFrame.red, "MenuItem", add_states=["focused", "selected"])
statesCheck(tsddbFrame.blue, "MenuItem")
#select menuitem green to rise selected and focused
tsddbFrame.assertSelectionChild(tsddbFrame.menu1, 2)
sleep(config.SHORT_DELAY)
statesCheck(tsddbFrame.green, "MenuItem", add_states=["focused", "selected"])
statesCheck(tsddbFrame.red, "MenuItem")
#clear selection, all menuitem without focused and selected
tsddbFrame.assertClearSelection(tsddbFrame.menu1)
sleep(config.SHORT_DELAY)
statesCheck(tsddbFrame.red, "MenuItem")
statesCheck(tsddbFrame.blue, "MenuItem")
statesCheck(tsddbFrame.green, "MenuItem")

#test Text implementation for Menu and MenuItems
tsddbFrame.assertText(tsddbFrame.menu1, "ToolStripDropDownButton1")
tsddbFrame.assertText(tsddbFrame.menu2, "ToolStripDropDownButton2")
tsddbFrame.assertText(tsddbFrame.red, "Red")
tsddbFrame.assertText(tsddbFrame.blue, "Blue")
tsddbFrame.assertText(tsddbFrame.green, "Green")

print "INFO:  Log written to: %s" % config.OUTPUT_DIR

#close application frame window
tsddbFrame.quit()
