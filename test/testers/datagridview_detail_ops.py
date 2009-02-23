#!/usr/bin/env python

##
# Written by:  Mario Carrion <mcarrion@novell.com>
# Date:        02/23/2009
# Description: Test accessibility of DataGridView widget 
#              Use the datagridviewframe.py wrapper script
#              Test the samples/datagridview.py script
##

# The docstring below  is used in the generated log file
"""
Test accessibility of DataGridView widget
"""

# imports
import sys
import os

from strongwind import *
from datagridview import *
from helpers import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the datagridview sample application
try:
  app = launchDataGridView(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
dtgvFrame = app.dataGridViewFrame

#check DataGridView TableColumnHeader's actions list
actionsCheck(dtgvFrame.checkbox_column, "TableColumnHeader")
actionsCheck(dtgvFrame.textbox_column, "TableColumnHeader")

#close application frame window
dtgvFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
