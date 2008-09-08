#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        09/08/2008
# Description: Test accessibility of numericupdown widget 
#              Use the numericupdownframe.py wrapper script
#              Test the samples/numericupdown.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of numericupdown widget
"""

# imports
import sys
import os

from strongwind import *
from numericupdown import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the numericupdown sample application
try:
  app = launchNumericUpDown(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
nudFrame = app.numericUpDownFrame

#check numericupdown's states list
nudFrame.statesCheck(nudFrame.numericupdown)

#set numericupdown's value to 0
nudFrame.valueNumericUpDown(0)
sleep(config.SHORT_DELAY)
nudFrame.assertValue(0)

#set numericupdown's value to 100
nudFrame.valueNumericUpDown(100)
sleep(config.SHORT_DELAY)
nudFrame.assertValue(100)

#set numericupdown's value to maximumValue
nudFrame.valueNumericUpDown(nudFrame.maximumValue)
sleep(config.SHORT_DELAY)
nudFrame.assertValue(nudFrame.maximumValue)

#set numericupdown's value to maximumValue+1
nudFrame.valueNumericUpDown(nudFrame.maximumValue + 1)
sleep(config.SHORT_DELAY)
nudFrame.assertValue(nudFrame.maximumValue + 1)

#set numericupdown's value to minimumValue
nudFrame.valueNumericUpDown(nudFrame.minimumValue)
sleep(config.SHORT_DELAY)
nudFrame.assertValue(nudFrame.minimumValue)

#set numericupdown's value to minimumValue-1
nudFrame.valueNumericUpDown(nudFrame.minimumValue - 1)
sleep(config.SHORT_DELAY)
nudFrame.assertValue(nudFrame.minimumValue - 1)

print "INFO:  Log written to: %s" % config.OUTPUT_DIR

#close application frame window
nudFrame.quit()
