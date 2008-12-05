#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        10/31/2008
# Description: Test accessibility of toolstripmenuitem widget 
#              Use the toolstripmenuitemframe.py wrapper script
#              Test the samples/toolstripmenuitem.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of toolstripmenuitem widget
"""

# imports
import sys
import os

from strongwind import *
from toolstripmenuitem import *
from helpers import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the toolstripmenuitem sample application
try:
  app = launchToolStripMenuItem(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

sleep(config.SHORT_DELAY)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
tsmiFrame = app.toolStripMenuItemFrame

tsmiFrame.view_menu.click()
sleep(config.SHORT_DELAY)
tsmiFrame.create_menu_item.click()
sleep(config.SHORT_DELAY)
tsmiFrame.assert_message_box_appeared("Clear Clicked")

#close main window
tsmiFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
