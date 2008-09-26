#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        07/24/2008
# Description: Test accessibility of checkbox widget 
#              Use the checkboxframe.py wrapper script
#              Test the samples/checkbox_radiobutton.py script
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

#check1 have "focused" state
statesCheck(cbFrame.check1, "CheckBox", 
                    add_states=["focused"])

#check2 have the default states
statesCheck(cbFrame.check2, "CheckBox")

#check3 have "checked" state
statesCheck(cbFrame.check3, "CheckBox", 
                    add_states=["checked"])

#check4 have 2 invalid states
statesCheck(cbFrame.check4, "CheckBox",
                    invalid_states=["sensitive", "enabled"])

#click check2 would rise 'checked' states but not 'focused'
cbFrame.click(cbFrame.check2)
sleep(config.SHORT_DELAY)
statesCheck(cbFrame.check2, "CheckBox",
                    add_states=["checked"])

# Make sure that check2 and check3 are multi checked
statesCheck(cbFrame.check3, "CheckBox",
                    add_states=["checked"])

#click check3 would get rid of 'checked' states
cbFrame.click(cbFrame.check3)
sleep(config.SHORT_DELAY)
statesCheck(cbFrame.check3, "CheckBox")

#check and uncheck the check1 in succession
cbFrame.click(cbFrame.check1)
sleep(config.SHORT_DELAY)
statesCheck(cbFrame.check1, "CheckBox",
                    add_states=["focused", "checked"])
#uncheck check1 but still focus
cbFrame.click(cbFrame.check1)
sleep(config.SHORT_DELAY)
statesCheck(cbFrame.check1, "CheckBox",
                    add_states=["focused"])

#focus and check check3 by mouseClick would rise 'focused' state for check3
cbFrame.check3.mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(cbFrame.check3, "CheckBox",
                    add_states=["focused", "checked"])

#click check4 doesn't update the states
cbFrame.check4.mouseClick()
statesCheck(cbFrame.check4, "CheckBox",
                    invalid_states=["sensitive", "enabled"])


print "INFO:  Log written to: %s" % config.OUTPUT_DIR

#close application frame window
cbFrame.quit()
