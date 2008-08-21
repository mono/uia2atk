#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        08/21/2008
# Description: Test accessibility of checkedlistbox widget 
#              Use the checkedlistboxframe.py wrapper script
#              Test the samples/checkedlistbox.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of checkedlistbox widget
"""

# imports
import sys
import os

from strongwind import *
from checkedlistbox import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the treeview sample application
try:
  app = launchCheckedListBox(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
clbFrame = app.checkedListBoxFrame

#check CheckedListBox's actions list
clbFrame.actionsCheck(clbFrame.listitem0)

#check CheckedListBox's states list
clbFrame.statesCheck_box(clbFrame.listbox)

#check listitem's states list
clbFrame.statesCheck_item(clbFrame.listitem0)

#click listitem to change the label
clbFrame.click('0')
sleep(config.SHORT_DELAY)
clbFrame.assertItemSelected('0')

clbFrame.click('5')
sleep(config.SHORT_DELAY)
clbFrame.assertItemSelected('0 5')

clbFrame.click('8')
sleep(config.SHORT_DELAY)
clbFrame.assertItemSelected('0 5 8')

#click listitem 1 to rise checked state
clbFrame.click('1')
sleep(config.SHORT_DELAY)
clbFrame.assertChecked('1')

#click listitem 1 again checked state disappear
clbFrame.click('1')
sleep(config.SHORT_DELAY)
clbFrame.assertUnchecked('1')

#close application frame window
clbFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
