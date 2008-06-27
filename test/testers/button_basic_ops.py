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

print "\"app\":", app

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
cbFrame = app.buttonFrame

#enter "aaa" to TextBox, then check it
cbFrame.findText(None).enterText("aaa")
sleep(config.SHORT_DELAY)
cbFrame.textResult("aaa")

#click button1 to get a messagedialog, then colse messagedialog
cbFrame.button1.click()
sleep(config.SHORT_DELAY)
cbFrame.clickResult()

#close the messagedialog
cbFrame.close()

#click button2 to get a messagedialog, then close messagedialog
cbFrame.button2.click()
sleep(config.SHORT_DELAY)
cbFrame.clickResult()

#close the messagedialog
cbFrame.close()

#give button1 a press action, then to check if rise armed status
cbFrame.press(cbFrame.button1)
sleep(config.SHORT_DELAY)
cbFrame.assertResult(cbFrame.button1,"armed");

#give button1 a relese action, then to check if rase armed status
cbFrame.release(cbFrame.button1)
sleep(config.SHORT_DELAY)
cbFrame.assertResult(cbFrame.button1, "unarmed");
cbFrame.clickResult()

#close the messagedialog
cbFrame.close()

##give button2 a press action, then to check if rise armed status
cbFrame.press(cbFrame.button2)
sleep(config.SHORT_DELAY)
cbFrame.assertResult(cbFrame.button2, "armed");

#give button2 a relese action, then to check if rase armed status
cbFrame.release(cbFrame.button2)
sleep(config.SHORT_DELAY)
cbFrame.assertResult(cbFrame.button2, "unarmed");
cbFrame.clickResult()

#close the messagedialog
cbFrame.close()

cbFrame.altF4(cbFrame)
print "INFO:  Log written to: %s" % config.OUTPUT_DIR
