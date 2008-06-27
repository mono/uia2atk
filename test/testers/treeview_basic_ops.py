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
from treeview import *
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
cbFrame = app.treeViewFrame


#expand parent 0
cbFrame.expand(cbFrame.parent0,'expand or contract')
sleep(config.SHORT_DELAY)
cbFrame.assertResult(cbFrame.parent0, "expanded")

#contract parent0
cbFrame.contract(cbFrame.parent0,'expand or contract')
sleep(config.SHORT_DELAY)
cbFrame.assertResult(cbFrame.parent0, "contracted")

#expand parent 1
cbFrame.expand(cbFrame.parent1,'expand or contract')
sleep(config.SHORT_DELAY)
cbFrame.assertResult(cbFrame.parent1, "expanded")

#contract parent1
cbFrame.contract(cbFrame.parent1,'expand or contract')
sleep(config.SHORT_DELAY)
cbFrame.assertResult(cbFrame.parent1, "contracted")

#expand parent 2
cbFrame.expand(cbFrame.parent2,'expand or contract')
sleep(config.SHORT_DELAY)
cbFrame.assertResult(cbFrame.parent2, "expanded")

#contract parent2
cbFrame.contract(cbFrame.parent2,'expand or contract')
sleep(config.SHORT_DELAY)
cbFrame.assertResult(cbFrame.parent2, "contracted")

#expand parent 3
cbFrame.expand(cbFrame.parent3,'expand or contract')
sleep(config.SHORT_DELAY)
cbFrame.assertResult(cbFrame.parent3, "expanded")

#contract parent3
cbFrame.contract(cbFrame.parent3,'expand or contract')
sleep(config.SHORT_DELAY)
cbFrame.assertResult(cbFrame.parent3, "contracted")

print "INFO:  Log written to: %s" % config.OUTPUT_DIR

cbFrame.altF4(cbFrame)

