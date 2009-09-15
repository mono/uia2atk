#!/usr/bin/env python
# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        02/02/2009
# Description: main test script of flowlayoutpanel
#              ../samples/winforms/flowlayoutpanel.py is the test sample script
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
statesCheck(flpFrame.panel1, "Panel")
statesCheck(flpFrame.label1, "Label")
statesCheck(flpFrame.button1, "Button", add_states=["focused"])

statesCheck(flpFrame.button2, "Button")
statesCheck(flpFrame.panel2, "Panel")
statesCheck(flpFrame.label2, "Label")

statesCheck(flpFrame.panel3, "Panel")
statesCheck(flpFrame.label3, "Label")
statesCheck(flpFrame.button3, "Button")

statesCheck(flpFrame.panel4, "Panel")
statesCheck(flpFrame.label4, "Label")
statesCheck(flpFrame.button4, "Button")

flpFrame.keyCombo("Tab", grabFocus=False)
sleep(config.SHORT_DELAY)
flpFrame.keyCombo("Tab", grabFocus=False)
sleep(config.SHORT_DELAY)

statesCheck(flpFrame.panel1, "Panel")
statesCheck(flpFrame.label1, "Label")
statesCheck(flpFrame.button1, "Button")

statesCheck(flpFrame.button2, "Button")

statesCheck(flpFrame.panel3, "Panel")
statesCheck(flpFrame.label3, "Label")
statesCheck(flpFrame.button3, "Button", add_states=["focused"])

statesCheck(flpFrame.panel4, "Panel")
statesCheck(flpFrame.label4, "Label")
statesCheck(flpFrame.button4, "Button")

# Perform click action on button1
flpFrame.button1.click(log=True)
sleep(config.SHORT_DELAY)
flpFrame.assertText(flpFrame.label1, "TopDown")
# Bug 504593 Button does not receive focus when click action is performed on it
#statesCheck(flpFrame.button1, "Button", add_states=["focused"])
#statesCheck(flpFrame.button3, "Button")

flpFrame.button2.click(log=True)
sleep(config.SHORT_DELAY)
flpFrame.assertText(flpFrame.label2, "BottomUp")
# Bug 504593 Button does not receive focus when click action is performed on it
#statesCheck(flpFrame.button2, "Button", add_states=["focused"])
#statesCheck(flpFrame.button1, "Button")

flpFrame.button3.click(log=True)
sleep(config.SHORT_DELAY)
flpFrame.assertText(flpFrame.label3, "LeftToRight")

flpFrame.button4.click(log=True)
sleep(config.SHORT_DELAY)
flpFrame.assertText(flpFrame.label4, "RightToLeft")

##############################
# End
##############################
flpFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
