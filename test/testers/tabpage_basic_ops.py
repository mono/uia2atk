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

#check AtkText implementation
tpFrame.assertText(tpFrame.tabpage0, "Tab 0")
tpFrame.assertText(tpFrame.tabpage1, "Tab 1")
tpFrame.assertText(tpFrame.tabpage2, "Tab 2")
tpFrame.assertText(tpFrame.tabpage3, "Tab 3")

#check TabPages' default states, tabpage0 with focused and selected
statesCheck(tpFrame.tabpage0, "TabPage", add_states=["focused", "selected"])
statesCheck(tpFrame.tabpage1, "TabPage")
statesCheck(tpFrame.tabpage2, "TabPage")
statesCheck(tpFrame.tabpage3, "TabPage")

######Test Tab 0######

#items in TabPages should with correct states, button with showing
statesCheck(tpFrame.button, "Button")

#keyTab move focus from tabpage0 to button
tpFrame.keyCombo("Tab", grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(tpFrame.button, "Button", add_states=["focused"])
statesCheck(tpFrame.tabpage0, "TabPage", add_states=["selected"])
#keyTab move focus from button to tabpage0
tpFrame.keyCombo("Tab", grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(tpFrame.tabpage0, "TabPage", add_states=["focused", "selected"])
statesCheck(tpFrame.button, "Button")

tpFrame.click(tpFrame.button)
sleep(config.SHORT_DELAY)
tpFrame.assertLabeChange("You click me 1 times")

######Test Tab 1######
#keyRight move focus to Tab 1
tpFrame.keyCombo("Right", grabFocus=False)
sleep(config.SHORT_DELAY)
#label is changed to show the current tab page 1
tpFrame.assertTabChange(tpFrame.tabpage1, "Tab 1")
#the current tab page 1 with focused and selected state
statesCheck(tpFrame.tabpage1, "TabPage", add_states=["focused", "selected"])
#the uper tab page 0 get rid of focused state
statesCheck(tpFrame.tabpage0, "TabPage")
#TextBox in current tab page 1 with showing state
assert tpFrame.textbox.showing

#keyTab move focus from tabpage1 to textbox
tpFrame.keyCombo("Tab", grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(tpFrame.textbox, "Text", add_states=["focused"])
statesCheck(tpFrame.tabpage1, "TabPage", add_states=["selected"])
#keyTab move focus from button to tabpage0
tpFrame.keyCombo("Tab", grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(tpFrame.tabpage1, "TabPage", add_states=["focused", "selected"])
statesCheck(tpFrame.textbox, "Text")

tpFrame.enterTextValue(tpFrame.textbox, "enter")
sleep(config.SHORT_DELAY)
tpFrame.assertLabeChange("You input:enter")

######Test Tab 2######
#keyRight move focus to Tab 2
tpFrame.keyCombo("Right", grabFocus=False)
sleep(config.SHORT_DELAY)
#label is changed to show the current tab page 2
tpFrame.assertTabChange(tpFrame.tabpage2, "Tab 2")
#the current tab page 2 with focused and selected state
statesCheck(tpFrame.tabpage2, "TabPage", add_states=["focused", "selected"])
#the uper tab page 1 get rid of focused state
statesCheck(tpFrame.tabpage1, "TabPage")
#CheckBox in current tab page 2 with showing state
assert tpFrame.checkbox.showing

#keyTab move focus from tabpage2 to checkbox
tpFrame.keyCombo("Tab", grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(tpFrame.checkbox, "CheckBox", add_states=["focused"])
statesCheck(tpFrame.tabpage2, "TabPage", add_states=["selected"])
#keyTab move focus from checkbox to tabpage0
tpFrame.keyCombo("Tab", grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(tpFrame.tabpage2, "TabPage", add_states=["focused", "selected"])
statesCheck(tpFrame.checkbox, "CheckBox")

tpFrame.click(tpFrame.checkbox)
sleep(config.SHORT_DELAY)
tpFrame.assertLabeChange("checked")

tpFrame.click(tpFrame.checkbox)
sleep(config.SHORT_DELAY)
tpFrame.assertLabeChange("unchecked")

######Test Tab 3######
#keyRight move focus to Tab 3
tpFrame.keyCombo("Right", grabFocus=False)
sleep(config.SHORT_DELAY)
#label is changed to show the current tab page 3
tpFrame.assertTabChange(tpFrame.tabpage3, "Tab 3")
#the current tab page 3 with focused and selected state
statesCheck(tpFrame.tabpage3, "TabPage", add_states=["focused", "selected"])
#the uper tab page 2 get rid of focused state
statesCheck(tpFrame.tabpage2, "TabPage")
#RadioButton in current tab page 3 with showing state, others without showing state
assert tpFrame.radiobutton.showing

tpFrame.click(tpFrame.radiobutton)
sleep(config.SHORT_DELAY)
tpFrame.assertLabeChange("checked radiobutton")

#close application frame window
tpFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
