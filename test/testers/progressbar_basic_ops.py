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
from helpers import *
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
statesCheck(pbFrame.progressbar, "ProgressBar")

#click button the first time
pbFrame.click(pbFrame.button)
sleep(config.SHORT_DELAY)
pbFrame.assertLabel("10%")

#click button the second time
pbFrame.click(pbFrame.button)
sleep(config.SHORT_DELAY)
pbFrame.assertValue(pbFrame.progressbar, 20)

#click button the third time
pbFrame.click(pbFrame.button)
sleep(config.SHORT_DELAY)
pbFrame.assertLabel("30%")

#click button the fourth time
pbFrame.click(pbFrame.button)
sleep(config.SHORT_DELAY)
pbFrame.assertValue(pbFrame.progressbar, 40)

#check progressbar's states list again
statesCheck(pbFrame.progressbar, "ProgressBar")

print "INFO:  Log written to: %s" % config.OUTPUT_DIR

#close application frame window
pbFrame.quit()
