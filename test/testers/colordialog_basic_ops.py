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
##search for all widgets from color dialog
#####################################################

#click button to show colordialog page, then check subwidgets
cdFrame.click(cdFrame.colordialog_button)
sleep(config.SHORT_DELAY)
cdFrame.AssertWidgets()

#####################################################################
##States test for dialog, SmallColorControls and Panels 
#####################################################################

statesCheck(cdFrame.colordialog, "Dialog", add_states=["active", "modal"], \
                                           invalid_states=["resizable"])
##color panel has extra focusable state BUG484217
#statesCheck(cdFrame.panels[0], "Panel")

#statesCheck(cdFrame.panels[1], "Panel")

statesCheck(cdFrame.smallcolor_buttons[0], "Button")

statesCheck(cdFrame.smallcolor_buttons[63], "Button")

#mouse click may raise focused state
cdFrame.smallcolor_buttons[53].mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(cdFrame.smallcolor_buttons[53], "Button", add_states=["focused"])

#keyTab move focused to the next SmallColorControl
cdFrame.keyCombo("Tab", grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(cdFrame.smallcolor_buttons[52], "Button", add_states=["focused"])
statesCheck(cdFrame.smallcolor_buttons[53], "Button")

#AtkAction doesn't raise focused state
cdFrame.click(cdFrame.smallcolor_buttons[50])
sleep(config.SHORT_DELAY)
statesCheck(cdFrame.smallcolor_buttons[50], "Button")

####################################################################
##Text test for SmallColorControls under Base Colors and User Colors
####################################################################

#SmallColorControls' Text are None
cdFrame.assertSmallColorText(cdFrame.smallcolor_buttons[5])

cdFrame.assertSmallColorText(cdFrame.smallcolor_buttons[55])


####################################################################
##Name test for SmallColorControls under Base Colors and User Colors
####################################################################

##SmallColorControls are missing names for orca to announce due to BUG488998
#cdFrame.assertSmallColorName()

#cdFrame.assertSmallColorName(cdFrame.smallcolor_buttons[0], "White")

################################################
##AtkImage test for SmallColorControls
################################################

cdFrame.assertImageSize(cdFrame.smallcolor_buttons[10])

cdFrame.assertImageSize(cdFrame.smallcolor_buttons[40])

###########################################################################
##AtkAction test for SmallColorControls to change main label
###########################################################################

#do click action for SmallColorControl, then click OK button
#do click action may multi select SmallColorControl BUG478541
cdFrame.click(cdFrame.smallcolor_buttons[0])
sleep(config.SHORT_DELAY)
cdFrame.click(cdFrame.ok_button)
sleep(config.SHORT_DELAY)
#cdFrame.assertClickSmallColor("Color [White]")

#click ColorDialog Button to open color dialog again
cdFrame.click(cdFrame.colordialog_button)
sleep(config.SHORT_DELAY)
cdFrame.AssertWidgets()

#mouse click one SmallColorControl, then click OK button
cdFrame.smallcolor_buttons[24].mouseClick()
sleep(config.SHORT_DELAY)
#cdFrame.click(cdFrame.ok_button)
cdFrame.ok_button.click()
sleep(config.SHORT_DELAY)
cdFrame.assertClickSmallColor("Color [Olive]")

#close application frame window
cdFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
