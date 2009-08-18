#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        07/22/2008
# Description: Test accessibility of button widget 
#              Use the buttonframe.py wrapper script
#              Test the samples/winforms/button_label_linklabel.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of button widget
"""

# imports
import sys
import os

from strongwind import *
from button import *
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
    app = launchButton(app_path)
except IOError, msg:
    print "ERROR:  %s" % msg
    exit(2)

# make sure we got the app back
if app is None:
    exit(4)

# just an alias to make things shorter
bFrame = app.buttonFrame

# check Button's actions list
actionsCheck(bFrame.button1, "Button")
actionsCheck(bFrame.button2, "Button")
actionsCheck(bFrame.button3, "Button")
actionsCheck(bFrame.button4, "Button")

# check Button's original states
statesCheck(bFrame.button1, "Button")
statesCheck(bFrame.button2, "Button")
statesCheck(bFrame.button3, "Button", 
                   invalid_states=["focusable","sensitive", "enabled"])
statesCheck(bFrame.button4, "Button")

# move keyboard focus to button1, rise 'focused' state
bFrame.keyCombo("Down", grabFocus=False)
statesCheck(bFrame.button1, "Button", add_states=["focused"])

# move keyboard focus to button2 to rise 'focused' state, button1 get rid of
# 'focused' state
bFrame.keyCombo("Down", grabFocus=False)
statesCheck(bFrame.button2, "Button", add_states=["focused"])
statesCheck(bFrame.button1, "Button")

# another down key should skip button3 (because it is insensitive) and focus on
# button4
bFrame.keyCombo("Down", grabFocus=False)
statesCheck(bFrame.button4, "Button", add_states=["focused"])

# can't focus insensitive button3, states invariable
statesCheck(bFrame.button3, "Button", 
                   invalid_states=["focusable","sensitive", "enabled"])

# click button1 rise message frame window
bFrame.button1.click(log=True)
sleep(config.SHORT_DELAY)
bFrame.assertMessage()

# click button2 to change label text
bFrame.button2.click(log=True)
sleep(config.SHORT_DELAY)
bFrame.assertText(bFrame.label, 'You have clicked me 1 times')

# click button2 again to change label text
bFrame.button2.click(log=True)
sleep(config.SHORT_DELAY)
bFrame.assertText(bFrame.label, 'You have clicked me 2 times')

# click button3
bFrame.button3.mouseClick()
sleep(config.SHORT_DELAY)
bFrame.assertText(bFrame.label, 'You have clicked me 2 times')

# implement button's image
bFrame.assertImage(bFrame.button1, -1, -1)
bFrame.assertImage(bFrame.button2, -1, -1)
bFrame.assertImage(bFrame.button3, -1, -1)
bFrame.assertImage(bFrame.button4, 60, 38)

print "INFO:  Log written to: %s" % config.OUTPUT_DIR

# close application frame window
bFrame.quit()
