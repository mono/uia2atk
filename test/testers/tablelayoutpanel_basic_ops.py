#!/usr/bin/env python
# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        02/04/2009
# Description: main test script of tablelayoutpanel
#              ../samples/tablelayoutpanel.py is the test sample script
#              tablelayoutpanel/* is the wrapper of tablelayoutpanel test sample script
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of TableLayoutPanel widget
"""

# imports
from tablelayoutpanel import *
from helpers import *
from states import *
from sys import argv

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the tablelayoutpanel sample application
try:
  app = launchTableLayoutPanel(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

sleep(config.SHORT_DELAY)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
tlpFrame = app.tableLayoutPanelFrame

##############################
# check TableLayoutPanel and its children's AtkAccessible
##############################
statesCheck(tlpFrame.panel, "Panel")
statesCheck(tlpFrame.labels[0], "Label")
statesCheck(tlpFrame.buttons[0], "Button", add_states=["focused"])
statesCheck(tlpFrame.buttons[1], "Button")

statesCheck(tlpFrame.labels[1], "Label")
statesCheck(tlpFrame.buttons[1], "Button")
statesCheck(tlpFrame.buttons[0], "Button", add_states=["focused"])

tlpFrame.keyCombo("Tab", grabFocus=False)
sleep(config.SHORT_DELAY)
tlpFrame.keyCombo("Tab", grabFocus=False)
statesCheck(tlpFrame.labels[2], "Label")
statesCheck(tlpFrame.buttons[2], "Button", add_states=["focused"])
statesCheck(tlpFrame.buttons[1], "Button")

statesCheck(tlpFrame.labels[3], "Label")
statesCheck(tlpFrame.buttons[3], "Button")

# press Tab key again to move focused state from button
tlpFrame.click(tlpFrame.buttons[0])
sleep(config.SHORT_DELAY)
tlpFrame.assertText(tlpFrame.labels[0], "I am in cell1")

tlpFrame.click(tlpFrame.buttons[1])
sleep(config.SHORT_DELAY)
tlpFrame.assertText(tlpFrame.labels[1], "I am in cell2")

tlpFrame.click(tlpFrame.buttons[2])
sleep(config.SHORT_DELAY)
tlpFrame.assertText(tlpFrame.labels[2], "I am in cell3")

tlpFrame.click(tlpFrame.buttons[3])
sleep(config.SHORT_DELAY)
tlpFrame.assertText(tlpFrame.labels[3], "I am in cell4")

##############################
# End
##############################
tlpFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
