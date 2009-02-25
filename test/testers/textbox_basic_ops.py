#!/usr/bin/env python
# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        02/20/2008
# Description: main test script of textbox
#              ../samples/textbox.py is the test sample script
#              textbox/* is the wrapper of textbox test sample script
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of TextBox widget
"""

# imports
import pdb
from textbox import *
from helpers import *
from actions import *
from states import *
from eventlistener import *
from sys import argv

app_path = None 
try:
    app_path = argv[1]
except IndexError:
    pass #expected

# open the textbox sample application
try:
    app = launchTextBox(app_path)
except IOError, msg:
    print "ERROR:  %s" % msg
    exit(2)

sleep(config.SHORT_DELAY)

# make sure we got the app back
if app is None:
    exit(4)

# just an alias to make things shorter
tbFrame = app.textBoxFrame

##############################
# check TextBox's AtkAccessible
##############################
## check mormal textbox
#statesCheck(tbFrame.textbox_normal, "TextBox", add_states=["focused"])
#statesCheck(tbFrame.textbox_mline, "TextBox", add_states=["multi line"], invalid_states=["single line"])
#statesCheck(tbFrame.textbox_passwd, "TextBox")
#
## switch focus to next multi line  textbox
#tbFrame.keyCombo("Tab", grabFocus=False)
#sleep(config.SHORT_DELAY)
#
#statesCheck(tbFrame.textbox_normal, "TextBox")
#statesCheck(tbFrame.textbox_mline, "TextBox", add_states=["multi line", "focused"], invalid_states=["single line"])
#statesCheck(tbFrame.textbox_passwd, "TextBox")
#
## switch focus to next password textbox
#tbFrame.textbox_passwd.mouseClick()
#sleep(config.SHORT_DELAY)
#
#statesCheck(tbFrame.textbox_normal, "TextBox")
#statesCheck(tbFrame.textbox_mline, "TextBox", add_states=["multi line"], invalid_states=["single line"])
#statesCheck(tbFrame.textbox_passwd, "TextBox", add_states=["focused"])

##############################
# check TextBox's AtkAction and AtkComponent 
##############################
# check its events
#reg = EventListener(event_types=("object:text-caret-moved", "text-changed", "text-selection-changed"))
#reg.start()

# switch focus to normal textbox
tbFrame.textbox_normal.mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(tbFrame.textbox_normal, "TextBox", add_states=["focused"])

# Insert a character: text-changed:insert, text-caret-moved
tbFrame.textbox_normal.typeText("test")
sleep(config.SHORT_DELAY)
tbFrame.assertText(tbFrame.textbox_normal, "test")
# TODO: need to write some test to test event change or assert offset to indicate the event is occured. 
#assert reg.containsEvent(tbFrame.textbox_normal, 'object:text-caret-moved')
#reg.clearQueuedEvents()

tbFrame.textbox_normal.enterText("sample")
sleep(config.SHORT_DELAY)
tbFrame.assertText(tbFrame.textbox_normal, "sample")
# TODO: need to write some test to test event change or assert offset to indicate the event is occured. 
#assert reg.containsEvent(tbFrame.textbox_normal, 'object:text-caret-moved')
#reg.clearQueuedEvents()

tbFrame.textbox_normal.insertText("test", 0)
sleep(config.SHORT_DELAY)
tbFrame.assertText(tbFrame.textbox_normal, "testsample")
# TODO: need to write some test to test event change or assert offset to indicate the event is occured. 
#assert reg.containsEvent(tbFrame.textbox_normal, ["object:text-caret-moved"])
#reg.clearQueuedEvents()

# Select a character: text-caret-moved, text-selection-changed 
#tbFrame.textbox_normal.getSelectedText(0)
tbFrame.textbox_normal.assertSelectedText("sample", 3)
# TODO: need to write some test to test event change or assert offset to indicate the event is occured. 
#assert reg.containsEvent(tbFrame.textbox_normal, ["object:text-caret-moved"])
#reg.clearQueuedEvents()
tbFrame.textbox_normal.removeTextSelection()

# Delete a character: text-changed:delete, text-caret-moved 
# delete text from 4 to end (delete "sample")
tbFrame.textbox_normal.deleteText(4)
tbFrame.assertText(tbFrame.textbox_normal, "text")

# Drag and drop a character: text-changed:insert, text-changed:delete, 
#                            text-caret-moved 

#reg.stop()
##############################
# check TextBox's AtkStreamableContent
##############################

##############################
# check TextBox's AtkText
##############################

##############################
# check TextBox's AtkEditableText
##############################

##############################
# End
##############################
# close application frame window
tbFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
