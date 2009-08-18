#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        09/02/2008
# Description: Test accessibility of tooltip widget 
#              Use the tooltipframe.py wrapper script
#              Test the samples/winforms/tooltip.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of tooltip widget
"""

# imports
import sys
import os

from strongwind import *
from tooltip import *
from sys import argv
from os import path
from helpers import *

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the tooltip sample application
try:
  app = launchToolTip(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

sleep(config.SHORT_DELAY)

# make sure we got the app back
if app is None:
  exit(4)

TIP1 = "show button's tooltip"
TIP2 = "my favorite fruit"

# just an alias to make things shorter
ttFrame = app.toolTipFrame

#move mouse to button to rise tooltip 1
ttFrame.button.mouseMove()
sleep(config.SHORT_DELAY)
ttFrame.assertTooltip(TIP1)

#check states of tooltips
statesCheck(ttFrame.tooltip, "ToolTip")

#move mouse to label
ttFrame.label1.mouseMove()
sleep(config.SHORT_DELAY)

#now there should be no tooltips
ttFrame.assertNoTooltip(TIP1)
ttFrame.assertNoTooltip(TIP2)

#move mouse to checkbox to rise tooltip 2
ttFrame.checkbox.mouseMove()
sleep(config.SHORT_DELAY)
ttFrame.assertTooltip(TIP2)

#check states of tooltips
statesCheck(ttFrame.tooltip, "ToolTip")

#move mouse to label
ttFrame.label1.mouseMove()
sleep(config.SHORT_DELAY)

#now there should be no tooltip 2
ttFrame.assertNoTooltip(TIP2)

# make sure that the tooltips rise in succession
#move mouse to button to rise tooltip 1
ttFrame.button.mouseMove()
sleep(config.SHORT_DELAY)
ttFrame.assertTooltip(TIP1)

#move mouse to checkbox to rise tooltip 2
ttFrame.checkbox.mouseMove()
sleep(config.SHORT_DELAY)
ttFrame.assertTooltip(TIP2)

#close application frame window
ttFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
