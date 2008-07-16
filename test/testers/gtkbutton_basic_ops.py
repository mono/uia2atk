#!/usr/bin/env python

##############################################################################
# Written by:  Calen Chen <cachen@novell.com>
# Date:        06/27/2008
# Description: Test accessibility of gtk button widget 
#              Use the gtkbuttonframe.py wrapper script
#              Test the samples/gtkbutton.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of button widget
"""

# imports
from strongwind import *
from gtkbutton import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the button sample application
try:
  app = launchButton(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
bFrame = app.gtkButtonFrame

#click button1 to get a messagedialog, then close messagedialog
bFrame.click(bFrame.button1)
sleep(config.SHORT_DELAY)
bFrame.assertClicked()

##give button2 a press action, then to check if rise armed status
bFrame.press(bFrame.button2)
sleep(config.SHORT_DELAY)
bFrame.assertArmed(bFrame.button2);

#give button2 a relese action, then to check if rase armed status
bFrame.release(bFrame.button2)
sleep(config.SHORT_DELAY)
bFrame.assertUnarmed(bFrame.button2);

#check the rising messagedialog then close it
bFrame.assertClicked()

#close the sample app
bFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
