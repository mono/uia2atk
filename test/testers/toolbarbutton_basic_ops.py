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
# States test
###########################
# BUG506765 ToolBarButton: DropDown style toolbarbutton with wrong role name
# check toolstrip's states
statesCheck(tbbFrame.toolbar, "ToolBar")

statesCheck(tbbFrame.pushbutton_style, "Button")

statesCheck(tbbFrame.dropdown_button, "Button")

statesCheck(tbbFrame.dropdown_toggle, "ToggleButton")

statesCheck(tbbFrame.toggle_style, "ToggleButton")

# nop toolbarbutton is enabled and sensitive by default
statesCheck(tbbFrame.nop_unable, "Button")
# click toggle_style button to set "Enabled = False" for nop_unable button
tbbFrame.toggle_style.click(log=True)
sleep(config.SHORT_DELAY)
# nop_unable button get rid of enabled and sensitive states
statesCheck(tbbFrame.nop_unable, "Button", \
                                        invalid_states=["enabled", "sensitive"])
# click toggle_style button again to set "Enabled = True" for nop_unable button
tbbFrame.toggle_style.click(log=True)
sleep(config.SHORT_DELAY)
# nop_unable button is enalbed and sensitive
statesCheck(tbbFrame.nop_unable, "Button")

statesCheck(tbbFrame.separator_style, "ToolStripSeparator")

#######################################
# AtkText, AtkAction, AtkComponent test
#######################################

# test PushButton style
tbbFrame.TestBasicPushButtonStyle(tbbFrame.pushbutton_style)

# test DropDownButton style
# BUG490105: dropdown_toggle has wrong postion
# BUG498724: missing AtkText implemented

# BUG514667: DropDown button's 'click'action can't be implemented
#tbbFrame.TestBasicDropDownButtonStyle()

# test Toggle style
tbbFrame.TestBasicToggleStyle(tbbFrame.toggle_style)

# test Insensitive ToolBarButton
tbbFrame.TestBasicInsensitiveButton(tbbFrame.nop_unable)

# test Separator style
tbbFrame.TestBasicSeparatorStyle(tbbFrame.separator_style)

############################
# Image test
############################

# test Image size for all toolbar buttons
tbbFrame.assertImageSize(tbbFrame.pushbutton_style, \
                                          expected_width=24, expected_height=24)

tbbFrame.assertImageSize(tbbFrame.dropdown_button, \
                                          expected_width=24, expected_height=24)

tbbFrame.assertImageSize(tbbFrame.toggle_style, \
                                          expected_width=24, expected_height=24)

tbbFrame.assertImageSize(tbbFrame.nop_unable, \
                                          expected_width=24, expected_height=24)

tbbFrame.assertImageSize(tbbFrame.separator_style)

############################
# assert name
############################
# BUG506765 - ToolBarButton: DropDown style toolbarbutton with wrong role name
#tbbFrame.assertName(tbbFrame.filler, " ")
tbbFrame.assertName(tbbFrame.pushbutton_style, "PushButton")
tbbFrame.assertName(tbbFrame.dropdown_button, "DropDownButton")
tbbFrame.assertName(tbbFrame.dropdown_toggle, "DropDownButton")
tbbFrame.assertName(tbbFrame.toggle_style, "Toggle")
tbbFrame.assertName(tbbFrame.nop_unable, "nop")
tbbFrame.assertName(tbbFrame.separator_style, "separator")
tbbFrame.assertName(tbbFrame.label, "You clicked PushButton 2 times")

# close main window
tbbFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
