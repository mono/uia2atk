#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        09/04/2008
# Description: Test accessibility of toolstripprogressbar widget 
#              Use the toolstripprogressbarframe.py wrapper script
#              Test the samples/winforms/toolstripprogressbar.py script
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

# click button to check if label is changed, in each time current value is up
# 20, label would shows "Done" after value rise to 100
tspbFrame.button.click(log=True)
sleep(config.SHORT_DELAY)
tspbFrame.assertLabel("It is 20% of 100%")
tspbFrame.assertCurrentValue(tspbFrame.progressbar, 20)
# changing the label should change the name of the progress bar, because
# progress bars get their names from labels
# BUG500402 - Accessible that gets it name from a label does not update its
# name when label text changes
# tspbFrame.findProgressBar("It is 20% of 100%")

tspbFrame.button.click(log=True)
sleep(config.SHORT_DELAY)
tspbFrame.assertLabel("It is 40% of 100%")
tspbFrame.assertCurrentValue(tspbFrame.progressbar, 40)
# BUG500402
# tspbFrame.findProgressBar("It is 40% of 100%")

tspbFrame.button.click(log=True)
sleep(config.SHORT_DELAY)
tspbFrame.assertLabel("It is 60% of 100%")
tspbFrame.assertCurrentValue(tspbFrame.progressbar, 60)
# BUG500402
# tspbFrame.findProgressBar("It is 60% of 100%")

tspbFrame.button.click(log=True)
sleep(config.SHORT_DELAY)
tspbFrame.assertLabel("It is 80% of 100%")
tspbFrame.assertCurrentValue(tspbFrame.progressbar, 80)
# BUG500402
# tspbFrame.findProgressBar("It is 80% of 100%")

tspbFrame.button.click(log=True)
sleep(config.SHORT_DELAY)
tspbFrame.assertLabel("It is 100% of 100%")
tspbFrame.assertCurrentValue(tspbFrame.progressbar, 100)
# BUG500402
# tspbFrame.findProgressBar("It is 100% of 100%")

# label shows "Done" when click button again after the value rise to 100%,
# value is still 100% 

tspbFrame.button.click(log=True)
sleep(config.SHORT_DELAY)
tspbFrame.assertLabel("Done")
tspbFrame.assertCurrentValue(tspbFrame.progressbar, 100)
# BUG500402
# tspbFrame.findProgressBar("Done")

# value doesn't changed by giving number in acceciser
tspbFrame.assignValue(10)
sleep(config.SHORT_DELAY)
tspbFrame.assertCurrentValue(tspbFrame.progressbar, 100)

tspbFrame.assignValue(100)
sleep(config.SHORT_DELAY)
tspbFrame.assertCurrentValue(tspbFrame.progressbar, 100)

tspbFrame.assignValue(-1)
sleep(config.SHORT_DELAY)
tspbFrame.assertCurrentValue(tspbFrame.progressbar, 100)

tspbFrame.assignValue(101)
sleep(config.SHORT_DELAY)
tspbFrame.assertCurrentValue(tspbFrame.progressbar, 100)

# maximumValue is 100 and minimumValue is 0
tspbFrame.assertMaximumValue()
tspbFrame.assertMinimumValue()

# check progressbar's states list again
statesCheck(tspbFrame.progressbar, "ProgressBar")

print "INFO:  Log written to: %s" % config.OUTPUT_DIR

# close application frame window
tspbFrame.quit()
