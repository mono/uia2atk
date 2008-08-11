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

#check form's states list
fFrame.statesCheck()

#close form window
fFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
