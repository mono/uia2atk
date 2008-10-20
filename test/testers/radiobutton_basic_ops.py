#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        07/29/2008
# Description: Test accessibility of radiobutton widget 
#              Use the radiobuttonframe.py wrapper script
#              Test the samples/checkbox_radiobutton.py script
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
  pass #expected

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

#check radiobutton's actions list
actionsCheck(rbFrame.button1, "RadioButton")
actionsCheck(rbFrame.button2, "RadioButton")
actionsCheck(rbFrame.button3, "RadioButton")

#check radiobutton1 with 'checked' 'focused' states
rbFrame.keyCombo("Tab", grabFocus=False)
statesCheck(rbFrame.button1, "RadioButton", 
                           add_states=["focused", "checked"])

#check radiobutton2 with default states
statesCheck(rbFrame.button2, "RadioButton")

#check radiobutton3's states
statesCheck(rbFrame.button3, "RadioButton",
                           invalid_states=["focusable", "sensitive", "enabled"])

#press "right" key to move focused state to button2
rbFrame.keyCombo("Right", grabFocus=False)
statesCheck(rbFrame.button2, "RadioButton", add_states=["focused"])
#button1 get rid of focused state
statesCheck(rbFrame.button1, "RadioButton", 
                           add_states=["checked"])

#press "space" key to select button2 to rise checked state
rbFrame.keyCombo("space", grabFocus=False)
statesCheck(rbFrame.button2, "RadioButton", add_states=["focused", "checked"])
#button1 get rid of checked state
statesCheck(rbFrame.button1, "RadioButton")

#click radiobutton1 to renew label's text 
rbFrame.click(rbFrame.button1)
sleep(config.SHORT_DELAY)
rbFrame.assertLabel("You are Male")
#radiobutton1 rise 'checked' state
statesCheck(rbFrame.button1, "RadioButton",
                           add_states=["checked"])
#radiobutton2 get rid of 'checked' but still focused
statesCheck(rbFrame.button2, "RadioButton",
                           add_states=["focused"])

#mouse click radiobutton1 to rise 'focused' state and still checked
rbFrame.button1.mouseClick()
statesCheck(rbFrame.button1, "RadioButton",
                           add_states=["focused", "checked"])
#radiobutton2 return to default states
statesCheck(rbFrame.button2, "RadioButton")

#click insensitive radiobutton3 doesn't change states
rbFrame.button3.mouseClick()
statesCheck(rbFrame.button3, "RadioButton",
                           invalid_states=["focusable", "sensitive", "enabled"])

#implement radiobutton's image
rbFrame.assertImageSize(rbFrame.button1, -1, -1)
rbFrame.assertImageSize(rbFrame.button2, -1, -1)
rbFrame.assertImageSize(rbFrame.button3, -1, -1)

print "INFO:  Log written to: %s" % config.OUTPUT_DIR

#close application frame window
rbFrame.quit()
