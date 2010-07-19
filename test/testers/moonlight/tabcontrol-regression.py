#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        09/21/2009
# Description: Test accessibility of tabcontrol widget 
#              Use the tabcontrolframe.py wrapper script
#              Test the Moonlight TabControl sample
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of tabcontrol widget
"""

# imports

from strongwind import *
from TabControl import *
from helpers import *
from sys import argv
from os import path

app_path = None 
try:
    app_path = argv[1]
except IndexError:
    pass  #expected

# open the TabControl sample application
try:
    app = launchTabControl(app_path)
except IOError, msg:
    print "ERROR:  %s" % msg
    exit(2)

# make sure we got the app back
if app is None:
    exit(4)

# just an alias to make things shorter
tcFrame = app.TabControlFrame

######################
# TabControl Actions
######################
for tab_item in tcFrame.tab_items:
    actionsCheck(tab_item, "TabPage")

#############################
# TabControl default States
#############################
statesCheck(tcFrame.tab, "TabControl")

for tab_item in tcFrame.tab_items:
    if tab_item.text == "TabItem1":
        # TabItem1 is focused and selected by default
        statesCheck(tab_item, "TabPage", add_states=["focused", "selected"])
        # A label in TabItem1 is showing on
        statesCheck(tcFrame.label_in_tab, "Label")
    else:
        statesCheck(tab_item, "TabPage")
        tcFrame.testLabel(is_showing=False)

################################
# Do Click action for TabPages
################################
# click each TabItem to check its states and make sure the label in it
# will be showing
for tab_item in tcFrame.tab_items:
    tab_item.click(log=True)
    sleep(config.SHORT_DELAY)
    # main_label shows which one tab_item is selected
    assertText(tcFrame.main_label, "Selected:%s" % tab_item.text)
    # label in tab_item is showing up and visible
    tcFrame.testLabel("This is %s" % tab_item.text, is_showing=True)
    # the clicked tab_item is focused and selected
    statesCheck(tab_item, "TabPage", add_states=["focused", "selected"])

###################
# nevigation
###################
# mouse button click TabItem0 to grab focus
tcFrame.tab_item0.mouseClick()
sleep(config.SHORT_DELAY)
assertText(tcFrame.main_label, "Selected:TabItem0")
tcFrame.testLabel("This is TabItem0", is_showing=True)
statesCheck(tcFrame.tab_item0, "TabPage", add_states=["focused", "selected"])

# move focus to TabItem1 by key/Right
tcFrame.keyCombo("Right", grabFouse=False)
sleep(config.SHORT_DELAY)
assertText(tcFrame.main_label, "Selected:TabItem1")
tcFrame.testLabel("This is TabItem1", is_showing=True)
statesCheck(tcFrame.tab_item1, "TabPage", add_states=["focused", "selected"])
# TabItem0 won't focused again
statesCheck(tcFrame.tab_item0, "TabPage")

# move focus to TabItem2 by key/Up
tcFrame.keyCombo("Up", grabFouse=False)
sleep(config.SHORT_DELAY)
assertText(tcFrame.main_label, "Selected:TabItem2")
tcFrame.testLabel("This is TabItem2", is_showing=True)
statesCheck(tcFrame.tab_item2, "TabPage", add_states=["focused", "selected"])
# TabItem0 won't focused again
statesCheck(tcFrame.tab_item1, "TabPage")

print "INFO:  Log written to: %s" % config.OUTPUT_DIR

#close application frame window
quit(tcFrame)
