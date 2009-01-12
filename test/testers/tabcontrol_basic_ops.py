#!/usr/bin/env python

##
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        01/12/2009
# Description: Test accessibility of tabcontrol widget 
#              Use the tabcontrolframe.py wrapper script
#              Test the samples/tabcontrol.py script
##

# The docstring below  is used in the generated log file
"""
Test accessibility of tabcontrol widget
"""

# imports
import sys
import os

from strongwind import *
from tabcontrol import *
from helpers import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the tabcontrol sample application
try:
  app = launchTabControl(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
tcFrame = app.tabControlFrame

#check TabControl's default states
statesCheck(tcFrame.tabcontrol, "TabControl")

#select Tab 1, label in statusbar shows "The current tab is: Tab 1"
tcFrame.assertSelectionChild(tcFrame.tabcontrol, 1)
sleep(config.SHORT_DELAY)
tcFrame.assertTabChange("Tab 1")

statesCheck(tcFrame.tabpage1, "TabPage", add_states=["focused", "selected"])

#select Tab 2, label in statusbar shows "The current tab is: Tab 2"
tcFrame.assertSelectionChild(tcFrame.tabcontrol, 2)
sleep(config.SHORT_DELAY)
tcFrame.assertTabChange("Tab 2")

statesCheck(tcFrame.tabpage2, "TabPage", add_states=["focused", "selected"])

#select Tab 3, label in statusbar shows "The current tab is: Tab 3"
tcFrame.assertSelectionChild(tcFrame.tabcontrol, 3)
sleep(config.SHORT_DELAY)
tcFrame.assertTabChange("Tab 3")

statesCheck(tcFrame.tabpage3, "TabPage", add_states=["focused", "selected"])

#select Tab 0, label in statusbar shows "The current tab is: Tab 0"
tcFrame.assertSelectionChild(tcFrame.tabcontrol, 0)
sleep(config.SHORT_DELAY)
tcFrame.assertTabChange("Tab 0")

statesCheck(tcFrame.tabpage0, "TabPage", add_states=["focused", "selected"])

#close application frame window
tcFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
