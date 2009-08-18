#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        08/04/2008
# Description: Test accessibility of vscrollbar widget 
#              Use the vscrollbarframe.py wrapper script
#              Test the samples/winforms/vscrollbar.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of vscrollbar widget
"""

# imports
import sys
import os

from strongwind import *
from vscrollbar import *
from helpers import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the vscrollbar sample application
try:
  app = launchVScrollBar(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
vsbFrame = app.vScrollBarFrame

# check vscrollbar's states list
statesCheck(vsbFrame.vscrollbar, "VScrollBar")

# ensure that the minimum and maximum values are what we expect
vsbFrame.assertMaximumValue()
vsbFrame.assertMinimumValue()
# ensure that the minimum increment value is what we expect
vsbFrame.assertMinimumIncrement()

# mouse click scrollbar
# the mouseClick method will, by default, click on the middle of the scrollbar,
# which scrolls down to 20
vsbFrame.vscrollbar.mouseClick()
sleep(config.SHORT_DELAY)
vsbFrame.assertScrollBar(20)
# mouseClick action update label's text
vsbFrame.assertLabel(20)

# check vscrollbar's states list
statesCheck(vsbFrame.vscrollbar, "VScrollBar")

# keyCombo move scrollbar
vsbFrame.vscrollbar.keyCombo("Page_Down", grabFocus=False)
sleep(config.SHORT_DELAY)
vsbFrame.assertScrollBar(40)
# keyCombo action update label's text
vsbFrame.assertLabel(40)

# keyCombo move scrollbar
vsbFrame.vscrollbar.keyCombo("Page_Up", grabFocus=False)
sleep(config.SHORT_DELAY)
vsbFrame.assertScrollBar(20)
# keyCombo action update label's text
vsbFrame.assertLabel(20)

# keyCombo move scrollbar
vsbFrame.vscrollbar.keyCombo("Page_Down", grabFocus=False)
sleep(config.SHORT_DELAY)
vsbFrame.assertScrollBar(40)
# keyCombo action update label's text
vsbFrame.assertLabel(40)

# set value to 10
vsbFrame.assignScrollBarValue(10)
sleep(config.SHORT_DELAY)
vsbFrame.assertScrollBar(10)
vsbFrame.assertLabel(10)

# set value to 0
vsbFrame.assignScrollBarValue(0)
sleep(config.SHORT_DELAY)
vsbFrame.assertScrollBar(0)
vsbFrame.assertLabel(0)

# set value to 100
vsbFrame.assignScrollBarValue(100)
sleep(config.SHORT_DELAY)
vsbFrame.assertScrollBar(100)
vsbFrame.assertLabel(100)

# set value to -10, minimum value is 0
vsbFrame.assignScrollBarValue(-10)
sleep(config.SHORT_DELAY)
vsbFrame.assertScrollBar(0)
vsbFrame.assertLabel(0)

# set value to maximum value
vsbFrame.assignScrollBarValue(vsbFrame.MAXVAL)
sleep(config.SHORT_DELAY)
vsbFrame.assertScrollBar(vsbFrame.MAXVAL)
vsbFrame.assertLabel(vsbFrame.MAXVAL)

# another check that the minimum and maximum values are what we expect
vsbFrame.assertMaximumValue()
vsbFrame.assertMinimumValue()
# another check that the minimum increment value is what we expect
vsbFrame.assertMinimumIncrement()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR

# close application frame window
vsbFrame.quit()
