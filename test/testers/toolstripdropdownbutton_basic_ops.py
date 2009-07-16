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

# check menu's action
actionsCheck(tsddbFrame.menu1, "Menu")
actionsCheck(tsddbFrame.menu2, "Menu")

# check states list for menu without click
# we are sure that menu should have "focusable" states according to bug502972
statesCheck(tsddbFrame.menu1, "Menu", add_states=["focusable"])
statesCheck(tsddbFrame.menu2, "Menu", add_states=["focusable"])
# move mouse to menu1 to rise focused and selected
panels = tsddbFrame.toolbar.findAllPanels(None)
panels[0].mouseMove()
sleep(config.SHORT_DELAY)
statesCheck(tsddbFrame.menu1, "Menu", add_states=["focusable", "focused", "selected"])
statesCheck(tsddbFrame.menu2, "Menu", add_states=["focusable"])
# move mouse to menu2 to rise focused and selected
panels[1].mouseMove()
sleep(config.SHORT_DELAY)
statesCheck(tsddbFrame.menu2, "Menu", add_states=["focusable", "focused", "selected"])
statesCheck(tsddbFrame.menu1, "Menu", add_states=["focusable"])

# click menu1 to rise selected
tsddbFrame.menu1.click(log=True)
sleep(config.SHORT_DELAY)
statesCheck(tsddbFrame.menu1, "Menu", add_states=["focusable", "focused", "selected"])
statesCheck(tsddbFrame.menu2, "Menu", add_states=["focusable"])

# click menu1 again
tsddbFrame.menu1.click(log=True)
sleep(config.SHORT_DELAY)
statesCheck(tsddbFrame.menu1, "Menu", add_states=["focusable", "focused", "selected"])
statesCheck(tsddbFrame.menu2, "Menu", add_states=["focusable"])

# click menu2 to move selection to menu2
tsddbFrame.menu2.click(log=True)
sleep(config.SHORT_DELAY)
statesCheck(tsddbFrame.menu2, "Menu", add_states=["focusable", "focused", "selected"])
statesCheck(tsddbFrame.menu1, "Menu", add_states=["focusable"])
# click menu2 again
tsddbFrame.menu2.click(log=True)
sleep(config.SHORT_DELAY)
statesCheck(tsddbFrame.menu2, "Menu", add_states=["focusable", "focused", "selected"])
statesCheck(tsddbFrame.menu1, "Menu", add_states=["focusable"])

##############menu items test##################
# check states list for menuitems
# BUG486335: extraneous showing state
'''
statesCheck(tsddbFrame.red, "MenuItem", invalid_states=["showing"])
statesCheck(tsddbFrame.blue, "MenuItem", invalid_states=["showing"])
statesCheck(tsddbFrame.green, "MenuItem", invalid_states=["showing"])
statesCheck(tsddbFrame.item1, "MenuItem", invalid_states=["showing"])
statesCheck(tsddbFrame.item2, "MenuItem", invalid_states=["showing"])
statesCheck(tsddbFrame.item3, "MenuItem", invalid_states=["showing"])
'''

tsddbFrame.menu1.click(log=True)
sleep(config.SHORT_DELAY)
# check menuitem's states
statesCheck(tsddbFrame.red, "MenuItem")
statesCheck(tsddbFrame.blue, "MenuItem")
statesCheck(tsddbFrame.green, "MenuItem")

# check menuitem's actions
actionsCheck(tsddbFrame.red, "MenuItem")
actionsCheck(tsddbFrame.blue, "MenuItem")
actionsCheck(tsddbFrame.green, "MenuItem")
actionsCheck(tsddbFrame.item1, "MenuItem")
actionsCheck(tsddbFrame.item2, "MenuItem")
actionsCheck(tsddbFrame.item3, "MenuItem")

# move mouse cursor to blue item may rise focused and selected
tsddbFrame.blue.mouseMove()
sleep(config.SHORT_DELAY)
statesCheck(tsddbFrame.blue, "MenuItem", add_states=["focused","selected"])
# press "down" moving focus and selection to green item
tsddbFrame.keyCombo("Down", grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(tsddbFrame.green, "MenuItem", add_states=["focused", "selected"])

# click menuitems to select color, label would shows you slected which color
# BUG522570: after doing click action menu_item shouldn't is showing selected
tsddbFrame.red.click(log=True)
sleep(config.SHORT_DELAY)
tsddbFrame.assertText(tsddbFrame.label, "You selected Red")
# BUG508984: menuitems still remain focused and selected states when clicked
# statesCheck(tsddbFrame.red, "MenuItem")

tsddbFrame.blue.click(log=True)
sleep(config.SHORT_DELAY)
tsddbFrame.assertText(tsddbFrame.label, "You selected Blue")
#statesCheck(tsddbFrame.blue, "MenuItem")
statesCheck(tsddbFrame.red, "MenuItem")

tsddbFrame.green.click(log=True)
sleep(config.SHORT_DELAY)
tsddbFrame.assertText(tsddbFrame.label, "You selected Green")
#statesCheck(tsddbFrame.green, "MenuItem")
statesCheck(tsddbFrame.red, "MenuItem")
statesCheck(tsddbFrame.blue, "MenuItem")

# test Selection implementation for Menu
# select menuitem red to rise selected
tsddbFrame.menu1.selectChild(0)
sleep(config.SHORT_DELAY)
# Gtk adds both focused and selected states when using the selectChild method
statesCheck(tsddbFrame.red, "MenuItem", add_states=["focused", "selected"])
statesCheck(tsddbFrame.blue, "MenuItem")
# select menuitem green to rise selected
tsddbFrame.menu1.selectChild(2)
sleep(config.SHORT_DELAY)
statesCheck(tsddbFrame.green, "MenuItem", add_states=["focused", "selected"])
statesCheck(tsddbFrame.red, "MenuItem")

# test Text implementation for Menu and MenuItems
tsddbFrame.assertText(tsddbFrame.menu1, "ToolStripDropDownButton1")
tsddbFrame.assertText(tsddbFrame.menu2, "ToolStripDropDownButton2")
tsddbFrame.assertText(tsddbFrame.red, "Red")
tsddbFrame.assertText(tsddbFrame.blue, "Blue")
tsddbFrame.assertText(tsddbFrame.green, "Green")

print "INFO:  Log written to: %s" % config.OUTPUT_DIR

# close application frame window
tsddbFrame.quit()
