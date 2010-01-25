#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        09/09/2009
# Description: Test accessibility of checkbox widget 
#              Use the checkboxframe.py wrapper script
#              Test the Moonlight CheckBox sample
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of checkbox widget
"""

# imports
from strongwind import *
from checkbox import *
from helpers import *

from sys import argv
from os import path

app_path = None 
try:
    app_path = argv[1]
except IndexError:
    pass  #expected

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

####################
# Check Actions
####################
actionsCheck(cbFrame.check1, "CheckBox")
actionsCheck(cbFrame.check2, "CheckBox")

######################
# Check default States
######################
statesCheck(cbFrame.check1, "CheckBox")
statesCheck(cbFrame.check2, "CheckBox")

###############################
# Do Click action for checkbox1
###############################
# click check1 to raise "checked" state, text1 label is updated to show 
# checked, click action should grab focus to check1 to raise "focused" state
cbFrame.check1.click(log=True)
sleep(config.SHORT_DELAY)
assertName(cbFrame.text1, "Two state CheckBox checked")
## BUG554017: click action doesn't move focus
#statesCheck(cbFrame.check1, "CheckBox", add_states=["checked", "focused"])
# check2 shows default states
statesCheck(cbFrame.check2, "CheckBox")

# click check1 again to uncheck it, text1 label is updated to show unchecked, 
# check1 still is focused
cbFrame.check1.click(log=True)
sleep(config.SHORT_DELAY)
assertName(cbFrame.text1, "Two state CheckBox unchecked")
## BUG554017
#statesCheck(cbFrame.check1, "CheckBox", add_states=["focused"])
# check2 shows default states
statesCheck(cbFrame.check2, "CheckBox")

###############################
# Do Click action for checkbox2
###############################
# click check2 to raise "checked" state, text2 is updated check2 is focused
cbFrame.check2.click(log=True)
sleep(config.SHORT_DELAY)
assertName(cbFrame.text2, "Three state CheckBox checked")
## BUG554017
#statesCheck(cbFrame.check2, "CheckBox", add_states=["checked", "focused"])
# check1 shows default states
statesCheck(cbFrame.check1, "CheckBox")

# click check2 again to indeterminate it but still focused, text2 is updated 
# to show indeterminate
cbFrame.check2.click(log=True)
sleep(config.SHORT_DELAY)
assertName(cbFrame.text2, "Three state CheckBox indeterminate")
## BUG554017:
#statesCheck(cbFrame.check2, "CheckBox", add_states=["indeterminate", "focused"])
# check1 shows default states
#statesCheck(cbFrame.check1, "CheckBox")

# click check2 again to uncheck it, text2 label is updated to show unchecked 
# but still focused
cbFrame.check2.click(log=True)
sleep(config.SHORT_DELAY)
assertName(cbFrame.text2, "Three state CheckBox unchecked")
## BUG554017:
#statesCheck(cbFrame.check2, "CheckBox", add_states=["focused"])
# check1 shows default states
#statesCheck(cbFrame.check1, "CheckBox")

###################
# Key nevigation
###################
# move focus to check1
cbFrame.check1.mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(cbFrame.check1, "CheckBox", add_states=["focused"])
statesCheck(cbFrame.check2, "CheckBox")

# press "space" to check check1
cbFrame.check1.keyCombo("space", grabFocus=True)
sleep(config.SHORT_DELAY)
statesCheck(cbFrame.check1, "CheckBox", add_states=["focused", "checked"])

# press "space" to uncheck check1
cbFrame.check1.keyCombo("space", grabFocus=True)
sleep(config.SHORT_DELAY)
statesCheck(cbFrame.check1, "CheckBox", add_states=["focused"])

# press Tab move focus to check2
cbFrame.keyCombo("Tab", grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(cbFrame.check2, "CheckBox", add_states=["focused"])
statesCheck(cbFrame.check1, "CheckBox")

# press "space" to check check2
cbFrame.check2.keyCombo("space", grabFocus=True)
sleep(config.SHORT_DELAY)
statesCheck(cbFrame.check2, "CheckBox", add_states=["focused", "checked"])

# press "space" to indeterminate check2
cbFrame.check2.keyCombo("space", grabFocus=True)
sleep(config.SHORT_DELAY)
statesCheck(cbFrame.check2, "CheckBox", add_states=["focused", "indeterminate"])

# press "space" to uncheck check2
cbFrame.check2.keyCombo("space", grabFocus=True)
sleep(config.SHORT_DELAY)
statesCheck(cbFrame.check2, "CheckBox", add_states=["focused"])

print "INFO:  Log written to: %s" % config.OUTPUT_DIR

#close application frame window
quit(cbFrame)
