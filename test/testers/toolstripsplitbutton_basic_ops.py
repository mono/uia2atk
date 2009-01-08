#!/usr/bin/env python
# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        01/08/2008
# Description: main test script of toolstripsplitbutton
#              ../samples/toolstripsplitbutton.py is the test sample script
#              toolstripsplitbutton/* are the wrappers of toolstripsplitbutton test sample 
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of ToolStripSplitButton widget
"""

# imports
from toolstripsplitbutton import *
from helpers import *
from actions import *
from states import *
from sys import argv

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the toolstripsplitbutton sample application
try:
  app = launchToolStripSplitButton(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
tssbFrame = app.toolStripSplitButtonFrame
tssbFrame.press(tssbFrame.toolstripsplitbutton)

##############################
# check toolstripsplitbutton children's AtkAction
##############################
actionsCheck(tssbFrame.push_button, "PushButton")
actionsCheck(tssbFrame.toggle_button, "ToggleButton")
actionsCheck(tssbFrame.menuitem_10, "MenuItem")
actionsCheck(tssbFrame.menuitem_12, "MenuItem")
actionsCheck(tssbFrame.menuitem_14, "MenuItem")

##############################
# check toolstripsplitbutton's AtkAccessible
##############################
statesCheck(tssbFrame.push_button, "PushButton")
statesCheck(tssbFrame.toggle_button, "ToggleButton")
statesCheck(tssbFrame.menuitem_10, "MenuItem")
statesCheck(tssbFrame.menuitem_12, "MenuItem")
statesCheck(tssbFrame.menuitem_14, "MenuItem")
sleep(config.SHORT_DELAY)

# select item from splitbutton
tssbFrame.menuitem_12.click
sleep(config.SHORT_DELAY)
statesCheck(tssbFrame.menuitem_12, "MenuItem", add_states=["selected"])
statesCheck(tssbFrame.menuitem_10, "MenuItem")

# select the last item from splitbutton
tssbFrame.menuitem_14.click
sleep(config.SHORT_DELAY)
statesCheck(tssbFrame.menuitem_14, "MenuItem", add_states=["selected"])
statesCheck(tssbFrame.menuitem_12, "MenuItem")

##############################
# check toolstripsplitbutton's AtkSelection
##############################
tssbFrame.assertSelectionChild(tssbFrame.menuitem_10, 0)
sleep(config.SHORT_DELAY)
tssbFrame.assertText(tssbFrame.label, "The font current size is 10")

# select the last item from combobox
tssbFrame.assertSelectionChild(tssbFrame.menuitem_14, 0)
sleep(config.SHORT_DELAY)
tssbFrame.assertText(tssbFrame.label, "The font current size is 14")

##############################
# check toolstripsplitbutton's AtkImage
##############################
tssbFrame.assertImage(tssbFrame.push_button, 48, 48)

##############################
# End
##############################
# close application frame window
tssbFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
