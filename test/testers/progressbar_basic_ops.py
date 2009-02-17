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

#click button to check if label and progressbar's current value is changed
#value would return to 0 and start again after up to 100
pbFrame.click(pbFrame.button)
sleep(config.SHORT_DELAY)
pbFrame.assertLabel("20%")
pbFrame.assertCurrnetValue(pbFrame.progressbar, 20)

pbFrame.click(pbFrame.button)
sleep(config.SHORT_DELAY)
pbFrame.assertLabel("40%")
pbFrame.assertCurrnetValue(pbFrame.progressbar, 40)

pbFrame.click(pbFrame.button)
sleep(config.SHORT_DELAY)
pbFrame.assertLabel("60%")
pbFrame.assertCurrnetValue(pbFrame.progressbar, 60)

pbFrame.click(pbFrame.button)
sleep(config.SHORT_DELAY)
pbFrame.assertLabel("80%")
pbFrame.assertCurrnetValue(pbFrame.progressbar, 80)

pbFrame.click(pbFrame.button)
sleep(config.SHORT_DELAY)
pbFrame.assertLabel("100%")
pbFrame.assertCurrnetValue(pbFrame.progressbar, 100)
#start progress again after the value up to 100
pbFrame.click(pbFrame.button)
sleep(config.SHORT_DELAY)
pbFrame.assertLabel("20%")
pbFrame.assertCurrnetValue(pbFrame.progressbar, 20)

#value doesn't changed by giving number in acceciser
pbFrame.value(10)
sleep(config.SHORT_DELAY)
pbFrame.assertCurrnetValue(pbFrame.progressbar, 20)

pbFrame.value(100)
sleep(config.SHORT_DELAY)
pbFrame.assertCurrnetValue(pbFrame.progressbar, 20)

pbFrame.value(-1)
sleep(config.SHORT_DELAY)
pbFrame.assertCurrnetValue(pbFrame.progressbar, 20)

pbFrame.value(101)
sleep(config.SHORT_DELAY)
pbFrame.assertCurrnetValue(pbFrame.progressbar, 20)

#maximumValue is 100 and minimumValue is 0
pbFrame.assertValueImplemented("maximumValue")

pbFrame.assertValueImplemented("minimumValue")

#check progressbar's states list again
statesCheck(pbFrame.progressbar, "ProgressBar")

print "INFO:  Log written to: %s" % config.OUTPUT_DIR

#close application frame window
pbFrame.quit()
