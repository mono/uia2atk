#!/usr/bin/env python

##
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        01/07/2009
# Description: Test accessibility of tabpage widget 
#              Use the tabpageframe.py wrapper script
#              Test the samples/tabpage.py script
##

# The docstring below  is used in the generated log file
"""
Test accessibility of tabpage widget
"""

# imports
import sys
import os

from strongwind import *
from tabpage import *
from helpers import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the tabpage sample application
try:
  app = launchTabPage(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
tpFrame = app.tabPageFrame


#check TabPages' default states, tabpage0 with focused and selected
statesCheck(tpFrame.tabpage0, "TabPage", add_states=["focused", "selected"])
statesCheck(tpFrame.tabpage1, "TabPage")
statesCheck(tpFrame.tabpage2, "TabPage")
statesCheck(tpFrame.tabpage3, "TabPage")

#items in TabPages should with correct states, button with showing
statesCheck(tpFrame.button, "Button")
statesCheck(tpFrame.textbox, "Text", invalid_states=["showing"])
statesCheck(tpFrame.checkbox, "CheckBox", invalid_states=["showing"])
statesCheck(tpFrame.radiobutton, "RadioButton", invalid_states=["showing"])

#check AtkText implementation
tpFrame.assertText(tpFrame.tabpage0, "Tab 0")
tpFrame.assertText(tpFrame.tabpage1, "Tab 1")
tpFrame.assertText(tpFrame.tabpage2, "Tab 2")
tpFrame.assertText(tpFrame.tabpage3, "Tab 3")

######Test Tab 0######
#keyTab move focus from tabpage0 to button
tpFrame.keyCombo("Tab", grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(tpFrame.button, "Button", add_states=["focused"])
statesCheck(tpFrame.tabpage0, "TabPage")
#keyTab move focus from button to tabpage0
tpFrame.keyCombo("Tab", grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(tpFrame.tabpage0, "TabPage", add_states=["focused", "selected"])
statesCheck(tpFrame.button, "Button")

######Test Tab 1######
#keyRight move focus to Tab 1
tpFrame.keyCombo("Right", grabFocus=False)
sleep(config.SHORT_DELAY)
#label is changed to show the current tab page 1
tpFrame.assertTabChange("Tab 1")
#the current tab page 1 with focused and selected state
statesCheck(tpFrame.tabpage1, "TabPage", add_states=["focused", "selected"])
#the uper tab page 0 get rid of focused state
statesCheck(tpFrame.tabpage0, "TabPage")
#TextBox in current tab page 1 with showing state, others without showing state
statesCheck(tpFrame.textbox, "Text")
statesCheck(tpFrame.button, "Button", invalid_states=["showing"])
statesCheck(tpFrame.checkbox, "CheckBox", invalid_states=["showing"])
statesCheck(tpFrame.radiobutton, "RadioButton", invalid_states=["showing"])
#keyTab move focus from tabpage1 to textbox
tpFrame.keyCombo("Tab", grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(tpFrame.textbox, "Text", add_states=["focused"])
statesCheck(tpFrame.tabpage1, "TabPage")
#keyTab move focus from button to tabpage0
tpFrame.keyCombo("Tab", grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(tpFrame.tabpage1, "TabPage", add_states=["focused", "selected"])
statesCheck(tpFrame.textbox, "Text")

######Test Tab 2######
#keyRight move focus to Tab 2
tpFrame.keyCombo("Right", grabFocus=False)
sleep(config.SHORT_DELAY)
#label is changed to show the current tab page 2
tpFrame.assertTabChange("Tab 2")
#the current tab page 2 with focused and selected state
statesCheck(tpFrame.tabpage2, "TabPage", add_states=["focused", "selected"])
#the uper tab page 1 get rid of focused state
statesCheck(tpFrame.tabpage1, "TabPage")
#CheckBox in current tab page 2 with showing state, others without showing state
statesCheck(tpFrame.checkbox, "CheckBox")
statesCheck(tpFrame.textbox, "Text", invalid_states=["showing"])
statesCheck(tpFrame.button, "Button", invalid_states=["showing"])
statesCheck(tpFrame.radiobutton, "RadioButton", invalid_states=["showing"])
#keyTab move focus from tabpage2 to checkbox
tpFrame.keyCombo("Tab", grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(tpFrame.checkbox, "CheckBox", add_states=["focused"])
statesCheck(tpFrame.tabpage2, "TabPage")
#keyTab move focus from checkbox to tabpage0
tpFrame.keyCombo("Tab", grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(tpFrame.tabpage2, "TabPage", add_states=["focused", "selected"])
statesCheck(tpFrame.checkbox, "CheckBox")

######Test Tab 3######
#keyRight move focus to Tab 3
tpFrame.keyCombo("Right", grabFocus=False)
sleep(config.SHORT_DELAY)
#label is changed to show the current tab page 3
tpFrame.assertTabChange("Tab 3")
#the current tab page 3 with focused and selected state
statesCheck(tpFrame.tabpage3, "TabPage", add_states=["focused", "selected"])
#the uper tab page 2 get rid of focused state
statesCheck(tpFrame.tabpage2, "TabPage")
#RadioButton in current tab page 3 with showing state, others without showing state
statesCheck(tpFrame.radiobutton, "RadioButton")
statesCheck(tpFrame.textbox, "Text", invalid_states=["showing"])
statesCheck(tpFrame.button, "Button", invalid_states=["showing"])
statesCheck(tpFrame.checkbox, "CheckBox", invalid_states=["showing"])
#keyTab move focus from tabpage3 to radiobutton
tpFrame.keyCombo("Tab", grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(tpFrame.radiobutton, "RadioButton", add_states=["focused"])
statesCheck(tpFrame.tabpage3, "TabPage")
#keyTab move focus from radiobutton to tabpage0
tpFrame.keyCombo("Tab", grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(tpFrame.tabpage3, "TabPage", add_states=["focused", "selected"])
statesCheck(tpFrame.radiobutton, "RadioButton")

######Test items action######
#click radiobutton to change label
tpFrame.click(tpFrame.radiobutton)
sleep(config.SHORT_DELAY)
tpFrame.assertLabeChange("checked radiobutton")

#keyLeft move focus to Tab 2, check/uncheck checkbox to change label
tpFrame.keyCombo("Left", grabFocus=False)
sleep(config.SHORT_DELAY)
tpFrame.click(tpFrame.checkbox)
sleep(config.SHORT_DELAY)
tpFrame.assertLabeChange("checked")

tpFrame.click(tpFrame.checkbox)
sleep(config.SHORT_DELAY)
tpFrame.assertLabeChange("unchecked")

#keyLeft move focus to Tab 1, input something to change label
tpFrame.keyCombo("Left", grabFocus=False)
sleep(config.SHORT_DELAY)
tpFrame.enterTextValue(tpFrame.textbox, "enter")
sleep(config.SHORT_DELAY)
tpFrame.assertLabeChange("enter")

#keyLeft move focus to Tab 0, click buton to change label
tpFrame.keyCombo("Left", grabFocus=False)
sleep(config.SHORT_DELAY)
tpFrame.click(tpFrame.button)
sleep(config.SHORT_DELAY)
tpFrame.assertLabeChange("You click me 1 times")

#close application frame window
tpFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
