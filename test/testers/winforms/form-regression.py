#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        08/11/2008
# Description: Test accessibility of form widget 
#              Use the formframe.py wrapper script
#              Test the samples/winforms/form.py script
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

###########################
# check forms's AtkAccessible
###########################
#check main form's states with 'active' state
statesCheck(fFrame, "Form", add_states=["active"])

###########################
# check ExtraForm(SWF.MessageBox)'s AtkAccessible
###########################
#click button1 to appear extra message widget
fFrame.click(fFrame.button1)
sleep(config.SHORT_DELAY)
#message = fFrame.app.findFrame("Message Form")
extra_form_1 = fFrame.app.findDialog("Message Form")
statesCheck(extra_form_1, "Form", add_states=["active", "modal"], invalid_states=["resizable"])

#check main form's states without 'active'
statesCheck(fFrame, "Form")

# click frame, but MessageBox window should remain active
fFrame.mouseClick()
#check main form's states again without 'active'
statesCheck(fFrame, "Form")
# make sure that the message widget's states stay the same
statesCheck(extra_form_1, "Form", add_states=["active", "modal"], invalid_states=["resizable"])

#close message form widget, main form rise 'active' state again
extra_form_1.altF4(assertClosed=False)
statesCheck(fFrame, "Form", add_states=["active"])

###########################
# check ExtraForm(Frame)'s AtkAccessible
###########################
#click button2 to appear extra empty form widget
fFrame.click(fFrame.button2)
sleep(config.SHORT_DELAY)
extra_form_2 = fFrame.app.findFrame("Extra Form")

#check extra form widget's states with 'active' state
statesCheck(extra_form_2, "Form", add_states=["active"])
#check main form's states without 'active'
statesCheck(fFrame, "Form")

# click main frame, which should become active
fFrame.mouseClick()
statesCheck(fFrame, "Form", add_states=["active"])
#check extra form's states without 'active'
statesCheck(extra_form_2, "Form")

#close extra form widget, main form rise 'active' state again
extra_form_2.mouseClick()
extra_form_2.altF4()
statesCheck(fFrame, "Form", add_states=["active"])

###########################
# check ExtraForm(Dialog)'s AtkAccessible
###########################
#click button3 to appear extra empty form widget
fFrame.click(fFrame.button3)
sleep(config.SHORT_DELAY)
extra_form_3 = fFrame.app.findDialog("Extra Form")

#check extra form widget's states with 'active' state
statesCheck(extra_form_3, "Form", add_states=["active", "modal"])
#check main form's states without 'active'
statesCheck(fFrame, "Form")

# click frame, but MessageBox window should remain active
fFrame.mouseClick()
# check main form's states again which is without 'active'
statesCheck(fFrame, "Form")
# make sure that the message widget's states stay the same
statesCheck(extra_form_3, "Form", add_states=["active", "modal"])

# close message form widget, main form rise 'active' state again
extra_form_3.altF4()
statesCheck(fFrame, "Form", add_states=["active"])

#close main form window
fFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
