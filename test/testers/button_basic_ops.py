#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        07/22/2008
# Description: Test accessibility of button widget 
#              Use the buttonframe.py wrapper script
#              Test the samples/button_label_linklabel.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of button widget
"""

# imports
import sys
import os

from strongwind import *
from button import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the treeview sample application
try:
  app = launchButton(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
bFrame = app.buttonFrame

#check Button's actions list
bFrame.actionsCheck(bFrame.button1)

#check Button's states list
bFrame.statesCheck(bFrame.button1)
bFrame.statesCheck(bFrame.button2)
bFrame.statesCheck(bFrame.button3)

#click button1 rise message frame window
bFrame.click(bFrame.button1)
sleep(config.SHORT_DELAY)
bFrame.assertMessage()

#click button2 change label text
bFrame.click(bFrame.button2)
sleep(config.SHORT_DELAY)
bFrame.assertLabel('You have clicked me 1 times')

#click button2 change label text
bFrame.click(bFrame.button2)
sleep(config.SHORT_DELAY)
bFrame.assertLabel('You have clicked me 2 times')

print "INFO:  Log written to: %s" % config.OUTPUT_DIR

#close application frame window
bFrame.quit()
