#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        07/24/2008
# Description: Test accessibility of checkbox widget 
#              Use the checkboxframe.py wrapper script
#              Test the samples/winforms/checkbox_radiobutton.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of checkbox widget
"""

# imports
import sys
import os

from strongwind import *
from checkbox import *
from helpers import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the treeview sample application
try:
  app = launchCheckBox(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
cbFrame = app.checkBoxFrame

#check CheckBox's actions list
actionsCheck(cbFrame.check1, "CheckBox")
actionsCheck(cbFrame.check2, "CheckBox")
actionsCheck(cbFrame.check3, "CheckBox")
actionsCheck(cbFrame.check4, "CheckBox")
actionsCheck(cbFrame.check5, "CheckBox")
actionsCheck(cbFrame.check6, "CheckBox")

#check1 have "focused" state
statesCheck(cbFrame.check1, "CheckBox", add_states=["focused"])

# Press 'Right' key until we get back to check1, and check that each
# check button gets the focused state except check4, which should be
# skipped
cbFrame.keyCombo("Tab", grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(cbFrame.check2, "CheckBox", add_states=["focused"])

cbFrame.keyCombo("Right", grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(cbFrame.check3, "CheckBox", add_states=["focused","checked"])

cbFrame.keyCombo("Right", grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(cbFrame.check5, "CheckBox", add_states=["focused"])

# check and uncheck with space bar
cbFrame.keyCombo("space", grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(cbFrame.check5, "CheckBox", add_states=["focused","checked"])

cbFrame.keyCombo("space", grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(cbFrame.check5, "CheckBox", add_states=["focused"])

#check2 have the default states
statesCheck(cbFrame.check2, "CheckBox")

#check3 have "checked" state
statesCheck(cbFrame.check3, "CheckBox", add_states=["checked"])

#check4 have 2 invalid states
statesCheck(cbFrame.check4, "CheckBox",
                    invalid_states=["sensitive", "enabled","focusable"])

#click check2 would rise 'checked' states but not 'focused'
cbFrame.click(cbFrame.check2)
sleep(config.SHORT_DELAY)
statesCheck(cbFrame.check2, "CheckBox",
                    add_states=["checked"])

# Make sure that check2 and check3 are multi checked
statesCheck(cbFrame.check3, "CheckBox",
                    add_states=["checked"])

# Make sure check5 still has focus
statesCheck(cbFrame.check5, "CheckBox", add_states=["focused"])

#click check3 would get rid of 'checked' states
cbFrame.click(cbFrame.check3)
sleep(config.SHORT_DELAY)
statesCheck(cbFrame.check3, "CheckBox")

#check and uncheck the check5 in succession
cbFrame.click(cbFrame.check5)
sleep(config.SHORT_DELAY)
statesCheck(cbFrame.check5, "CheckBox",
                    add_states=["focused", "checked"])
#uncheck check5 but still focus
cbFrame.click(cbFrame.check5)
sleep(config.SHORT_DELAY)
statesCheck(cbFrame.check5, "CheckBox",
                    add_states=["focused"])

#focus and check check3 by mouseClick would rise 'focused' state for check3
cbFrame.check3.mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(cbFrame.check3, "CheckBox",
                    add_states=["focused", "checked"])

#make sure check5 is no longer focused
statesCheck(cbFrame.check5, "CheckBox")

#click check4 doesn't update the states
cbFrame.check4.mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(cbFrame.check4, "CheckBox",
                    invalid_states=["sensitive", "enabled","focusable"])

#checkbox with image and background should have correct states and can be 
#and focused
cbFrame.check5.mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(cbFrame.check5, "CheckBox", add_states=["focused", "checked"])

cbFrame.check6.mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(cbFrame.check6, "CheckBox", add_states=["focused", "checked"])

#implement checkbox's image, default checkbox have image size(-1*-1)
cbFrame.assertImageSize(cbFrame.check1)
cbFrame.assertImageSize(cbFrame.check2)
cbFrame.assertImageSize(cbFrame.check3)
cbFrame.assertImageSize(cbFrame.check4)
#checkbox wich image property have image size(60*38)
cbFrame.assertImageSize(cbFrame.check5, 60, 38)
#checkbox with background property have image size(-1*-1)
cbFrame.assertImageSize(cbFrame.check6)

#test click event to update label's text
#mouse click checkbox in application directly
cbFrame.check1.mouseClick()
sleep(config.SHORT_DELAY)
cbFrame.assertLabel(cbFrame.check1, "checked")

cbFrame.check1.mouseClick()
sleep(config.SHORT_DELAY)
cbFrame.assertLabel(cbFrame.check1, "unchecked")

#do click action py pyatspi
cbFrame.click(cbFrame.check1)
sleep(config.SHORT_DELAY)
cbFrame.assertLabel(cbFrame.check1, "checked")

cbFrame.click(cbFrame.check1)
sleep(config.SHORT_DELAY)
cbFrame.assertLabel(cbFrame.check1, "unchecked")

print "INFO:  Log written to: %s" % config.OUTPUT_DIR

#close application frame window
cbFrame.quit()
