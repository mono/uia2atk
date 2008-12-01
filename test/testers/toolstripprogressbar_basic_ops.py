#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        09/04/2008
# Description: Test accessibility of toolstripprogressbar widget 
#              Use the toolstripprogressbarframe.py wrapper script
#              Test the samples/toolstripprogressbar.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of toolstripprogressbar widget
"""

# imports
import sys
import os

from strongwind import *
from toolstripprogressbar import *
from helpers import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the toolstripprogressbar sample application
try:
  app = launchToolStripProgressBar(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
tspbFrame = app.toolStripProgressBarFrame

#check progressbar's states list
statesCheck(tspbFrame.progressbar, "ToolStripProgressBar")

#click button1 each time, current value would increase 20, when it runout to 100
#label would shows "Done"
tspbFrame.click(tspbFrame.button)
sleep(config.SHORT_DELAY)
tspbFrame.assertLabel("It is 20% of 100%")
tspbFrame.assertCurrnetValue(tspbFrame.progressbar, 20)

tspbFrame.click(tspbFrame.button)
sleep(config.SHORT_DELAY)
tspbFrame.assertLabel("It is 40% of 100%")
tspbFrame.assertCurrnetValue(tspbFrame.progressbar, 40)

tspbFrame.click(tspbFrame.button)
sleep(config.SHORT_DELAY)
tspbFrame.assertLabel("It is 60% of 100%")
tspbFrame.assertCurrnetValue(tspbFrame.progressbar, 60)

tspbFrame.click(tspbFrame.button)
sleep(config.SHORT_DELAY)
tspbFrame.assertLabel("It is 80% of 100%")
tspbFrame.assertCurrnetValue(tspbFrame.progressbar, 80)

tspbFrame.click(tspbFrame.button)
sleep(config.SHORT_DELAY)
tspbFrame.assertLabel("It is 100% of 100%")
tspbFrame.assertCurrnetValue(tspbFrame.progressbar, 100)
#label shows "Done" when you click button again after the value rise 100%
tspbFrame.click(tspbFrame.button)
sleep(config.SHORT_DELAY)
tspbFrame.assertLabel("Done")
tspbFrame.assertCurrnetValue(tspbFrame.progressbar, 100)

#if yo enter number under Value in accerciser you would still get the lastest 
#value, you can't change the progress by give value
tspbFrame.value(10)
sleep(config.SHORT_DELAY)
tspbFrame.assertCurrnetValue(tspbFrame.progressbar, 100)

tspbFrame.value(100)
sleep(config.SHORT_DELAY)
tspbFrame.assertCurrnetValue(tspbFrame.progressbar, 100)

tspbFrame.value(-1)
sleep(config.SHORT_DELAY)
tspbFrame.assertCurrnetValue(tspbFrame.progressbar, 100)

tspbFrame.value(101)
sleep(config.SHORT_DELAY)
tspbFrame.assertCurrnetValue(tspbFrame.progressbar, 100)

#maximumValue is 100 and minimumValue is 0
tspbFrame.assertValueImplemented("maximumValue")

tspbFrame.assertValueImplemented("minimumValue")

#check progressbar's states list again
statesCheck(tspbFrame.progressbar, "ProgressBar")

print "INFO:  Log written to: %s" % config.OUTPUT_DIR

#close application frame window
tspbFrame.quit()
