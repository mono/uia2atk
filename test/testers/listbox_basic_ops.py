#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        08/18/2008
# Description: Test accessibility of listbox widget 
#              Use the listboxframe.py wrapper script
#              Test the samples/listbox.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of listbox widget
"""

# imports
import sys
import os

from strongwind import *
from listbox import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the treeview sample application
try:
  app = launchListBox(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
lbFrame = app.listBoxFrame

#check ListBox's actions list
lbFrame.actionsCheck(lbFrame.listitem0)

#check ListBox's states list
lbFrame.statesCheck_box(lbFrame.listbox)

#check ListItem's states list
lbFrame.statesCheck_item(lbFrame.listitem0)

#click ListItem 
lbFrame.click('0')
sleep(config.SHORT_DELAY)
lbFrame.assertItemSelected('0')

lbFrame.click('5')
sleep(config.SHORT_DELAY)
lbFrame.assertItemSelected('5')

lbFrame.click('8')
sleep(config.SHORT_DELAY)
lbFrame.assertItemSelected('8')

#close application frame window
lbFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
