#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        08/11/2008
# Description: Test accessibility of form widget 
#              Use the formframe.py wrapper script
#              Test the samples/form.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of form widget
"""

# imports
import sys
import os

from strongwind import *
from form import *
from helpers import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the form sample application
try:
  app = launchForm(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

sleep(config.SHORT_DELAY)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
fFrame = app.formFrame

#check main form's states with 'active' state
statesCheck(fFrame, "Form", add_states=["active"])

#click button1 to appear extra message widget
fFrame.click(fFrame.button1)
sleep(config.SHORT_DELAY)
message = fFrame.app.findFrame("Message Form")

#check extra message widget's states with 'active' state, without 'resizable'
statesCheck(message, "Form", invalid_states=["resizable"], add_states=["active"])

#check main form's states without 'active'
statesCheck(fFrame, "Form")

# click frame, but MessageBox window should remain active
fFrame.mouseClick()
#check main form's states again without 'active'
statesCheck(fFrame, "Form")
# make sure that the message widget's states stay the same
statesCheck(message, "Form", invalid_states=["resizable"], add_states=["active"])

#close message form widget, main form rise 'active' state again
message.altF4()
statesCheck(fFrame, "Form", add_states=["active"])

#click button2 to appear extra empty form widget
fFrame.click(fFrame.button2)
sleep(config.SHORT_DELAY)
extraform = fFrame.app.findFrame("Extra Form")

#check extra form widget's states with 'active' state
statesCheck(extraform, "Form", add_states=["active"])

# click main frame, which should become active
fFrame.mouseClick()
statesCheck(fFrame, "Form", add_states=["active"])
#check main form's states without 'active'
statesCheck(extraform, "Form")

#close extra form widget, main form rise 'active' state again
extraform.mouseClick()
sleep(config.SHORT_DELAY)
extraform.altF4()
statesCheck(fFrame, "Form", add_states=["active"])

#close main form window
fFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
