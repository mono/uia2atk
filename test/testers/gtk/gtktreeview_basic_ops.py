#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        06/18/2008
# Description: Test accessibility of gtk TreeView widget 
#              Use the gtktreeviewframe.py wrapper script
#              Test the samples/winforms/gtktreeview.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of checkbutton widget
"""

# imports
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
tvFrame = app.gtkTreeViewFrame
 
#expand parent 0
tvFrame.expand(tvFrame.parent0)
sleep(config.SHORT_DELAY)
tvFrame.assertExpanded(tvFrame.parent0)
 
#contract parent0
tvFrame.contract(tvFrame.parent0)
sleep(config.SHORT_DELAY)
tvFrame.assertContracted(tvFrame.parent0)
 
#expand parent 1
tvFrame.expand(tvFrame.parent1)
sleep(config.SHORT_DELAY)
tvFrame.assertExpanded(tvFrame.parent1)
 
#contract parent1
tvFrame.contract(tvFrame.parent1)
sleep(config.SHORT_DELAY)
tvFrame.assertContracted(tvFrame.parent1)
 
#expand parent 2
tvFrame.expand(tvFrame.parent2)
sleep(config.SHORT_DELAY)
tvFrame.assertExpanded(tvFrame.parent2)
 
#contract parent2
tvFrame.contract(tvFrame.parent2)
sleep(config.SHORT_DELAY)
tvFrame.assertContracted(tvFrame.parent2)
 
#expand parent 3
tvFrame.expand(tvFrame.parent3)
sleep(config.SHORT_DELAY)
tvFrame.assertExpanded(tvFrame.parent3)
 
#contract parent3
tvFrame.contract(tvFrame.parent3)
sleep(config.SHORT_DELAY)
tvFrame.assertContracted(tvFrame.parent3)
 
# we should also make sure that clicking on the table header column reorders
# the tree view
 
# evidently, in Gtk the first click just enables ordering
tvFrame.tchClick(tvFrame.column0)
sleep(config.SHORT_DELAY)
# the treeview should still be ascending at this point
tvFrame.assertAscending()
 
# next time we click the sorting should change to descending
tvFrame.tchClick(tvFrame.column0)
sleep(config.SHORT_DELAY)
tvFrame.assertDescending()
 
# and now the sorting should go back to ascending
tvFrame.tchClick(tvFrame.column0)
sleep(config.SHORT_DELAY)
tvFrame.assertAscending()
 
print "INFO:  Log written to: %s" % config.OUTPUT_DIR
 
# close the app using Strongwind's altF4 method (from accessibles.py).
# this is the standard way of closing an application that doesn't have a
# clickable "quit" option.
tvFrame.altF4(tvFrame)
