#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        02/09/2009
# Description: Test accessibility of toolbarbutton widget 
#              Use the toolbarbuttonframe.py wrapper script
#              Test the samples/toolbarbutton.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of toolbarbutton widget
"""

# imports
import sys
import os

from strongwind import *
from toolbarbutton import *
from helpers import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the toolbarbutton sample application
try:
  app = launchToolBar(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

sleep(config.SHORT_DELAY)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
tbbFrame = app.toolBarButtonFrame

###########################
##Accessible test
###########################

#check toolstrip's states
statesCheck(tbbFrame.toolbar, "ToolBar")

statesCheck(tbbFrame.pushbutton_style, "Button")

statesCheck(tbbFrame.dropdown_button, "Button")

##missing toggle button BUG473755
statesCheck(tbbFrame.dropdown_toggle, "ToggleButton")

##missing armed and checked states when this button is pushed BUG474649
statesCheck(tbbFrame.toggle_style, "Button")

#nop toolbarbutton is enabled and sensitive by default
statesCheck(tbbFrame.nop_unable, "Button")
#click toggle_style button to set "Enabled = False" for nop_unable button
tbbFrame.click(tbbFrame.toggle_style)
sleep(config.SHORT_DELAY)
#nop_unable button get rid of enabled and sensitive states
##extraneous states BUG474197
statesCheck(tbbFrame.nop_unable, "Button", invalid_states=["enabled", "sensitive"])
#click toggle_style button again to set "Enabled = True" for nop_unable button
tbbFrame.click(tbbFrame.toggle_style)
sleep(config.SHORT_DELAY)
#nop_unable button is enalbed and sensitive
statesCheck(tbbFrame.nop_unable, "Button")

statesCheck(tbbFrame.separator_style, "Separator")

#####################################
##Text, Action, Component test
#####################################

#test PushButton style
tbbFrame.PushButtonStyle(tbbFrame.pushbutton_style)

#test DropDownButton style
tbbFrame.DropDownButtonStyle(tbbFrame.dropdown_button, tbbFrame.toggle_button)

#test Toggle style
tbbFrame.ToggleStyle(tbbFrame.toggle_style)

#test Unable ToolBarButton
tbbFrame.UnableButton(tbbFrame.nop_unable)

#test Separator style
##Separator style toolbarbutton have wrong role name BUG473763
tbbFrame.SeparatorStyle(tbbFrame.separator_style)

############################
##Image test
############################

#test Image size for all toolbar buttons
tbbFrame.assertImageSize(tbbFrame.pushbutton_style, width=24, height=24)

tbbFrame.assertImageSize(tbbFrame.dropdown_button, width=24, height=24)

tbbFrame.assertImageSize(tbbFrame.toggle_style, width=24, height=24)

tbbFrame.assertImageSize(tbbFrame.nop_unable, width=24, height=24)

tbbFrame.assertImageSize(tbbFrame.separator_style, width=-1, height=-1)

#close main window
tbbFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
