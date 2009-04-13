#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        02/20/2009
# Description: Test accessibility of fontdialog widget 
#              Use the fontdialogframe.py wrapper script
#              Test the samples/fontdialog.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of fontdialog widget 

"""

# imports
import sys
import os

from strongwind import *
from fontdialog import *
from helpers import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the fontdialog sample application
try:
  app = launchFontDialog(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
fdFrame = app.fontDialogFrame

###################################################
# search for all widgets from font dialog
###################################################

#click "Click me" button to show fontdialog page, then check subwidgets
fdFrame.fontdialog_button.click()
sleep(config.MEDIUM_DELAY)
fdFrame.AssertWidgets()

################################################################################
# search for color ComboBox and its sub children in Effects GroupBox;
# check States;
# check AtkText for MenuItems;
# color ComboBox Press action test;
# color MenuItem Click action test
################################################################################
## BUG478526 that MenuItems have wrong texts, remember to return test case in 
## frame wraper line123-125
fdFrame.colorComboBoxTest()

# close font dialog
fdFrame.click(fdFrame.ok_button)

# close application frame window
fdFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
