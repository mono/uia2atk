#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        08/06/2008
# Description: Test accessibility of scrollbar widget 
#              Use the scrollbarframe.py wrapper script
#              Test the samples/checkedlistbox.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of scrollbar widget
"""

# imports
import sys
import os

from strongwind import *
from scrollbar import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the hscrollbar sample application
try:
  app = launchScrollBar(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
sbFrame = app.scrollBarFrame

#check hscrollbar's states list
sbFrame.statesCheck(sbFrame.scrollbar)

#set value to 50
sbFrame.valueScrollBar(5)
sleep(config.SHORT_DELAY)
sbFrame.assertScrollbar(5)

#set value to 0
sbFrame.valueScrollBar(0)
sleep(config.SHORT_DELAY)
sbFrame.assertScrollbar(0)

#set value to 100
sbFrame.valueScrollBar(10)
sleep(config.SHORT_DELAY)
sbFrame.assertScrollbar(10)

#set value to -10
sbFrame.valueScrollBar(-10)
sleep(config.SHORT_DELAY)
sbFrame.assertScrollbar(-10)

#set value to 210
sbFrame.valueScrollBar(210)
sleep(config.SHORT_DELAY)
sbFrame.assertScrollbar(210)

print "INFO:  Log written to: %s" % config.OUTPUT_DIR

#close application frame window
sbFrame.quit()
