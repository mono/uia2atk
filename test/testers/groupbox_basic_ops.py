#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        08/19/2008
# Description: Test accessibility of groupbox widget 
#              Use the groupboxframe.py wrapper script
#              Test the samples/groupbox.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of groupbox widget
"""

# imports
import sys
import os

from strongwind import *
from groupbox import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the treeview sample application
try:
  app = launchGroupBox(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
gbFrame = app.groupBoxFrame

#check Button's actions list
gbFrame.actionsCheck(gbFrame.button1)

#check Panel's states list
gbFrame.statesCheck()

#search for groupbox1 panel
gbFrame.searchGroupBox('GroupBox1')

#search for groupbox2 panel
gbFrame.searchGroupBox('GroupBox2')

#click button1 in groupbox1
gbFrame.click(gbFrame.button1)
sleep(config.SHORT_DELAY)
gbFrame.assertLabel('1')

#click button2 two times in groupbox2
gbFrame.click(gbFrame.button2)
gbFrame.click(gbFrame.button2)
sleep(config.SHORT_DELAY)
gbFrame.assertLabel('2')

#close application frame window
gbFrame.quit()


print "INFO:  Log written to: %s" % config.OUTPUT_DIR
