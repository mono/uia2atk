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
from textbox import *
from helpers import *
from actions import *
from states import *
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
# check mormal textbox
statesCheck(tbFrame.textbox_normal, "TextBox", add_states=["focused"])
statesCheck(tbFrame.textbox_mline, "TextBox", add_states=["multi line"], invalid_states=["single line"])
statesCheck(tbFrame.textbox_passwd, "TextBox")

# switch focus to next multi line  textbox
tbFrame.keyCombo("Tab", grabFocus=False)
sleep(config.SHORT_DELAY)

statesCheck(tbFrame.textbox_normal, "TextBox")
statesCheck(tbFrame.textbox_mline, "TextBox", add_states=["multi line", "focused"], invalid_states=["single line"])
statesCheck(tbFrame.textbox_passwd, "TextBox")

# switch focus to next password textbox
tbFrame.textbox_passwd.mouseClick()
sleep(config.SHORT_DELAY)

statesCheck(tbFrame.textbox_normal, "TextBox")
statesCheck(tbFrame.textbox_mline, "TextBox", add_states=["multi line"], invalid_states=["single line"])
statesCheck(tbFrame.textbox_passwd, "TextBox", add_states=["focused"])

##############################
# check TextBox's AtkAction and AtkComponent 
##############################
# switch focus to normal textbox
tbFrame.textbox_normal.mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(tbFrame.textbox_normal, "TextBox", add_states=["focused"])

# TODO: test move cursor: text-caret-moved

# Insert a character: text-changed:insert, text-caret-moved
tbFrame.textbox_normal.typeText("test")
sleep(config.SHORT_DELAY)
tbFrame.assertText(tbFrame.textbox_normal, "test")
tbFrame.assertOffset(tbFrame.textbox_normal, 4)

tbFrame.textbox_normal.insertText("test", 0)
sleep(config.SHORT_DELAY)
tbFrame.assertText(tbFrame.textbox_normal, "testtest")
tbFrame.assertOffset(tbFrame.textbox_normal, 8)

tbFrame.textbox_normal.inputText("test", 0)
sleep(config.SHORT_DELAY)
tbFrame.assertText(tbFrame.textbox_normal, "testtesttest")
tbFrame.assertOffset(tbFrame.textbox_normal, 12)

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
