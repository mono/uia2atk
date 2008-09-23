#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        07/22/2008
# Description: Test accessibility of button widget 
#              Use the buttonframe.py wrapper script
#              Test the samples/button_label_linklabel.py script
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

#check Button's actions list
actionsCheck(bFrame.button1, "Button")
actionsCheck(bFrame.button2, "Button")
actionsCheck(bFrame.button3, "Button")

#check Button's original states
statesCheck(bFrame.button1, "Button")
statesCheck(bFrame.button2, "Button")
statesCheck(bFrame.button3, "Button", 
                   invalid_states=["focusable","sensitive", "enabled"])

#move keyboard focus to button1, rise 'focused' state
bFrame.keyCombo("Down", grabFocus=False)
statesCheck(bFrame.button1, "Button", add_states=["focused"])

#move keyboard focus to button2 to rise 'focused' state, button1 get rid of
#'focused' state
bFrame.keyCombo("Down", grabFocus=False)
statesCheck(bFrame.button2, "Button", add_states=["focused"])
statesCheck(bFrame.button1, "Button")

#can't focus insensitive button3, states invariable
bFrame.keyCombo("Down", grabFocus=False)
statesCheck(bFrame.button3, "Button", 
                   invalid_states=["focusable","sensitive", "enabled"])

#click button1 rise message frame window
bFrame.click(bFrame.button1)
sleep(config.SHORT_DELAY)
bFrame.assertMessage()

#click button2 to change label text
bFrame.click(bFrame.button2)
sleep(config.SHORT_DELAY)
bFrame.assertLabel('You have clicked me 1 times')

#click button2 again to change label text
bFrame.click(bFrame.button2)
sleep(config.SHORT_DELAY)
bFrame.assertLabel('You have clicked me 2 times')

#click button3
bFrame.mouseClick(bFrame.button3)
sleep(config.SHORT_DELAY)
bFrame.assertLabel('You have clicked me 2 times')

print "INFO:  Log written to: %s" % config.OUTPUT_DIR

#close application frame window
bFrame.quit()
