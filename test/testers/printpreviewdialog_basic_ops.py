#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        01/19/2009
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

#click button to show PrintPreviewDialog page
ppdFrame.click(ppdFrame.button)

#check Dialog's states
statesCheck(ppdFrame.dialog, "Dialog", add_states=["modal"])

#check ToolBar's states
statesCheck(ppdFrame.toolbar, "ToolBar")

#in this example panel should have "focusable" state that is different from #Panel control due to IsKeyboardFocusable is True
##missing focusable BUG465945
statesCheck(ppdFrame.dialog_panel, "Panel", add_states=["focusable"])

#mouse click panel to rise focused state
##still missing focused after mouse click panel by pyatspi BUG473757
ppdFrame.dialog_panel.mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(ppdFrame.dialog_panel, "Panel", add_states=["focusable", "focused"])

#check how many items on toolbar
##with wrong items due to toolbar control's BUG428598
ppdFrame.searchItems("push button", 8)

ppdFrame.searchItems("toggle button", 1)

ppdFrame.searchItems("separator", 2)

ppdFrame.searchItems("spin button", 1)

#close dialog window
ppdFrame.dialog.altF4()

#close application frame window
ppdFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
