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


#click column 0
cbFrame.findTableColumnHeader("Column 0").click()
sleep(config.MEDIUM_DELAY)
cbFrame.tchClick("Column 0")
cbFrame.assertResult(cbFrame.column0, "checked")

#click column 0 again
cbFrame.tchClick("Column 0")
sleep(config.MEDIUM_DELAY)
cbFrame.checkRoleName(cbFrame.column0,"table column header")

#expand parent 0
cbFrame.expand(cbFrame.parent0,'expand or contract')
#sleep(config.MEDIUM_DELAY)
cbFrame.actionResult(cbFrame.findTableCell('child 0 of parent 0'),'child 0 of parent 0')

#expand parent 1
cbFrame.expand(cbFrame.parent1,'expand or contract')
#sleep(config.MEDIUM_DELAY)
cbFrame.actionResult(cbFrame.findTableCell('child 0 of parent 1'),'child 0 of parent 1')


#expand parent 2
cbFrame.expand(cbFrame.parent2,'expand or contract')
#sleep(config.MEDIUM_DELAY)
cbFrame.actionResult(cbFrame.findTableCell('child 0 of parent 2'),'child 0 of parent 2')


#expand parent 3
cbFrame.expand(cbFrame.parent3,'expand or contract')
#sleep(config.MEDIUM_DELAY)
cbFrame.actionResult(cbFrame.findTableCell('child 0 of parent 3'),'child 0 of parent 3')


#contract parent 2
cbFrame.contract(cbFrame.parent2,'expand or contract')
#sleep(config.LONG_DELAY)
cbFrame.actionResult(None, 'child 0 of parent 2',assertAction=False)


#expand parent 2
cbFrame.expand(cbFrame.parent2,'expand or contract')
#sleep(config.MEDIUM_DELAY)
cbFrame.focusResult('child 0 of parent 2')


#contract parent 3
cbFrame.contract(cbFrame.parent3,'expand or contract')
#sleep(config.LONG_DELAY)
cbFrame.focusResult('child 0 of parent 3', result=False)


#sleep(config.MEDIUM_DELAY)
print "INFO:  Log written to: %s" % config.OUTPUT_DIR
cbFrame.keyCombo('<Alt>F4')

