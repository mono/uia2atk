#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        07/24/2008
# Description: Test accessibility of checkbox widget 
#              Use the checkboxframe.py wrapper script
#              Test the samples/checkbox_radiobutton.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of checkbox widget
"""

# imports
import sys
import os

from strongwind import *
from checkbox import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the treeview sample application
try:
  app = launchCheckBox(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
cbFrame = app.checkBoxFrame

#check CheckBox's actions list
cbFrame.actionsCheck(cbFrame.check1)

#check CheckBox's states list
cbFrame.statesCheck(cbFrame.check2)
cbFrame.statesDisableCheck(cbFrame.check4)

#click checkbox 'Chicken' 
cbFrame.click(cbFrame.check2)
sleep(config.SHORT_DELAY)
cbFrame.assertChecked(cbFrame.check2)

#click checkbox 'Stuffed Peppers'
cbFrame.click(cbFrame.check3)
sleep(config.SHORT_DELAY)
cbFrame.assertUnchecked(cbFrame.check3)


print "INFO:  Log written to: %s" % config.OUTPUT_DIR

#close application frame window
cbFrame.quit()
