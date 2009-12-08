#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        11/03/2009
# Description: Test accessibility of textbox widget
#              Use the textboxframe.py wrapper script
#              Test the Moonlight textbox sample
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of textbox widget
"""

# imports
from strongwind import *
from textbox import *
from helpers import *
from sys import argv
from os import path

app_path = None
try:
    app_path = argv[1]
except IndexError:
    pass  #expected

# open the textbox sample application
try:
    app = launchTextBox(app_path)
except IOError, msg:
    print "ERROR:  %s" % msg
    exit(2)

# make sure we got the app back
if app is None:
    exit(4)

# just an alias to make things shorter
tbFrame = app.textBoxFrame

##############
# Check States
##############
# check the states of the text boxes
##BUG553160
#statesCheck(tbFrame.textbox1, "TextBox")
##BUG553160
#statesCheck(tbFrame.textbox2, "TextBox", invalid_states=["editable"])
##BUG553160
#statesCheck(tbFrame.textbox3, "TextBox")

# mouse click frame to focus on textbox1
tbFrame.mouseClick()
sleep(config.SHORT_DELAY)
##BUG553160
#statesCheck(tbFrame.textbox1, "TextBox", add_states=["focused"])
##BUG553160
#statesCheck(tbFrame.textbox2, "TextBox", invalid_states=["editable"])
##BUG553160
#statesCheck(tbFrame.textbox3, "TextBox")

# switch focus to textbox2
tbFrame.keyCombo("Tab", grabFocus=False)
sleep(config.SHORT_DELAY)
##BUG553160
#statesCheck(tbFrame.textbox1, "TextBox")
##BUG553160
#statesCheck(tbFrame.textbox2, "TextBox", add_states=["focused"],
#                                           invalid_states=["editable"])
##BUG553160
#statesCheck(tbFrame.textbox3, "TextBox")

# switch focus to textbox3
tbFrame.textbox3.mouseClick()
sleep(config.SHORT_DELAY)
##BUG553160
#statesCheck(tbFrame.textbox1, "TextBox")
##BUG553160
#statesCheck(tbFrame.textbox2, "TextBox", invalid_states=["editable"])
##BUG553160
#statesCheck(tbFrame.textbox3, "TextBox", add_states=["focused"])

##############################
# Test Read and Write TextBox
##############################
# Delete texts
tbFrame.textbox1.deleteText()
sleep(config.SHORT_DELAY)
# Insert texts from 0 offset
tbFrame.textbox1.insertText("single line", 0)
sleep(config.SHORT_DELAY)
tbFrame.assertEditableText(tbFrame.textbox1, "single line")
# textbox2's text is updated to the textbox1's text
assertText(tbFrame.textbox2, "single line")

# append text at the end
text_len = len(tbFrame.textbox1.text)
tbFrame.textbox1.insertText(" test", text_len)
sleep(config.SHORT_DELAY)
tbFrame.assertEditableText(tbFrame.textbox1, "single line test")
# textbox2's text is updated to the insert text
assertText(tbFrame.textbox2, "single line test")

# delete text from end (delete "test")
text_len = len(tbFrame.textbox1.text)
tbFrame.textbox1.deleteText(text_len - 5)
sleep(config.SHORT_DELAY)
tbFrame.assertEditableText(tbFrame.textbox1, "single line")
assertText(tbFrame.textbox2, "single line")

# delete text from front (delete "single")
tbFrame.textbox1.deleteText(0, 7)
sleep(config.SHORT_DELAY)
tbFrame.assertEditableText(tbFrame.textbox1, "line")
assertText(tbFrame.textbox2, "line")

# enter texts
tbFrame.textbox1.enterText("new test line")
sleep(config.SHORT_DELAY)
tbFrame.assertEditableText(tbFrame.textbox1, "new test line")
assertText(tbFrame.textbox2, "new test line")

####################
# Test Read TextBox
####################
# Insert texts from 0 offset
tbFrame.textbox2.insertText("single line ", 0)
sleep(config.SHORT_DELAY)
# textbox2 is not editable
tbFrame.assertEditableText(tbFrame.textbox2, "new test line")

# delete text from front (delete "single")
tbFrame.textbox2.deleteText(0, 7)
sleep(config.SHORT_DELAY)
# it's not deleteable
tbFrame.assertEditableText(tbFrame.textbox2, "new test line")

######################
# Test Search TextBox
######################
##TODO:search textbox has bugs
# mouse click textbox3 to remove text "Search"
tbFrame.textbox3.mouseClick()
sleep(config.SHORT_DELAY)
tbFrame.assertEditableText(tbFrame.textbox3, "")

# mouse click other textbox to reset text "Search"
tbFrame.textbox2.mouseClick()
sleep(config.SHORT_DELAY)
tbFrame.assertEditableText(tbFrame.textbox3, "Search")

# type texts
tbFrame.textbox3.typeText("Search TextBox ")
sleep(config.SHORT_DELAY)
tbFrame.assertEditableText(tbFrame.textbox3, "Search TextBox")

# move focus doesn't change text
tbFrame.textbox2.mouseClick()
sleep(config.SHORT_DELAY)
tbFrame.assertEditableText(tbFrame.textbox3, "Search TextBox")

# grab focus to textbox3 to remove text
tbFrame.keyCombo("Tab", grabFocus=False)
sleep(config.SHORT_DELAY)
tbFrame.assertEditableText(tbFrame.textbox3, "")

# move focus again will change text to "Search"
tbFrame.textbox2.mouseClick()
sleep(config.SHORT_DELAY)
tbFrame.assertEditableText(tbFrame.textbox3, "Search")

###############
# Close Firefox
###############
quit(tbFrame)
