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
# combobox states
statesCheck(cbFrame.combobox1, "ComboBox", add_states=["focusable", "showing", "expandable"])
statesCheck(cbFrame.combobox2, "ComboBox", add_states=["focusable", "showing", "expandable"])
statesCheck(cbFrame.combobox3, "ComboBox", add_states=["focusable", "showing", "expandable"])

# listitems states
cbFrame.checkAllStates(cbFrame.box1_listitems, focused_item="Item 0")
cbFrame.checkAllStates(cbFrame.box2_listitems, focused_item="Item 1")
#BUG629820
#cbFrame.checkAllStates(cbFrame.box3_listitems, focused_item="Honda")

#################
# Actions check
#################
# combobox action
for i in cbFrame.comboboxs:
    actionsCheck(i, "ComboBox", add_actions=["expand or collapse"], invalid_actions=["press"])

#BUG631348
#actionCheck(cbFrame.box2_checkbox, "CheckBox")

# do click action two times for each combobox, menu is expand and collapse
cbFrame.combobox1.expandOrCollapse(log=True)
sleep(config.SHORT_DELAY)
cbFrame.combobox1.expandOrCollapse(log=True)
sleep(config.SHORT_DELAY)
cbFrame.combobox2.expandOrCollapse(log=True)
sleep(config.SHORT_DELAY)
cbFrame.combobox2.expandOrCollapse(log=True)
sleep(config.SHORT_DELAY)
cbFrame.combobox3.expandOrCollapse(log=True)
sleep(config.SHORT_DELAY)
cbFrame.combobox3.expandOrCollapse(log=True)
sleep(config.SHORT_DELAY)

# do click action for checkbox in combobox2 to update label
#BUG631348
#cbFrame.box2_checkbox.click()
#sleep(config.SHORT_DELAY)
#assertText(cbFrame.label2, "item3_checkbox is Checked")
#statesCheck(cbFrame.box2_checkbox, "CheckBox", add_states=["checked"])
#cbFrame.box2_checkbox.click()
#sleep(config.SHORT_DELAY)
#assertText(cbFrame.label2, "item3_checkbox is Unchecked")
#statesCheck(cbFrame.box2_checkbox, "CheckBox")


#####################
# Text implementation
#####################
# listitems in combobox1
for i in range(10):
    assert cbFrame.box1_listitems[i].name == "Item " + str(i)
# listitems in combobox2
#BUG631348
#for i in range(7):
#    assert cbFrame.box2_listitems[i].name == "Item " + str(i)
# listitems in combobox3
#BUG629820
#items = ["Ferrari", "Honda", "Toyota"]
#for i in range(3):
#    assert cbFrame.box3_listitems[i].name == items[i]


'''no focused
##########################
# Selection implementation
##########################
# combobox1
cbFrame.assertSelectChild(cbFrame.combobox1, 5)
sleep(config.SHORT_DELAY)
statesCheck(cbFrame.box1_listitems[5], "ListItem", add_states=["selected", "focused"])
cbFrame.assertSelectChild(cbFrame.combobox1, 0)
sleep(config.SHORT_DELAY)
statesCheck(cbFrame.box1_listitems[0], "ListItem", add_states=["selected", "focused"])

cbFrame.assertClearSelection(cbFrame.combobox1)
sleep(config.SHORT_DELAY)
statesCheck(cbFrame.box1_listitems[0], "ListItem")

# combobox2
cbFrame.assertSelectChild(cbFrame.combobox2, 3)
sleep(config.SHORT_DELAY)
statesCheck(cbFrame.box2_listitems[3], "ListItem", add_states=["selected", "focused"])
cbFrame.assertSelectChild(cbFrame.combobox2, 5)
sleep(config.SHORT_DELAY)
statesCheck(cbFrame.box2_listitems[5], "ListItem", add_states=["selected", "focused"])

cbFrame.assertClearSelection(cbFrame.combobox2)
sleep(config.SHORT_DELAY)
statesCheck(cbFrame.box2_listitems[5], "ListItem")

# combobox3
cbFrame.assertSelectChild(cbFrame.combobox3, 0)
sleep(config.SHORT_DELAY)
statesCheck(cbFrame.box3_listitems[0], "ListItem", add_states=["selected", "focused"])

cbFrame.assertClearSelection(cbFrame.combobox3)
sleep(config.SHORT_DELAY)
statesCheck(cbFrame.box3_listitems[0], "ListItem")
'''

###############################
# Add & Delete & Reset menuitem
###############################
# insert new item name to the text
cbFrame.text1.insertText("item aaa")
sleep(config.SHORT_DELAY)
# click Add Item button
cbFrame.add_button.click()
sleep(config.SHORT_DELAY)
# refind listitems
box1_listitems = cbFrame.combobox1.findAllListItems(None)
# 11 listitems in box1
cbFrame.assertNumber(box1_listitems, 11)
# the last listitem's text should be the insert text
assert box1_listitems[-1].name == "item aaa"

'''no click on listitem
# click one listitem
cbFrame.combobox1.select("Item 3")
sleep(config.SHORT_DELAY)
# click Delete Item button to delete it
cbFrame.del_button.click()
sleep(config.SHORT_DELAY)
# refind listitems
box1_listitems = cbFrame.combobox1.findAllListItems(None)
# 10 listitems in box1
cbFrame.assertNumber(box1_listitems, 10)
'''

# click Reset Item button
cbFrame.reset_button.click()
sleep(config.SHORT_DELAY)
# refind listitems
box1_listitems = cbFrame.combobox1.findAllListItems(None)
# 10 listitems in box1
cbFrame.assertNumber(box1_listitems, 10)
# the last listitem's text should be Item 9
assert box1_listitems[-1].name == "Item 9"

print "INFO:  Log written to: %s" % config.OUTPUT_DIR

#close application frame window
quit(cbFrame)
