#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        08/05/2008
# Description: Test accessibility of hscrollbar widget 
#              Use the hscrollbarframe.py wrapper script
#              Test the samples/hscrollbar.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of hscrollbar widget
"""

# imports
import sys
import os

from strongwind import *
from hscrollbar import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the hscrollbar sample application
try:
  app = launchHScrollBar(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
hsbFrame = app.hScrollBarFrame

#check hscrollbar's states list
hsbFrame.statesCheck(hsbFrame.hscrollbar)

#set value to 50
hsbFrame.valueScrollBar(50)
sleep(config.SHORT_DELAY)
hsbFrame.assertScrollbar(50)

#set value to 0
hsbFrame.valueScrollBar(0)
sleep(config.SHORT_DELAY)
hsbFrame.assertScrollbar(0)

#set value to 100
hsbFrame.valueScrollBar(100)
sleep(config.SHORT_DELAY)
hsbFrame.assertScrollbar(100)

#set value to 100
hsbFrame.valueScrollBar(119)
sleep(config.SHORT_DELAY)
hsbFrame.assertScrollbar(119)


#set value to -10
hsbFrame.valueScrollBar(-10)
sleep(config.SHORT_DELAY)
hsbFrame.assertScrollbar(-10)

#set value to 210
hsbFrame.valueScrollBar(210)
sleep(config.SHORT_DELAY)
hsbFrame.assertScrollbar(210)

print "INFO:  Log written to: %s" % config.OUTPUT_DIR

#close application frame window
hsbFrame.quit()
