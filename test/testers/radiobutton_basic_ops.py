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
rbFrame.actionsCheck(rbFrame.button1)

#check radiobutton's states list
rbFrame.statesCheck(rbFrame.button2)
rbFrame.statesDisableCheck(rbFrame.button3)

#click radiobutton 'Female' to renew label's text 
rbFrame.click(rbFrame.button2)
sleep(config.SHORT_DELAY)
rbFrame.assertLabel("You are Female")

#click radiobutton 'Male', button1 rise 'checked' state
rbFrame.click(rbFrame.button1)
sleep(config.SHORT_DELAY)
rbFrame.assertChecked(rbFrame.button1)
#button2 disappear 'checked' state
rbFrame.assertUnchecked(rbFrame.button2)


print "INFO:  Log written to: %s" % config.OUTPUT_DIR

#close application frame window
rbFrame.quit()
