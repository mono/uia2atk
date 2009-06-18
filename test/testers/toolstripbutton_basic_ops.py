#!/usr/bin/env python

##############################################################################
# Written by:  Andres G. Aragoneses <aaragoneses@novell.com>
# Date:        02/23/2009
# Description: Test accessibility of toolstripbutton widget 
#              Use the toolstripbuttonframe.py wrapper script
#              Test the samples/toolstripdropdown_toolstripbutton.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of toolstripbutton widget
"""

# imports
import sys
import os

from strongwind import *
from toolstripbutton import *
from helpers import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the toolstripbutton sample application
try:
  app = launchToolStrip(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

sleep(config.SHORT_DELAY)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
tbbFrame = app.toolStripButtonFrame

###########################
##Accessible test
###########################

#check toolstrip's states
statesCheck(tbbFrame.toolbar, "ToolBar")

#bug 478838: missing focusable state
#statesCheck(tbbFrame.pushbutton_style, "Button")
#statesCheck(tbbFrame.toggle_style, "Button")

sleep(config.SHORT_DELAY)

#####################################
##Text, Action, Component test
#####################################

#test PushButton style
tbbFrame.PushButtonStyle(tbbFrame.pushbutton_style)

#test ToggleButton stylei
#BUG514300 ToolBarButton: the "new" toggle button is identified as check box
#tbbFrame.ToggleStyle(tbbFrame.toggle_style)


############################
##Image test
############################

#test Image size for all toolbar buttons
#NOTE: for this to work you need to patch strongwind with the diff in BGO#72995
tbbFrame.assertImageSize(tbbFrame.pushbutton_style, width=16, height=16)

#BUG514300 ToolBarButton: the "new" toggle button is identified as check box
#tbbFrame.assertImageSize(tbbFrame.toggle_style, width=0, height=0)

#close main window
tbbFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
