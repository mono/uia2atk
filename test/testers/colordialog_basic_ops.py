#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        02/23/2009
# Description: Test accessibility of colordialog widget 
#              Use the colordialogframe.py wrapper script
#              Test the samples/colordialog.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of colordialog widget 

"""

# imports
import sys
import os

from strongwind import *
from colordialog import *
from helpers import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the colordialog sample application
try:
  app = launchColorDialog(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
cdFrame = app.colorDialogFrame

#####################################################
# search for all widgets from color dialog
#####################################################
# click button to show colordialog page, then check subwidgets
cdFrame.openColorDialog()
cdFrame.findAllColorDialogAccessibles()

# check the states of the default Color dialog
cdFrame.checkColorDialogStates(False)

# mouse click should raise focused state
cdFrame.small_color_buttons[-3].mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(cdFrame.small_color_buttons[-3], "Button", add_states=["focused"])

# keyTab move focused to the next SmallColorControl
cdFrame.keyCombo("Tab", grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(cdFrame.small_color_buttons[-4], "Button", add_states=["focused"])
statesCheck(cdFrame.small_color_buttons[-3], "Button")

# perform click action should raise focused state
# BUG504593 Button does not receive focus when click action is performed on it
#cdFrame.small_color_buttons[-5].click(log=True)
#sleep(config.SHORT_DELAY)
#statesCheck(cdFrame.small_color_buttons[-5], "Button", add_states=["focused"])
#statesCheck(cdFrame.small_color_buttons[-4], "Button")

# click the "Define Custom Colors" button
cdFrame.def_custom_colors_button.click(log=True)
sleep(config.SHORT_DELAY)

# now check the states of the Color dialog again, this time the custom
# color accessibles should be visible and showing
cdFrame.checkColorDialogStates(True)

# assert that SmallColorControls' Text are blank (i.e., "")
cdFrame.assertSmallColorText()

####################################################################
# Name test for SmallColorControls under Base Colors and User Colors
####################################################################
# Depending on how BUG488998 is resolved, we may or may not want to
# add tests to check the names of the SmallColorControl buttons

################################################
# AtkImage test for SmallColorControls
################################################
cdFrame.assertImageSize(cdFrame.small_color_buttons[10])
cdFrame.assertImageSize(cdFrame.small_color_buttons[40])

################################################
# AtkComponent test for SmallColorControls
################################################
cdFrame.assertComponentSize(cdFrame.small_color_buttons[10], 25, 23)
cdFrame.assertComponentSize(cdFrame.small_color_buttons[40], 25, 23)

###########################################################################
# AtkAction test for SmallColorControls to select color, Label shows which 
# color is selected
###########################################################################

# close the Color dialog
cdFrame.color_dialog.altF4()

# click ColorDialog Button to open color dialog again
cdFrame.color_dialog_button.click(log=True)
sleep(config.SHORT_DELAY)
cdFrame.findAllColorDialogAccessibles()
cdFrame.checkColorDialogStates(False)

# close dialog
cdFrame.cancel_button.click(log=True)
sleep(config.SHORT_DELAY)

# BUG504655 at-spi 'click' action cannot be used to select a color
#cdFrame.selectColorAndAssertLabelChange(-9, "Color [Yellow]")
#cdFrame.selectColorAndAssertLabelChange(-10, "Color [Red]")

# close application frame window
cdFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
