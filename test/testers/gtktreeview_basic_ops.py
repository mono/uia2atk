#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        06/18/2008
# Description: Test accessibility of TreeView widget 
#              Use the treeviewframe.py wrapper script
#              Test the sample/gtktreeview.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of checkbutton widget
"""

# imports
import sys
import os

from strongwind import *
from gtktreeview import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the treeview sample application
try:
  app = launchTreeView(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
tvFrame = app.treeViewFrame


#expand parent 0
tvFrame.expand(tvFrame.parent0,'expand or contract')
sleep(config.SHORT_DELAY)
tvFrame.assertResult(tvFrame.parent0, "expanded")

#contract parent0
tvFrame.contract(tvFrame.parent0,'expand or contract')
sleep(config.SHORT_DELAY)
tvFrame.assertResult(tvFrame.parent0, "contracted")

#expand parent 1
tvFrame.expand(tvFrame.parent1,'expand or contract')
sleep(config.SHORT_DELAY)
tvFrame.assertResult(tvFrame.parent1, "expanded")

#contract parent1
tvFrame.contract(tvFrame.parent1,'expand or contract')
sleep(config.SHORT_DELAY)
tvFrame.assertResult(tvFrame.parent1, "contracted")

#expand parent 2
tvFrame.expand(tvFrame.parent2,'expand or contract')
sleep(config.SHORT_DELAY)
tvFrame.assertResult(tvFrame.parent2, "expanded")

#contract parent2
tvFrame.contract(tvFrame.parent2,'expand or contract')
sleep(config.SHORT_DELAY)
tvFrame.assertResult(tvFrame.parent2, "contracted")

#expand parent 3
tvFrame.expand(tvFrame.parent3,'expand or contract')
sleep(config.SHORT_DELAY)
tvFrame.assertResult(tvFrame.parent3, "expanded")

#contract parent3
tvFrame.contract(tvFrame.parent3,'expand or contract')
sleep(config.SHORT_DELAY)
tvFrame.assertResult(tvFrame.parent3, "contracted")

print "INFO:  Log written to: %s" % config.OUTPUT_DIR

tvFrame.altF4(tvFrame)

