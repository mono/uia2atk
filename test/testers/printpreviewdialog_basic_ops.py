#!/usr/bin/env python

############################################################################## # Written by:  Cachen Chen <cachen@novell.com> # Date:        01/19/2009
# Description: Test accessibility of printpreviewdialog widget 
#              Use the printpreviewdialogframe.py wrapper script
#              Test the samples/printpreviewdialog.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of printpreviewdialog widget 

"""

# imports
import sys
import os

from strongwind import *
from printpreviewdialog import *
from helpers import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the printpreviewdialog sample application
try:
  app = launchPrintPreviewDialog(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
ppdFrame = app.printPreviewDialogFrame

# click button to show PrintPreviewDialog page, then give plenty of time for
# all of the accessibles to load
ppdFrame.button.click(log=True)
sleep(config.MEDIUM_DELAY)

# find all of the PrintPreviewDialog controls
ppdFrame.findAllPrintPreviewDialogAccessibles()

# check the default states of all of the accessibles
ppdFrame.checkAllDefaultStates()

# check the actions of accessibles with actions
ppdFrame.checkAllActions()

# need to do a lot of keyboard navigation related tests once the following bugs
# are resolved: BUG509142, BUG509152 	

# click all of the buttons
ppdFrame.clickAllDefaultButtons()
# find all of the accessibles now that the zoom drop down menu is dropped down
ppdFrame.findZoomDropDownAccessibles()
# now check the zoom drop down menu's states and actions
ppdFrame.checkZoomDropDownDefaultStates()
ppdFrame.checkZoomDropDownActions()

# TODO: is there a good way to test to make sure our clicking has
# had the expected affects on the GUI?

# TODO: add tests for the NumericUpDown control once BUG508567 is fixed.
# should at least test using the assignNumericUpDownValue,
# assertNumericUpDownValue, and changing the NumericUpDown value with keyCombo

# TODO: Test the "Close" button after BUG508556 is fixed

# close the zoom menu because of BUG509299 (Zoom menu stays open when Alt+F4 is
# pressed to close the PrintPreviewDialog).  We must use a mouseClick and not
# a click action here because of BUG509344.
ppdFrame.zoom_toggle_button.mouseClick()
sleep(config.SHORT_DELAY)

# close dialog window
ppdFrame.dialog.altF4()

# close application frame window
ppdFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
