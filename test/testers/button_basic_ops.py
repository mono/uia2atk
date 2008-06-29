#!/usr/bin/env python
# -*- coding: utf-8 -*-

##############################################################################
# Written by:  Brian G. Merrell <bgmerrell@novell.com>
# Date:        May 23 2008
# Description: Test accessibility of checkbutton widget 
#              Use the checkbuttonframe.py wrapper script
#              Test the sample/checkButton.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of button widget
"""

# imports
from strongwind import *
from button import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the checkbutton sample application
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

#click button1 to get a messagedialog, then close messagedialog
bFrame.button1.click()
sleep(config.SHORT_DELAY)
bFrame.clickResult()

#close the messagedialog
bFrame.close_dialog()

##give button2 a press action, then to check if rise armed status
bFrame.press(bFrame.button2)
sleep(config.SHORT_DELAY)
bFrame.assertResult(bFrame.button2, "armed");

#give button2 a relese action, then to check if rase armed status
bFrame.release(bFrame.button2)
sleep(config.SHORT_DELAY)
bFrame.assertResult(bFrame.button2, "unarmed");
bFrame.clickResult()

#close the messagedialog
bFrame.close_dialog()

#close the sample app
bFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
