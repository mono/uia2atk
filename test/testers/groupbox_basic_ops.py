#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        08/19/2008
# Description: Test accessibility of groupbox widget 
#              Use the groupboxframe.py wrapper script
#              Test the samples/groupbox.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of groupbox widget
"""

# imports
import sys
import os

from strongwind import *
from groupbox import *
from helpers import *
from sys import argv
from os import path

app_path = None 
try:
    app_path = argv[1]
except IndexError:
    pass #expected

# open the groupbox sample application
try:
    app = launchGroupBox(app_path)
except IOError, msg:
    print "ERROR:  %s" % msg
    exit(2)

# make sure we got the app back
if app is None:
    exit(4)

# just an alias to make things shorter
gbFrame = app.groupBoxFrame

# check if button in panel still have correct actions
actionsCheck(gbFrame.button1, "Button")
actionsCheck(gbFrame.button2, "Button")

# check if button in panel still have correct states
statesCheck(gbFrame.button1, "Button", add_states=["focused"])
statesCheck(gbFrame.button2, "Button")

# check if label in panel still have correct states
statesCheck(gbFrame.label1, "Label")
statesCheck(gbFrame.label2, "Label")

# check Panel's states
statesCheck(gbFrame.panel1, "Panel")
statesCheck(gbFrame.panel2, "Panel")

# Only controls that are contained within the GroupBox control can be selected 
# or receive focus. The complete GroupBox itself cannot be selected or receive 
# focus. so pressing Tab key would move focused state from button1 to button2
gbFrame.keyCombo("Tab", grabFocus=False)

statesCheck(gbFrame.button1, "Button")

statesCheck(gbFrame.button2, "Button", add_states=["focused"])

# check Panel's states
statesCheck(gbFrame.panel1, "Panel")
statesCheck(gbFrame.panel2, "Panel")

# press Tab key again to move focused state from button2 to button1
gbFrame.keyCombo("Tab", grabFocus=False)

statesCheck(gbFrame.button2, "Button")

statesCheck(gbFrame.button1, "Button", add_states=["focused"])

# check Panel's states
statesCheck(gbFrame.panel1, "Panel")
statesCheck(gbFrame.panel2, "Panel")

# click button1 in groupbox1 to update label
gbFrame.button1.click(log=True)
sleep(config.SHORT_DELAY)
gbFrame.assertLabel('1')

# check Panel's states
statesCheck(gbFrame.panel1, "Panel")
statesCheck(gbFrame.panel2, "Panel")

# click button2 two times in groupbox2 by click and mouseClick to update label
gbFrame.button2.click(log=True)
gbFrame.button2.mouseClick()
sleep(config.SHORT_DELAY)
gbFrame.assertLabel('2')
# button2 rise 'focused' state
statesCheck(gbFrame.button2, "Button", add_states=["focused"])

# check Panel's states
statesCheck(gbFrame.panel1, "Panel")
statesCheck(gbFrame.panel2, "Panel")

# close application frame window
gbFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
