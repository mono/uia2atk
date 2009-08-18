#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        02/20/2009
# Description: Test accessibility of fontdialog widget 
#              Use the fontdialogframe.py wrapper script
#              Test the samples/winforms/fontdialog.py script
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

# click "Click me" button to show fontdialog page
fdFrame.fontdialog_button.click(log=True)
# give enough time for the font dialog to load entirely
sleep(config.MEDIUM_DELAY)
# find all of the new accessibles from the font dialog
fdFrame.findAllFontDialogAccessibles()

fdFrame.checkDefaultFontDialogStates()

# ensure that the color combo box texsts are what we expect
fdFrame.colorComboBoxNameAndTextTest()

# test the click action of MenuItems under color combo box 
fdFrame.colorComboBoxMenuItemActionTest()

# press down, which should move the keyboard focus to "Bold" under "Font Style"
fdFrame.keyCombo("Down", grabFocus=False)
# make sure that the "Bold" table cell gets the "selected" state and that the
# "Regular" table cell loses the selected states
statesCheck(fdFrame.fontstyle_table_cells[1],
            "TableCell",
            add_states=["selected"])
statesCheck(fdFrame.fontstyle_table_cells[0], "TableCell")

# close font dialog
fdFrame.fontdialog.ok()
sleep(config.SHORT_DELAY)

# close application frame window
fdFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
