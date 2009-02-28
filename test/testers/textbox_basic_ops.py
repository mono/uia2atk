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
# check mormal textbox
statesCheck(tbFrame.textbox_normal, "TextBox", add_states=["focused"])
statesCheck(tbFrame.textbox_mline, "TextBox", add_states=["multi line"], invalid_states=["single line"])
statesCheck(tbFrame.textbox_passwd, "TextBox")
# TODO: BUG480266, non-editable textbox should not have "editable" state
#statesCheck(tbFrame.textbox_nonedit, "TextBox", invalid_states=["editable", "enabled", "focusable", "sensitive"])
statesCheck(tbFrame.textbox_nonedit, "TextBox", invalid_states=["enabled", "focusable", "sensitive"])

# switch focus to next multi line  textbox
tbFrame.keyCombo("Tab", grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(tbFrame.textbox_normal, "TextBox")
statesCheck(tbFrame.textbox_mline, "TextBox", add_states=["multi line", "focused"], invalid_states=["single line"])
statesCheck(tbFrame.textbox_passwd, "TextBox")
# TODO: BUG480266, non-editable textbox should not have "editable" state
#statesCheck(tbFrame.textbox_nonedit, "TextBox", invalid_states=["editable", "enabled", "focusable", "sensitive"])
statesCheck(tbFrame.textbox_nonedit, "TextBox", invalid_states=["enabled", "focusable", "sensitive"])

# switch focus to next password textbox
tbFrame.textbox_passwd.mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(tbFrame.textbox_normal, "TextBox")
statesCheck(tbFrame.textbox_mline, "TextBox", add_states=["multi line"], invalid_states=["single line"])
statesCheck(tbFrame.textbox_passwd, "TextBox", add_states=["focused"])
# TODO: BUG480266, non-editable textbox should not have "editable" state
#statesCheck(tbFrame.textbox_nonedit, "TextBox", invalid_states=["editable", "enabled", "focusable", "sensitive"])
statesCheck(tbFrame.textbox_nonedit, "TextBox", invalid_states=["enabled", "focusable", "sensitive"])

# switch focus to single line textbox
tbFrame.keyCombo("Tab", grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(tbFrame.textbox_normal, "TextBox", add_states=["focused"])
statesCheck(tbFrame.textbox_mline, "TextBox", add_states=["multi line"], invalid_states=["single line"])
statesCheck(tbFrame.textbox_passwd, "TextBox")
# TODO: BUG480266, non-editable textbox should not have "editable" state
#statesCheck(tbFrame.textbox_nonedit, "TextBox", invalid_states=["editable", "enabled", "focusable", "sensitive"])
statesCheck(tbFrame.textbox_nonedit, "TextBox", invalid_states=["enabled", "focusable", "sensitive"])

##############################
# check TextBox's AtkAction, AtkComponent, AtkEditableText and AtkText
##############################
##
## Single line TextBox
## 
# TODO: Move cursor: text-caret-moved 
# Insert a character: text-changed:insert, text-caret-moved
tbFrame.textbox_normal.typeText("test")
sleep(config.SHORT_DELAY)
tbFrame.assertEditableText(tbFrame.textbox_normal, "test")

tbFrame.textbox_normal.enterText("textbox")
sleep(config.SHORT_DELAY)
tbFrame.assertEditableText(tbFrame.textbox_normal, "textbox")

# prefix text test
tbFrame.textbox_normal.insertText("single line ", 0)
sleep(config.SHORT_DELAY)
tbFrame.assertEditableText(tbFrame.textbox_normal, "single line textbox")

# append text test
text_len = len(tbFrame.textbox_normal.text)
tbFrame.textbox_normal.insertText(" test", text_len) 
sleep(config.SHORT_DELAY)
tbFrame.assertEditableText(tbFrame.textbox_normal, "single line textbox test")

# Select a character: text-caret-moved, text-selection-changed 
# TODO: select error
#tbFrame.textbox_normal.assertSelectedText("test", text_len - 4)

# Delete a character: text-changed:delete, text-caret-moved 
# delete text from end (delete "test")
text_len = len(tbFrame.textbox_normal.text)
tbFrame.textbox_normal.deleteText(text_len - 5)
sleep(config.SHORT_DELAY)
tbFrame.assertEditableText(tbFrame.textbox_normal, "single line textbox")

# delete text from front (delete "single line ")
tbFrame.textbox_normal.deleteText(0, 12)
sleep(config.SHORT_DELAY)
tbFrame.assertEditableText(tbFrame.textbox_normal, "textbox")

##
## Multi line TextBox
## 
tbFrame.textbox_mline.mouseClick()
# Move cursor: text-caret-moved 
# Insert a character: text-changed:insert, text-caret-moved
mline_content1 = """
Strongwind is a GUI test automation framework Strongwind is object-oriented and extensible. You can use Strongwind to build object-oriented representations of your applications ("application wrappers"), then reuse the application wrappers to quickly develop many test scripts. Strongwind scripts generate a human-readable log that contains the action, expected result and a screen shot of each step. Most simple actions are logged automatically.
"""
# TODO: when you use typeText method to input strings to textbox, all upper chars converted to lower chars, 
# so it leads to the actual text does not match the expected text.
#tbFrame.textbox_mline.typeText(mline_content1)
#sleep(config.SHORT_DELAY)
#tbFrame.assertEditableText(tbFrame.textbox_mline, mline_content1)

mline_content2 = """
Strongwind is written in Python and uses the pyatspi library to manipulate and query the state of applications. Strongwind automatically classifies widgets by their ATK role and provides implementations for common actions on regular widgets --for example, selecting an item in a menu or asserting that a window has closed --but you can extend Strongwind\'s implementations or add your own implementations for custom widgets to handle alternate behaviors or custom widgets in your applications.
"""
tbFrame.textbox_mline.enterText(mline_content2)
sleep(config.SHORT_DELAY)
tbFrame.assertEditableText(tbFrame.textbox_mline, mline_content2)

# prefix text test
tbFrame.textbox_mline.insertText(mline_content1, 0)
sleep(config.SHORT_DELAY)
tbFrame.assertEditableText(tbFrame.textbox_mline, mline_content1 + mline_content2) 

# append text test
text_len = len(tbFrame.textbox_mline.text)
tbFrame.textbox_mline.insertText("\nmulti-line textbox", text_len) 
sleep(config.SHORT_DELAY)
tbFrame.assertEditableText(tbFrame.textbox_mline, mline_content1 + mline_content2 + "\nmulti-line textbox")

# Select a character: text-caret-moved, text-selection-changed 
# Delete a character: text-changed:delete, text-caret-moved 
# delete text from end (delete "\nmulti-line textbox")
text_len = len(tbFrame.textbox_mline.text)
tbFrame.textbox_mline.deleteText(text_len - 19)
sleep(config.SHORT_DELAY)
tbFrame.assertEditableText(tbFrame.textbox_mline, mline_content1 + mline_content2) 

# delete text from front (delete mline_content1)
content_len = len(mline_content1)
tbFrame.textbox_mline.deleteText(0, content_len)
sleep(config.SHORT_DELAY)
tbFrame.assertEditableText(tbFrame.textbox_mline, mline_content2)

##
## Password TextBox
## 
tbFrame.textbox_passwd.mouseClick()
# Move cursor: text-caret-moved 
# Insert a character: text-changed:insert, text-caret-moved
tbFrame.textbox_passwd.typeText("test")
sleep(config.SHORT_DELAY)
tbFrame.assertEditableText(tbFrame.textbox_passwd, "test")

tbFrame.textbox_passwd.enterText("textbox")
sleep(config.SHORT_DELAY)
tbFrame.assertEditableText(tbFrame.textbox_passwd, "textbox")

# prefix text test
tbFrame.textbox_passwd.insertText("passwd ", 0)
sleep(config.SHORT_DELAY)
tbFrame.assertEditableText(tbFrame.textbox_passwd, "passwd textbox")

# append text test
text_len = len(tbFrame.textbox_passwd.text)
tbFrame.textbox_passwd.insertText(" test", text_len) 
sleep(config.SHORT_DELAY)
tbFrame.assertEditableText(tbFrame.textbox_passwd, "passwd textbox test")

# Select a character: text-caret-moved, text-selection-changed 
# Delete a character: text-changed:delete, text-caret-moved 
# delete text from end (delete "test")
text_len = len(tbFrame.textbox_passwd.text)
tbFrame.textbox_passwd.deleteText(text_len - 5)
sleep(config.SHORT_DELAY)
tbFrame.assertEditableText(tbFrame.textbox_passwd, "passwd textbox") 

# delete text from front (delete passwd)
tbFrame.textbox_passwd.deleteText(0, 7)
sleep(config.SHORT_DELAY)
tbFrame.assertEditableText(tbFrame.textbox_passwd, "textbox")

##
## Non-Editable TextBox
## 
# TODO: BUG480752 non-Enabled textbox should not be editable
tbFrame.textbox_nonedit.enterText("non-Enabled textbox")
sleep(config.SHORT_DELAY)
tbFrame.assertEditableText(tbFrame.textbox_nonedit, "")

##############################
# check TextBox's AtkStreamableContent
##############################
tbFrame.assertStreamableContent(tbFrame.textbox_normal)
sleep(config.SHORT_DELAY)
tbFrame.assertStreamableContent(tbFrame.textbox_mline)
sleep(config.SHORT_DELAY)
tbFrame.assertStreamableContent(tbFrame.textbox_passwd)
sleep(config.SHORT_DELAY)
tbFrame.assertStreamableContent(tbFrame.textbox_nonedit)

##############################
# End
##############################
# close application frame window
tbFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
