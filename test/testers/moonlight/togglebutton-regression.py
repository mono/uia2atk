#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        09/14/2009
# Description: Test accessibility of togglebutton widget 
#              Use the togglebuttonframe.py wrapper script
#              Test the Moonlight ToggleButton sample
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of togglebutton widget
"""

# imports

from strongwind import *
from togglebutton import *
from helpers import *
from sys import argv
from os import path

app_path = None 
try:
    app_path = argv[1]
except IndexError:
    pass  #expected

# open the togglebutton sample application
try:
    app = launchToggleButton(app_path)
except IOError, msg:
    print "ERROR:  %s" % msg
    exit(2)

# make sure we got the app back
if app is None:
    exit(4)

# just an alias to make things shorter
tbFrame = app.toggleButtonFrame

######################
# togglebutton Actions
######################
actionsCheck(tbFrame.toggle1, "ToggleButton")
actionsCheck(tbFrame.toggle2, "ToggleButton")

#############################
# togglebutton default States
#############################
#statesCheck(tbFrame.toggle1, "ToggleButton")
#statesCheck(tbFrame.toggle2, "ToggleButton")

###################################
# Do Click action for togglebutton1
###################################
# click toggle1 to raise "checked","focused" states
tbFrame.toggle1.click(log=True)
sleep(config.SHORT_DELAY)
# toggle1's test is updated to "Disable button2"
##Bug 556450
#assertName(tbFrame.toggle1, "Disable button2")
##Bug 553160
#statesCheck(tbFrame.toggle1, "ToggleButton", \
#                                add_states=["checked", "focused"])
# toggle2 is disabled
##Bug 553160
#statesCheck(tbFrame.toggle2, "ToggleButton", \
#                            invalid_states=["enabled", "sensitive","focusable"])

# click again to untoggle toggle1 which is focused
tbFrame.toggle1.click(log=True)
sleep(config.SHORT_DELAY)
# toggle1's test is updated to "Enable button2"
##Bug 556450
#assertName(tbFrame.toggle1, "Enable button2")
##Bug 553160
#statesCheck(tbFrame.toggle1, "ToggleButton", add_states=["focused"])
# toggle2 is enabled to show default states
##Bug 553160
#statesCheck(tbFrame.toggle2, "ToggleButton")

###################################
# Do Click action for ToggleButton2
###################################
# click toggle2 to raise "checked","focused" states
tbFrame.toggle2.click(log=True)
sleep(config.SHORT_DELAY)
##Bug 553160
#statesCheck(tbFrame.toggle2, "ToggleButton", \
#                                add_states=["checked", "focused"])
# toggle2's test is updated to "Checked"
##Bug 556450
#assertName(tbFrame.toggle2, "Checked")

# click toggle2 again to raise "Indetermined","focused" states
tbFrame.toggle2.click(log=True)
sleep(config.SHORT_DELAY)
# toggle2's test is updated to "Indetermined"
##Bug 556450
#assertName(tbFrame.toggle2, "Indetermined")
##Bug 553160
#statesCheck(tbFrame.toggle2, "ToggleButton", \
#                                add_states=["Indetermined", "focused"])

# click toggle2 third time to untoggle it
tbFrame.toggle2.click(log=True)
sleep(config.SHORT_DELAY)
# toggle2's test is updated to "UnChecked"
##Bug 556450
#assertName(tbFrame.toggle2, "UnChecked")
##Bug 553160
#statesCheck(tbFrame.toggle2, "ToggleButton", \
#                                add_states=["focused"])

###################
# Key nevigation
###################
# move focus to toggle1
#tbFrame.keyCombo("Tab", grabFouse=False)
tbFrame.frame.mouseClick()
sleep(config.SHORT_DELAY)
##Bug 553160
#statesCheck(tbFrame.toggle1, "ToggleButton", add_states=["focused"])
##Bug 553160
#statesCheck(tbFrame.toggle2, "ToggleButton")

# press "space" to check toggle1
tbFrame.keyCombo("space", grabFocus=False)
sleep(config.SHORT_DELAY)
assertName(tbFrame.toggle1, "Disable button2")
##Bug 553160
#statesCheck(tbFrame.toggle1, "ToggleButton", add_states=["focused", "checked"])
statesCheck(tbFrame.toggle2, "ToggleButton", \
                            invalid_states=["enabled", "sensitive","focusable"])

# press Tab couldn't move focus to toggle2 because it's disabled
tbFrame.keyCombo("Tab", grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(tbFrame.toggle2, "ToggleButton", \
                            invalid_states=["enabled", "sensitive","focusable"])
##Bug 553160
#statesCheck(tbFrame.toggle1, "ToggleButton", add_states=["focused", "checked"])

# press "space" to untoggle toggle1
tbFrame.keyCombo("space", grabFocus=False)
sleep(config.SHORT_DELAY)
assertName(tbFrame.toggle1, "Enable button2")
##Bug 553160
#statesCheck(tbFrame.toggle1, "ToggleButton", add_states=["focused"])
##Bug 553160
#statesCheck(tbFrame.toggle2, "ToggleButton")

# press Tab move focus to toggle2
tbFrame.keyCombo("Tab", grabFocus=False)
sleep(config.SHORT_DELAY)
##Bug 553160
#statesCheck(tbFrame.toggle2, "ToggleButton", add_states=["focused"])
##Bug 553160
#statesCheck(tbFrame.toggle1, "ToggleButton")

# press "space" to check toggle2
tbFrame.keyCombo("space", grabFocus=False)
sleep(config.SHORT_DELAY)
assertName(tbFrame.toggle2, "Checked")
##Bug 553160
#statesCheck(tbFrame.toggle2, "ToggleButton", add_states=["focused", "checked"])

# press "enter" to indeterminate toggle2
tbFrame.keyCombo("Enter", grabFocus=False)
sleep(config.SHORT_DELAY)
assertName(tbFrame.toggle2, "Indetermined")
##Bug 553160
#statesCheck(tbFrame.toggle2, "ToggleButton", \
#                                     add_states=["focused", "indetermined"])

# press "enter" to untoggle toggle2
tbFrame.keyCombo("Enter", grabFocus=False)
sleep(config.SHORT_DELAY)
assertName(tbFrame.toggle2, "UnChecked")
##Bug 553160
#statesCheck(tbFrame.toggle2, "ToggleButton", \
#                                     add_states=["focused"])

print "INFO:  Log written to: %s" % config.OUTPUT_DIR

#close application frame window
quit(tbFrame)
