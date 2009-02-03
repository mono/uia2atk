#!/usr/bin/env python
# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        02/02/2009
# Description: main test script of flowlayoutpanel
#              ../samples/flowlayoutpanel.py is the test sample script
#              flowlayoutpanel/* is the wrapper of flowlayoutpanel test sample script
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of FlowLayoutPanel widget
"""

# imports
from flowlayoutpanel import *
from helpers import *
from states import *
from sys import argv

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the flowlayoutpanel sample application
try:
  app = launchFlowLayoutPanel(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

sleep(config.SHORT_DELAY)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
flpFrame = app.flowLayoutPanelFrame

##############################
# check FlowLayoutPanel and its children's AtkAccessible
##############################
statesCheck(flpFrame.panels[0], "Panel")
statesCheck(flpFrame.labels[0], "Label")
statesCheck(flpFrame.buttons[0], "Button", add_states=["focused"])
statesCheck(flpFrame.buttons[1], "Button")

statesCheck(flpFrame.panels[1], "Panel")
statesCheck(flpFrame.labels[1], "Label")
statesCheck(flpFrame.buttons[1], "Button")
statesCheck(flpFrame.buttons[0], "Button", add_states=["focused"])

flpFrame.keyCombo("Tab", grabFocus=False)
sleep(config.SHORT_DELAY)
flpFrame.keyCombo("Tab", grabFocus=False)
statesCheck(flpFrame.panels[2], "Panel")
statesCheck(flpFrame.labels[2], "Label")
statesCheck(flpFrame.buttons[2], "Button", add_states=["focused"])
statesCheck(flpFrame.buttons[1], "Button")

statesCheck(flpFrame.panels[3], "Panel")
statesCheck(flpFrame.labels[3], "Label")
statesCheck(flpFrame.buttons[3], "Button")

# press Tab key again to move focused state from button
flpFrame.click(flpFrame.buttons[0])
sleep(config.SHORT_DELAY)
flpFrame.assertText(flpFrame.labels[0], "TopDown")

flpFrame.click(flpFrame.buttons[1])
sleep(config.SHORT_DELAY)
flpFrame.assertText(flpFrame.labels[1], "BottomUp")

flpFrame.click(flpFrame.buttons[2])
sleep(config.SHORT_DELAY)
flpFrame.assertText(flpFrame.labels[2], "LeftToRight")

flpFrame.click(flpFrame.buttons[3])
sleep(config.SHORT_DELAY)
flpFrame.assertText(flpFrame.labels[3], "RightToLeft")

##############################
# End
##############################
flpFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
