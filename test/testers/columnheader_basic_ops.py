#!/usr/bin/env python

##########################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        01/04/2009
# Description: Test accessibility of columnheader widget 
#              Use the columnheaderframe.py wrapper script
#              Test the samples/columnheader.py script
##########################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of columnheader widget
"""

# imports
import sys
import os

from strongwind import *
from columnheader import *
from helpers import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the columnheader sample application
try:
  app = launchColumnHeader(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
chFrame = app.columnHeaderFrame

# check TableColumnHeader's actions list
actionsCheck(chFrame.column_a, "TableColumnHeader")
actionsCheck(chFrame.column_b, "TableColumnHeader")

# check TableColumnHeader's states list
statesCheck(chFrame.column_a, "TableColumnHeader")
statesCheck(chFrame.column_b, "TableColumnHeader")

# check text implementation
chFrame.assertText(chFrame.column_a, "Column A")

chFrame.assertText(chFrame.column_b, "Num")


# check item's order after click column header
##click action doesn't work BUG476304
#chFrame.clickColumnHeaderToSortOrder(chFrame.column_a, "click", firstitem="Item5")
#statesCheck(chFrame.column_a, "TableColumnHeader")

#chFrame.clickColumnHeaderToSortOrder(chFrame.column_b, "click", firstitem="Item0")
#statesCheck(chFrame.column_b, "TableColumnHeader")

chFrame.clickColumnHeaderToSortOrder(chFrame.column_a, "mouseClick", firstitem="Item5")
statesCheck(chFrame.column_a, "TableColumnHeader")
statesCheck(chFrame.column_b, "TableColumnHeader")

chFrame.clickColumnHeaderToSortOrder(chFrame.column_b, "mouseClick", firstitem="Item0")
statesCheck(chFrame.column_a, "TableColumnHeader")
statesCheck(chFrame.column_b, "TableColumnHeader")

# check ColumnHeader AtkImage implementation
##incorrect imageSize BUG477563
#chFrame.assertImageSize(chFrame.column_a)

#chFrame.assertImageSize(chFrame.column_b)

# close application frame window
chFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
