#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        09/10/2009
# Description: Test accessibility of radiobutton widget 
#              Use the radiobuttonframe.py wrapper script
#              Test the Moonlight RadioButton sample
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of radiobutton widget
"""

# imports
import sys
import os

from strongwind import *
from radiobutton import *
from helpers import *
from sys import argv
from os import path

app_path = None
try:
    app_path = argv[1]
except IndexError:
    pass  #expected

# open the radiobutton sample application
try:
    app = launchRadioButton(app_path)
except IOError, msg:
    print "ERROR:  %s" % msg
    exit(2)

# make sure we got the app back
if app is None:
    exit(4)

# just an alias to make things shorter
rbFrame = app.radioButtonFrame

####################
# radio Actions
####################
actionsCheck(rbFrame.radio1, "RadioButton")
actionsCheck(rbFrame.radio2, "RadioButton")
actionsCheck(rbFrame.radio3, "RadioButton")
actionsCheck(rbFrame.radio4, "RadioButton")

######################
# radio default States
######################
statesCheck(rbFrame.radio1, "RadioButton")
statesCheck(rbFrame.radio2, "RadioButton")
statesCheck(rbFrame.radio3, "RadioButton")
statesCheck(rbFrame.radio4, "RadioButton")

#################################################
# Do Click action for radiobuttons in first group
#################################################
# click radio1 to raise "checked" state, text3 label is updated,
# click action should grab focus to radio1 to raise "focused" state
rbFrame.radio1.click(log=True)
sleep(config.SHORT_DELAY)
assertText(rbFrame.text3, "chose:First Group:radiobutton1")
statesCheck(rbFrame.radio1, "RadioButton", add_states=["checked", "focused"])
# radio2 shows default states
statesCheck(rbFrame.radio2, "RadioButton")

# click radio2, text3 label is updated, move checked and focused to radio2
rbFrame.radio2.click(log=True)
sleep(config.SHORT_DELAY)
assertText(rbFrame.text3s, "chose:First Group:radiobutton1")
statesCheck(rbFrame.radio2, "RadioButton", add_states=["checked", "focused"])
# radio1 shows default states
statesCheck(rbFrame.radio1, "RadioButton")

#################################################
# Do Click action for RadioButton in Second Group
#################################################
# click radio3 to raise "checked" state, text3 is updated
rbFrame.radio3.click(log=True)
sleep(config.SHORT_DELAY)
assertText(rbFrame.text3, "chose::radiobutton3")
statesCheck(rbFrame.radio3, "RadioButton", add_states=["checked", "focused"])
# radio2 still is checked, action doesn't affect radiobuttons in different group
statesCheck(rbFrame.radio2, "RadioButton", add_states=["checked"])

# click radio4, text3 is updated
rbFrame.radio4.click(log=True)
sleep(config.SHORT_DELAY)
assertText(rbFrame.text3, "chose::radiobutton4")
statesCheck(rbFrame.radio4, "RadioButton", add_states=["checked", "focused"])
# radio3 shows default states
statesCheck(rbFrame.radio3, "RadioButton")
# radio2 still is checked
statesCheck(rbFrame.radio2, "RadioButton", add_states=["checked"])

###################
# Key nevigation
###################
# move focus to radio1
rbFrame.keyCombo("Tab", grabFouse=False)
sleep(config.SHORT_DELAY)
statesCheck(rbFrame.radio1, "RadioButton", add_states=["focused"])
statesCheck(rbFrame.radio4, "RadioButton")

# press "space" to check radio1
rbFrame.keyCombo("space", grabFocus=False)
sleep(config.SHORT_DELAY)
assertText(rbFrame.text3, "chose:First Group:radiobutton1")
statesCheck(rbFrame.radio1, "RadioButton", add_states=["focused", "checked"])

# press Tab move focus to radio2, radio1 still checked
rbFrame.keyCombo("Tab", grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(rbFrame.radio2, "RadioButton", add_states=["focused"])
statesCheck(rbFrame.radio1, "RadioButton", add_states=["checked"])

# press "space" to check radio2
rbFrame.keyCombo("space", grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(rbFrame.radio2, "RadioButton", add_states=["focused", "checked"])

# press Tab move focus to Group2 radio3, radio2 in Group1 and radio4 in Group2
# are still checked
rbFrame.keyCombo("Tab", grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(rbFrame.radio3, "RadioButton", add_states=["focused"])
statesCheck(rbFrame.radio2, "RadioButton", add_states=["checked"])
statesCheck(rbFrame.radio4, "RadioButton", add_states=["checked"])

# press "space" to check radio3, radio2 in Group1 is still checked, radio4 in 
# Group2 turn to default
rbFrame.keyCombo("space", grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(rbFrame.radio3, "RadioButton", add_states=["focused", "checked"])
statesCheck(rbFrame.radio2, "RadioButton", add_states=["checked"])
statesCheck(rbFrame.radio4, "RadioButton")

###########################
# Image Implementation test
###########################
assertImageSize(rbFrame.radio1, expected_width=-1, expected_height=-1)
assertImageSize(rbFrame.radio2, expected_width=-1, expected_height=-1)
assertImageSize(rbFrame.radio3, expected_width=-1, expected_height=-1)
assertImageSize(rbFrame.radio4, expected_width=20, expected_height=20)

print "INFO:  Log written to: %s" % config.OUTPUT_DIR

#close application frame window
quit(rbFrame)
