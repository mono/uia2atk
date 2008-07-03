#!/usr/bin/env python
# -*- coding: utf-8 -*-

##############################################################################
# Written by:  Brian G. Merrell <bgmerrell@novell.com>
# Date:        May 23 2008
# Description: Test accessibility of gtk checkbutton widget 
#              Use the gtkcheckbuttonframe.py wrapper script
#              Test the samples/gtkcheckbutton.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of checkbutton widget
"""

# imports
from strongwind import *
from gtkcheckbutton import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the checkbutton sample application
try:
  app = launchCheckButton(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
cbFrame = app.checkButtonFrame

# find a shorter way instead of app.checkbuttonFrame.checkbox1
cbFrame.checkbox1.click()
# need a short delay when checking and unchecking the check boxes
sleep(config.SHORT_DELAY)
cbFrame.assertResult(cbFrame.checkbox1,"checked");

cbFrame.checkbox2.click()
sleep(config.SHORT_DELAY)
cbFrame.assertResult(cbFrame.checkbox2, "checked");

cbFrame.checkbox2.click()
sleep(config.SHORT_DELAY)
cbFrame.assertResult(cbFrame.checkbox2, "unchecked");

cbFrame.checkbox1.click()
sleep(config.SHORT_DELAY)
cbFrame.assertResult(cbFrame.checkbox1, "unchecked");

cbFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
