#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        09/04/2008
# Description: Test accessibility of progressbar widget 
#              Use the progressbarframe.py wrapper script
#              Test the samples/progressbar.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of progressbar widget
"""

# imports
import sys
import os

from strongwind import *
from progressbar import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the progressbar sample application
try:
  app = launchProgressBar(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
pbFrame = app.progressBarFrame

#check progressbar's states list
pbFrame.statesCheck(pbFrame.progressbar)

#click button the first time
pbFrame.click(pbFrame.button)
sleep(config.SHORT_DELAY)
pbFrame.assertLabel("10%")

#click button the second time
pbFrame.click(pbFrame.button)
sleep(config.SHORT_DELAY)
pbFrame.assertValue(20)

#set progressbar's value to 60
pbFrame.valueProgressBar(60)
sleep(config.SHORT_DELAY)
pbFrame.assertLabel("60%")

#set progressbar's value to 100
pbFrame.valueProgressBar(100)
sleep(config.SHORT_DELAY)
pbFrame.assertValue(100)

print "INFO:  Log written to: %s" % config.OUTPUT_DIR

#close application frame window
pbFrame.quit()
