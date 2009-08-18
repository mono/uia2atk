#!/usr/bin/env python
# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        02/11/2009
# Description: main test script of threadexceptiondialog
#              ../samples/winforms/threadexceptiondialog.py is the test sample script
#              threadexceptiondialog/* is the wrapper of threadexceptiondialog 
#              test sample script
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of ThreadExceptionDialog widget
"""

# imports
from threadexceptiondialog import *
from helpers import *
from states import *
from sys import argv

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the threadexceptiondialog sample application
try:
  app = launchThreadExceptionDialog(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

sleep(config.SHORT_DELAY)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
tedFrame = app.threadExceptionDialogFrame
tedFrame.showDialog(tedFrame.raise_button)
sleep(config.SHORT_DELAY)

##############################
# check Frame and Dialog's AtkAccessible
##############################
statesCheck(tedFrame, "Form")
statesCheck(tedFrame.dialog, "Dialog", add_states=["active", "modal"], invalid_states=["resizable"])

# ensure that the ThreadExceptionDialog is modal
tedFrame.mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(tedFrame, "Form")
statesCheck(tedFrame.dialog, "Dialog", add_states=["active", "modal"], invalid_states=["resizable"])

##############################
# check children of the Dialog's AtkAccessible
##############################
statesCheck(tedFrame.description_label, "Label")
statesCheck(tedFrame.errortype_label, "Label")

statesCheck(tedFrame.detail_button, "Button", add_states=["focused"])
statesCheck(tedFrame.ignore_button, "Button")
statesCheck(tedFrame.abort_button, "Button")

# press tab to change focus
tedFrame.keyCombo("Tab", grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(tedFrame.detail_button, "Button")
statesCheck(tedFrame.ignore_button, "Button", add_states=["focused"])
statesCheck(tedFrame.abort_button, "Button")
tedFrame.keyCombo("Tab", grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(tedFrame.detail_button, "Button")
statesCheck(tedFrame.ignore_button, "Button")
statesCheck(tedFrame.abort_button, "Button", add_states=["focused"])

##############################
# Test each children
##############################
# TEST DETAIL_BUTTON & TEXTBOX & ERRORTITLE_LABEL
# perform click to show the textbox
tedFrame.showTextBox(tedFrame.detail_button)
statesCheck(tedFrame.errortitle_label, "Label")
statesCheck(tedFrame.textbox, "ThreadExceptionDialog_Text")
statesCheck(tedFrame.scrollbar_ver, "VScrollBar")
statesCheck(tedFrame.scrollbar_hor, "HScrollBar")

# ensure that the textbox is not editable
error_msg = tedFrame.textbox.text
tedFrame.assignText(tedFrame.textbox, "test")
tedFrame.assertText(tedFrame.textbox, error_msg)

# hide the textbox
tedFrame.hideTextBox(tedFrame.detail_button)

# TEST IGNORE_BUTTON and make sure that the original window becomes active
tedFrame.ignore_button.click(log=True)
sleep(config.SHORT_DELAY)
statesCheck(tedFrame, "Form", add_states=["active"])

# TEST ABORT_BUTTON and make sure that the original window becomes active
tedFrame.showDialog(tedFrame.raise_button)
sleep(config.SHORT_DELAY)
tedFrame.abort_button.click(log=True)
sleep(config.SHORT_DELAY)

tedFrame.assertClosed()

##############################
# End
##############################
print "INFO:  Log written to: %s" % config.OUTPUT_DIR
