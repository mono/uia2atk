#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        02/04/2009
# Description: Test accessibility of toolbar widget 
#              Use the toolbarframe.py wrapper script
#              Test the samples/toolbar.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of toolbar widget
"""

# imports
import sys
import os

from strongwind import *
from toolbar import *
from helpers import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the toolstrip sample application
try:
  app = launchToolBar(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

sleep(config.SHORT_DELAY)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
tbFrame = app.toolBarFrame

#assert if pyatspi can get toolbar role for toolbar
tbFrame.assertToolBar()

#check toolstrip's states
statesCheck(tbFrame.toolbar, "ToolBar")

#mouse click toolbar, then check its states never changed
##has extraneous focused state BUG472279
tbFrame.toolbar.mouseClick()
sleep(config.SHORT_DELAY)
##statesCheck(tbFrame.toolbar, "ToolBar")

#check how many children of toolbar is accessible, in this example there are 4 
#toolstripbuttons are added by toolbar.Buttons.Add, 1 separator 1 label and 1 
#combobox are added by toolbar.Controls.add
tbFrame.searchforItems("push button", 5)

tbFrame.searforchItems("separator", 1)

tbFrame.searchforItems("label", 1)

tbFrame.searchItems("combo box", 1)

tbFrame.searchItems("menu", 1)

tbFrame.searchItems("menu item", 10)

#close main window
tbFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
