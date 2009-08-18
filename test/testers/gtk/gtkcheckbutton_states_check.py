#!/usr/bin/env python
# -*- coding: utf-8 -*-

##############################################################################
# Written by:  Brian G. Merrell <bgmerrell@novell.com>
# Date:        May 23 2008
# Description: Test accessibility of gtk checkbutton widget 
#              Use the gtkcheckbuttonframe.py wrapper script
#              Test the samples/gtk/gtkcheckbutton.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of checkbutton widget
"""

# imports
from strongwind import *
from gtkcheckbutton import *
from helpers import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the checkbutton sample application
try:
  app = launchCheckButton(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
cbFrame = app.gtkCheckButtonFrame

#check checkbox1 states with the keyboard focus
statesCheck(cbFrame.checkbox1, "CheckBox", add_states=["focused"])

#check checkbox2 states
statesCheck(cbFrame.checkbox2, "CheckBox")

#click action to check box1 and add 3 states
cbFrame.checkbox1.click()
sleep(config.SHORT_DELAY)
statesCheck(cbFrame.checkbox1, "CheckBox", 
                    add_states=["armed","focused","checked"])

#click action to uncheck box1 and delete 2 states but still focus
cbFrame.checkbox1.click()
sleep(config.SHORT_DELAY)
statesCheck(cbFrame.checkbox1, "CheckBox", add_states=["focused"])

#click and use keyCombo to move focus to checkbox2,rise 'focused' 'checked' 
#state
cbFrame.keyCombo('Down', grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(cbFrame.checkbox2, "CheckBox", add_states=["focused"])
cbFrame.keyCombo('Return', grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(cbFrame.checkbox2, "CheckBox", 
                    add_states=["armed","focused","checked"])

#click checkbox2 again, delete 'checked' state but still focus
cbFrame.checkbox2.click()
sleep(config.SHORT_DELAY)
statesCheck(cbFrame.checkbox2, "CheckBox", add_states=["focused"])

#click checkbox1 doesn't move focus
cbFrame.checkbox1.click()
sleep(config.SHORT_DELAY)
statesCheck(cbFrame.checkbox1, "CheckBox", 
                    add_states=["armed","checked"])
statesCheck(cbFrame.checkbox2, "CheckBox", add_states=["focused"])

#use mouseClick to uncheck checkbox1, delete 'checked' state but still focus
cbFrame.checkbox1.mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(cbFrame.checkbox1, "CheckBox", 
                    add_states=["focused"])

cbFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
