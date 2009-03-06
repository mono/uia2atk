#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        01/13/2009
# Description: Test accessibility of printpreviewcontrol widget 
#              Use the printpreviewcontrolframe.py wrapper script
#              Test the samples/printpreviewcontrol.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of printpreviewcontrol widget
"""

# imports
import sys
import os

from strongwind import *
from printpreviewcontrol import *
from helpers import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the printpreviewcontrol sample application
try:
  app = launchPrintPreviewControl(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
ppcFrame = app.printPreviewControlFrame

#click button to show PrintPreviewControl page
ppcFrame.click(ppcFrame.button)
sleep(config.MEDIUM_DELAY)

#in this example panel should have "focusable" state that is different from #Panel control due to IsKeyboardFocusable is True
statesCheck(ppcFrame.panel, "Panel", add_states=["focusable"])
sleep(config.SHORT_DELAY)

#panel role may rise "focused" state by press Tab
ppcFrame.keyCombo("Tab", grabFocus=True)
sleep(config.SHORT_DELAY)
statesCheck(ppcFrame.panel, "Panel", add_states=["focusable", "focused"])

#move foused from panel to button by press Tab again
ppcFrame.keyCombo("Tab", grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(ppcFrame.panel, "Panel", add_states=["focusable"])

statesCheck(ppcFrame.button, "Button", add_states=["focused"])

#check if AtkValue is implemented for scrollbar
ppcFrame.valueScrollBar(ppcFrame.vscrollbar, 50)

ppcFrame.valueScrollBar(ppcFrame.hscrollbar, 50)

#close application frame window
ppcFrame.quit()


print "INFO:  Log written to: %s" % config.OUTPUT_DIR
