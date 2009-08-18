#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        11/03/2008
# Description: Test accessibility of listview widget 
#              Use the listviewframe.py wrapper script
#              Test the samples/winforms/ListView_list.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of listview_list widget
"""

# imports
import sys
import os

from strongwind import *
from listview_list import *
from helpers import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the listview_list sample application
try:
  app = launchListView(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
lvFrame = app.listViewFrame

#click listitem to rise selected and focused states, listitem1 also with 
#selected states after click listitem3 because MultiSelect is True
lvFrame.click(lvFrame.listitem[1])
sleep(config.SHORT_DELAY)
a = lvFrame.listitem[1]._accessible.getState()
print "listitem 1 states:", a.getStates()

lvFrame.click(lvFrame.listitem[3])
b = lvFrame.listitem[3]._accessible.getState()
print "listitem 2 states:", b.getStates()

print "listitem 1 states again:", a.getStates()

#close application frame window
lvFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
