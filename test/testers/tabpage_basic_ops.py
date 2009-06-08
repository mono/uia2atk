#!/usr/bin/env python

##################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        01/07/2009
# Description: Test accessibility of tabpage widget 
#              Use the tabpageframe.py wrapper script
#              Test the samples/tabpage.py script
##################################################################

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
  print "ERROR: %s" % msg
  exit(2)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
tpFrame = app.tabPageFrame

####################################################################
# check TabPages' default states, tab_page_0 with focused and selected
####################################################################
statesCheck(tpFrame.tab_page_0, "TabPage", add_states=["focused", "selected"])
statesCheck(tpFrame.tab_page_1, "TabPage")
statesCheck(tpFrame.tab_page_2, "TabPage")
statesCheck(tpFrame.tab_page_3, "TabPage")

# BUG510841: off screen tab page is missing "visible" state 
#statesCheck(tpFrame.tab_pages[6], "TabPage", invalid_states=["showing"])

###################
# Test Tab 0
###################

# items in TabPages should with correct states, button is showing
statesCheck(tpFrame.button, "Button")

# keyTab move focus from tab_page_0 to button
tpFrame.keyCombo("Tab", grabFocus=False)
sleep(config.SHORT_DELAY)
# We should set 'export MONO_WINFORMS_XIM_STYLE=disabled' to ensure keyCombo
# working fine
statesCheck(tpFrame.button, "Button", add_states=["focused"])
statesCheck(tpFrame.tab_page_0, "TabPage", add_states=["selected"])
# keyTab move focus from button to tab_page_0
tpFrame.keyCombo("Tab", grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(tpFrame.tab_page_0, "TabPage", add_states=["focused", "selected"])
statesCheck(tpFrame.button, "Button")

# click the button on Tab 0, and make sure that the label changes
tpFrame.button.click(log=True)
sleep(config.SHORT_DELAY)
tpFrame.assertLabelText("You click me 1 times")

###################
# Test Tab 1
###################

# keyRight move focus to Tab 1
tpFrame.keyCombo("Right", grabFocus=False)
sleep(config.SHORT_DELAY)
# tab page 1 is selected, find its accessibles
tpFrame.findTabPageAccessibles(1)
tpFrame.assertTabPageChildCount(tpFrame.tab_page_1, 2)
# the current tab page 1 with focused and selected state
statesCheck(tpFrame.tab_page_1, "TabPage", add_states=["focused", "selected"])
# the uper tab page 0 get rid of focused state
statesCheck(tpFrame.tab_page_0, "TabPage")
# TextBox in current tab page 1 with showing state
assert tpFrame.textbox.showing

# keyTab move focus from tab_page_1 to textbox
tpFrame.keyCombo("Tab", grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(tpFrame.textbox, "Text", add_states=["focused"])
statesCheck(tpFrame.tab_page_1, "TabPage", add_states=["selected"])
# keyTab move focus from button to tab_page_0
tpFrame.keyCombo("Tab", grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(tpFrame.tab_page_1, "TabPage", add_states=["focused", "selected"])
statesCheck(tpFrame.textbox, "Text")

tpFrame.enterTextValue(tpFrame.textbox, "enter")
sleep(config.SHORT_DELAY)
tpFrame.assertLabelText("You input:enter")

###################
# Test Tab 2
###################

# keyRight move focus to Tab 2
tpFrame.keyCombo("Right", grabFocus=False)
sleep(config.SHORT_DELAY)
# tab page 2 is selected, find its accessibles
tpFrame.findTabPageAccessibles(2)
tpFrame.assertTabPageChildCount(tpFrame.tab_page_2, 2)
# the current tab page 2 with focused and selected state
statesCheck(tpFrame.tab_page_2, "TabPage", add_states=["focused", "selected"])
# the uper tab page 1 get rid of focused state
statesCheck(tpFrame.tab_page_1, "TabPage")
# CheckBox in current tab page 2 with showing state
assert tpFrame.checkbox.showing

# keyTab move focus from tab_page_2 to checkbox
tpFrame.keyCombo("Tab", grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(tpFrame.checkbox, "CheckBox", add_states=["focused"])
statesCheck(tpFrame.tab_page_2, "TabPage", add_states=["selected"])
# keyTab move focus from checkbox to tab_page_0
tpFrame.keyCombo("Tab", grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(tpFrame.tab_page_2, "TabPage", add_states=["focused", "selected"])
statesCheck(tpFrame.checkbox, "CheckBox")

tpFrame.checkbox.click(log=True)
sleep(config.SHORT_DELAY)
tpFrame.assertLabelText("checked")

tpFrame.checkbox.click(log=True)
sleep(config.SHORT_DELAY)
tpFrame.assertLabelText("unchecked")

###################
# Test Tab 3
###################

# keyRight move focus to Tab 3
tpFrame.keyCombo("Right", grabFocus=False)
sleep(config.SHORT_DELAY)
# tab page 3 is selected, find its accessibles
tpFrame.findTabPageAccessibles(3)
tpFrame.assertTabPageChildCount(tpFrame.tab_page_3, 2)
# the current tab page 3 with focused and selected state
statesCheck(tpFrame.tab_page_3, "TabPage", add_states=["focused", "selected"])
# the uper tab page 2 get rid of focused state
statesCheck(tpFrame.tab_page_2, "TabPage")
# RadioButton in current tab page 3 with showing state
assert tpFrame.radiobutton.showing

tpFrame.radiobutton.click(log=True)
sleep(config.SHORT_DELAY)
tpFrame.assertLabelText("checked radiobutton")

##############################
# check AtkText implementation
##############################
tpFrame.assertText(tpFrame.tab_page_0, "Tab 0")
tpFrame.assertText(tpFrame.tab_page_1, "Tab 1")
tpFrame.assertText(tpFrame.tab_page_2, "Tab 2")
tpFrame.assertText(tpFrame.tab_page_3, "Tab 3")

# close application frame window
tpFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
