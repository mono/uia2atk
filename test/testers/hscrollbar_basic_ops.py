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
from helpers import *
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

# check hscrollbar's default states
statesCheck(hsbFrame.hscrollbar, "HScrollBar")

# mouse click scrollbar
hsbFrame.hscrollbar.mouseClick()
sleep(config.SHORT_DELAY)
hsbFrame.assertScrollBar(20)
# mouseClick action update label's text
hsbFrame.assertLabel(20)

# still have default states
statesCheck(hsbFrame.hscrollbar, "HScrollBar")

# keyCombo move scrollbar using Page Down
hsbFrame.hscrollbar.keyCombo("Page_Down", grabFocus=False)
sleep(config.SHORT_DELAY)
hsbFrame.assertScrollBar(40)
# keyCombo action update label's text
hsbFrame.assertLabel(40)

# keyCombo move scrollbar using Page Up
hsbFrame.hscrollbar.keyCombo("Page_Up", grabFocus=False)
sleep(config.SHORT_DELAY)
hsbFrame.assertScrollBar(20)
# keyCombo action update label's text
hsbFrame.assertLabel(20)

# keyCombo move scrollbar using Page Down
hsbFrame.hscrollbar.keyCombo("Page_Down", grabFocus=False)
sleep(config.SHORT_DELAY)
hsbFrame.assertScrollBar(40)
# keyCombo action update label's text
hsbFrame.assertLabel(40)

# set value to 10
hsbFrame.assignScrollBar(10)
sleep(config.SHORT_DELAY)
hsbFrame.assertScrollBar(10)
# update label's text
hsbFrame.assertLabel(10)

# set value to 0
hsbFrame.assignScrollBar(0)
sleep(config.SHORT_DELAY)
hsbFrame.assertScrollBar(0)
# update label's text
hsbFrame.assertLabel(0)

# set value to 100
hsbFrame.assignScrollBar(100)
sleep(config.SHORT_DELAY)
hsbFrame.assertScrollBar(100)
# update label's text
hsbFrame.assertLabel(100)

# set value to -10, minimum value is 0
hsbFrame.assignScrollBar(-10)
sleep(config.SHORT_DELAY)
hsbFrame.assertScrollBar(0)
# doesn't change label's text
hsbFrame.assertLabel(0)

# set value to 120, maximum value is 119
hsbFrame.assignScrollBar(120)
sleep(config.SHORT_DELAY)
hsbFrame.assertScrollBar(0)
# doesn't change label's text
hsbFrame.assertLabel(0)

# assert the max, min, and min increment values
# BUG499883 - Accessible maximum value of a scroll bar is 119
#hsbFrame.assertMaximumValue()
hsbFrame.assertMinimumValue()
hsbFrame.assertMinimumIncrement()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR

# close application frame window
hsbFrame.quit()
