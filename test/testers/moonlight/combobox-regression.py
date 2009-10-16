#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        09/25/2009
# Description: Test accessibility of combobox widget
#              Use the comboboxframe.py wrapper script
#              Test the Moonlight combobox sample
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of combobox widget
"""

# imports
from strongwind import *
from combobox import *
from helpers import *
from sys import argv
from os import path

app_path = None
try:
    app_path = argv[1]
except IndexError:
    pass  #expected

# open the combobox sample application
try:
    app = launchComboBox(app_path)
except IOError, msg:
    print "ERROR:  %s" % msg
    exit(2)

# make sure we got the app back
if app is None:
    exit(4)

# just an alias to make things shorter
cbFrame = app.comboBoxFrame

#################
# Default States
#################
# menu states
statesCheck(cbFrame.box1_menu, "Menu", invalid_states=["showing"])

statesCheck(cbFrame.box2_menu, "Menu", invalid_states=["showing"])

statesCheck(cbFrame.box3_menu, "Menu", invalid_states=["showing"])

# menuitems states
cbFrame.checkAllStates(cbFrame.box1_menuitems, focused_item="Item 0")

cbFrame.checkAllStates(cbFrame.box2_menuitems, focused_item="Item 1")

cbFrame.checkAllStates(cbFrame.box3_menuitems, focused_item="Honda")

#################
# Actions check
#################
# combobox action
for i in cbFrame.comboboxs:
    actionsCheck(i, "Combobox")

# menuitem action
for i in cbFrame.box1_menuitems:
    actionsCheck(i, "MenuItem")

for i in cbFrame.box2_menuitems:
    actionCheck(i, "MenuItem")

actionCheck(cbFrame.box2_checkbox, "CheckBox")

for i in cbFrame.box3_menuitems:
    actionCheck(i, "MenuItem")

# do click action two times for each combobox, menu is expand and collapse
cbFrame.combobox1.click(log=True)
sleep(config.SHORT_DELAY)
statesCheck(cbFrame.box1_menu, "Menu")

cbFrame.combobox1.click(log=True)
sleep(config.SHORT_DELAY)
statesCheck(cbFrame.box1_menu, "Menu", invalid_states=["showing"])

cbFrame.combobox2.click(log=True)
sleep(config.SHORT_DELAY)
statesCheck(cbFrame.box2_menu, "Menu")

cbFrame.combobox2.click(log=True)
sleep(config.SHORT_DELAY)
statesCheck(cbFrame.box2_menu, "Menu", invalid_states=["showing"])

cbFrame.combobox3.click(log=True)
sleep(config.SHORT_DELAY)
statesCheck(cbFrame.box3_menu, "Menu")

cbFrame.combobox3.click(log=True)
sleep(config.SHORT_DELAY)
statesCheck(cbFrame.box3_menu, "Menu", invalid_states=["showing"])

# do click action for menuitems in combobox1
for item in cbFrame.box1_menuitems:
    item.click(log=True)
    sleep(config.SHORT_DELAY)
    statesCheck(item, "MenuItem", add_states=["selected", "focused"])

# do click action for menuitems in combobox2
for item in cbFrame.box2_menuitems:
    item.click(log=True)
    sleep(config.SHORT_DELAY)
    assertText(cbFrame.text2, "Selected:%s" % item.name)
    statesCheck(item, "MenuItem", add_states=["selected", "focused"])

# do click action for menuitems in combobox3
for item in cbFrame.box3_menuitems:
    item.click(log=True)
    sleep(config.SHORT_DELAY)
    statesCheck(item, "MenuItem", add_states=["selected", "focused"])

# do click action for checkbox in combobox2 to update label
cbFrame.box2_checkbox.click(log=True)
sleep(config.SHORT_DELAY)
assertText(cbFrame.text2, "item3_checkbox is Checked")
statesCheck(cbFrame.box2_checkbox, "CheckBox", add_states=["checked"])

cbFrame.box2_checkbox.click(log=True)
sleep(config.SHORT_DELAY)
assertText(cbFrame.text2, "item3_checkbox is Unchecked")
statesCheck(cbFrame.box2_checkbox, "CheckBox")

#####################
# Text implementation
#####################
# menuitems in combobox1
for i in range(10):
    assertText(cbFrame.box1_menuitems[i], "Item " + str(i))

# menuitems in combobox2
for i in range(6):
    assertText(cbFrame.box2_menuitems[i], "Item " + str(i))

# menuitems in combobox3
items = ["Ferrari", "Honda", "Toyota"]
for i in range(3):
    assertText(cbFrame.box3_menuitems[i], items[i])

##########################
# Selection implementation
##########################
# combobox1
cbFrame.assertSelectChild(cbFrame.combobox1, 5)
sleep(config.SHORT_DELAY)
statesCheck(cbFrame.box1_menuitems[5], "MenuItem", add_states=["selected",
                                                                  "focused"])
cbFrame.assertSelectChild(cbFrame.combobox1, 0)
sleep(config.SHORT_DELAY)
statesCheck(cbFrame.box1_menuitems[0], "MenuItem", add_states=["selected",
                                                                  "focused"])

cbFrame.assertClearSelection(cbFrame.combobox1)
sleep(config.SHORT_DELAY)
statesCheck(cbFrame.box1_menuitems[0], "MenuItem")

# combobox2
cbFrame.assertSelectChild(cbFrame.combobox2, 3)
sleep(config.SHORT_DELAY)
statesCheck(cbFrame.box2_menuitems[3], "MenuItem", add_states=["selected",
                                                                  "focused"])
cbFrame.assertSelectChild(cbFrame.combobox2, 5)
sleep(config.SHORT_DELAY)
statesCheck(cbFrame.box2_menuitems[5], "MenuItem", add_states=["selected",
                                                                  "focused"])

cbFrame.assertClearSelection(cbFrame.combobox2)
sleep(config.SHORT_DELAY)
statesCheck(cbFrame.box2_menuitems[5], "MenuItem")

# combobox3
cbFrame.assertSelectChild(cbFrame.combobox3, 0)
sleep(config.SHORT_DELAY)
statesCheck(cbFrame.box3_menuitems[0], "MenuItem", add_states=["selected",
                                                                  "focused"])

cbFrame.assertClearSelection(cbFrame.combobox3)
sleep(config.SHORT_DELAY)
statesCheck(cbFrame.box3_menuitems[0], "MenuItem")

###############################
# Add & Delete & Reset menuitem
###############################
# insert new item name to the text
cbFrame.text.insertText("item aaa")
sleep(config.SHORT_DELAY)
# click Add Item button
cbFrame.add_button.click(log=True)
sleep(config.SHORT_DELAY)
# refind menuitems
box1_menuitems = cbFrame.combobox1.findAllMenuItems(None)
# 11 menuitems in box1
cbFrame.assertNumber(box1_menuitems, 11)
# the last menuitem's text should be the insert text
assertText(box1_menuitems[-1], "item aaa")

# click one menuitem
box1_menuitems[2].click(log=True)
sleep(config.SHORT_DELAY)
# click Delete Item button to delete it
cbFrame.del_button.click(log=True)
sleep(config.SHORT_DELAY)
# refind menuitems
box1_menuitems = cbFrame.combobox1.findAllMenuItems(None)
# 10 menuitems in box1
cbFrame.assertNumber(box1_menuitems, 10)

# click Reset Item button
cbFrame.reset_button.click(log=True)
sleep(config.SHORT_DELAY)
# refind menuitems
box1_menuitems = cbFrame.combobox1.findAllMenuItems(None)
# 10 menuitems in box1
cbFrame.assertNumber(box1_menuitems, 10)
# the last menuitem's text should be Item 9
assertText(box1_menuitems[-1], "Item 9")

print "INFO:  Log written to: %s" % config.OUTPUT_DIR

#close application frame window
quit(cbFrame)
