#!/usr/bin/env python
# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        02/11/2009
# Description: main test script of threadexceptiondialog
#              ../samples/threadexceptiondialog.py is the test sample script
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
tedFrame.show_dialog(tedFrame.raise_button)
sleep(config.SHORT_DELAY)

##############################
# check Frame and Dialog's AtkAccessible
##############################
statesCheck(tedFrame, "Form")
# BUG474694
#statesCheck(tedFrame.dialog, "Dialog", add_states=["active", "modal"])
statesCheck(tedFrame.dialog, "Dialog", add_states=["active", "modal"], invalid_states=["resizable"])

tedFrame.mouseClick()
statesCheck(tedFrame, "Form")
# BUG474694
#statesCheck(tedFrame.dialog, "Dialog", add_states=["active", "modal"])
statesCheck(tedFrame.dialog, "Dialog", add_states=["active", "modal"], invalid_states=["resizable"])

##############################
# check children of the Dialog's AtkAccessible
##############################
statesCheck(tedFrame.description_label, "Label")
statesCheck(tedFrame.errortype_label, "Label")

statesCheck(tedFrame.detail_button, "Button", add_states=["focused"])
statesCheck(tedFrame.ignore_button, "Button")
statesCheck(tedFrame.abort_button, "Button")

# BUG474254
#statesCheck(tedFrame.scrollbar_ver, "ScrollBar")
#statesCheck(tedFrame.scrollbar_hor, "ScrollBar")

##############################
# Test each children
##############################
# TEST DETAIL_BUTTON & TEXTBOX & ERRORTITLE_LABEL
# perform click to show the textbox
tedFrame.show_textbox(tedFrame.detail_button)
statesCheck(tedFrame.errortitle_label, "Label")
statesCheck(tedFrame.textbox, "ThreadExceptionDialog_Text")

# test the textbox could be edit or not
error_msg = tedFrame.textbox.text
tedFrame.inputText(tedFrame.textbox, "test")
tedFrame.assertText(tedFrame.textbox, error_msg)

# hide the textbox
tedFrame.hide_textbox(tedFrame.detail_button)
sleep(config.SHORT_DELAY)

# TEST IGNORE_BUTTON
tedFrame.click(tedFrame.ignore_button)
#
statesCheck(tedFrame, "Form", add_states=["active"])

# TEST ABORT_BUTTON
tedFrame.show_dialog(tedFrame.raise_button)
sleep(config.SHORT_DELAY)

tedFrame.click(tedFrame.abort_button)
sleep(config.SHORT_DELAY)

tedFrame.assertClosed()

##############################
# End
##############################
print "INFO:  Log written to: %s" % config.OUTPUT_DIR
