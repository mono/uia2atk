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

# check the states of the text boxes
statesCheck(tbFrame.textbox_normal, "TextBox", add_states=["focused"])
statesCheck(tbFrame.textbox_mline,
            "TextBox",
            add_states=["multi line"],
            invalid_states=["single line"])
statesCheck(tbFrame.textbox_passwd, "TextBox")
statesCheck(tbFrame.textbox_nonedit,
            "TextBox",
            invalid_states=["editable", "enabled", "focusable", "sensitive"])

# switch focus to next multi line textbox and check the states of all the
# text boxes again
tbFrame.keyCombo("Tab", grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(tbFrame.textbox_normal, "TextBox")
statesCheck(tbFrame.textbox_mline,
            "TextBox",
            add_states=["multi line", "focused"],
            invalid_states=["single line"])
statesCheck(tbFrame.textbox_passwd, "TextBox")
statesCheck(tbFrame.textbox_nonedit,
            "TextBox",
            invalid_states=["editable", "enabled", "focusable", "sensitive"])

# switch focus to next password textbox (using a mouse click) and check the
# states
tbFrame.textbox_passwd.mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(tbFrame.textbox_normal, "TextBox")
statesCheck(tbFrame.textbox_mline, "TextBox", add_states=["multi line"], invalid_states=["single line"])
statesCheck(tbFrame.textbox_passwd, "TextBox", add_states=["focused"])
statesCheck(tbFrame.textbox_nonedit, "TextBox", invalid_states=["editable", "enabled", "focusable", "sensitive"])

# switch focus to single line textbox and check the states
tbFrame.keyCombo("Tab", grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(tbFrame.textbox_normal, "TextBox", add_states=["focused"])
statesCheck(tbFrame.textbox_mline,
            "TextBox",
            add_states=["multi line"],
            invalid_states=["single line"])
statesCheck(tbFrame.textbox_passwd, "TextBox")
statesCheck(tbFrame.textbox_nonedit,
            "TextBox",
            invalid_states=["editable", "enabled", "focusable", "sensitive"])

##############################
# check TextBox's AtkAction, AtkComponent, AtkEditableText and AtkText
##############################
##
## Single line TextBox
## 
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

tbFrame.textbox_mline.enterText(mline_content1)
sleep(config.SHORT_DELAY)
tbFrame.assertEditableText(tbFrame.textbox_mline, mline_content1)

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
mline_content3 = "\nmulti-line textbox\n\n"
text_len = len(tbFrame.textbox_mline.text)
tbFrame.textbox_mline.insertText(mline_content3, text_len) 
sleep(config.SHORT_DELAY)
tbFrame.assertEditableText(tbFrame.textbox_mline,
                     ''.join([mline_content1, mline_content2, mline_content3]))

# find the scroll bar accessibles now that they exist, and then check their
# states
tbFrame.findScrollBars(tbFrame.textbox_mline)
statesCheck(tbFrame.vertical_scroll_bar, "VScrollBar")
statesCheck(tbFrame.horizontal_scroll_bar, "HScrollBar")

# Select a character: text-caret-moved, text-selection-changed 
# Delete a character: text-changed:delete, text-caret-moved 
# delete text from end (delete "\nmulti-line textbox")
text_len = len(tbFrame.textbox_mline.text)
tbFrame.textbox_mline.deleteText(text_len - len(mline_content3))
sleep(config.SHORT_DELAY)
tbFrame.assertEditableText(tbFrame.textbox_mline, mline_content1 + mline_content2) 

# delete text from front (delete mline_content1)
content_len = len(mline_content1)
tbFrame.textbox_mline.deleteText(0, content_len)
sleep(config.SHORT_DELAY)
tbFrame.assertEditableText(tbFrame.textbox_mline, mline_content2)

# delete all text and then test the typeText method
# BUG491282 when you use typeText method to input strings to textbox, all
# upper chars converted to lower chars, so it leads to the actual text does
# not match the expected text.  
#tbFrame.textbox_mline.deleteText()
#sleep(config.SHORT_DELAY)
#tbFrame.textbox_mline.typeText(mline_content1)
#sleep(config.SHORT_DELAY)
#tbFrame.assertEditableText(tbFrame.textbox_mline, mline_content1)

##
## Password TextBox
## 
tbFrame.textbox_passwd.mouseClick()
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
