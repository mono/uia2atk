#!/usr/bin/env python

##
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        12/16/2008
# Description: Test accessibility of listview widget 
#              Use the listviewframe.py wrapper script
#              Test the samples/ListView_detail.py script
##

# The docstring below  is used in the generated log file
"""
Test accessibility of listview_detail widget
"""

# imports
import sys
import os

from strongwind import *
from listview_detail import *
from helpers import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the listview_detail sample application
try:
  app = launchListView(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
lvFrame = app.listViewFrame

#check ListView TableColumnHeader's actions list
actionsCheck(lvFrame.column_a, "TableColumnHeader")
actionsCheck(lvFrame.column_b, "TableColumnHeader")

#check ListView checkbox's actions list
for index in range(6):
    actionsCheck(lvFrame.checkbox[index], "CheckBox")

#check TreeTable 's states
statesCheck(lvFrame.treetable, "TreeTable", add_states=["focused"])

#check TableColumnHeader's states list
statesCheck(lvFrame.column_a, "TableColumnHeader")
statesCheck(lvFrame.column_b, "TableColumnHeader")

#check checkbox's states
for index in range(6):
    statesCheck(lvFrame.checkbox[index], "ListViewCheckBox")

#check text's states
for index in range(13):
    statesCheck(lvFrame.texts[index], "ListViewText")

#mouse click Column A to rise "focused"
lvFrame.column_a.mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(lvFrame.column_a, "TableColumnHeader", add_states=["focused"])

#mouse click Column num to rise "focused", Column A without focused
lvFrame.column_b.mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(lvFrame.column_b, "TableColumnHeader", add_states=["focused"])
statesCheck(lvFrame.column_a, "TableColumnHeader")

#mouse click Item0 checkbox to rise "checked" and "focused"
lvFrame.checkbox[0].mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(lvFrame.checkbox[0], "ListViewCheckBox", add_states=["checked", "focused"])

lvFrame.checkbox[0].mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(lvFrame.checkbox[0], "ListViewCheckBox", add_states=["focused"])

#do click action for Item1 checkbox to rise "checked"
lvFrame.click(lvFrame.checkbox[1])
sleep(config.SHORT_DELAY)
statesCheck(lvFrame.checkbox[1], "ListViewCheckBox", add_states=["checked"])

lvFrame.click(lvFrame.checkbox[1])
sleep(config.SHORT_DELAY)
statesCheck(lvFrame.checkbox[1], "ListViewCheckBox", add_states=["focused"])

#mouse click items under Column A, both items in the same row be selected, 
#because FullRowSelect is True
lvFrame.texts[0].mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(lvFrame.texts[0], "ListViewText", add_states=["selected"])
statesCheck(lvFrame.texts[1], "ListViewText", add_states=["selected"])

#check TreeTable selection implementation
lvFrame.assertSelectionChild(lvFrame.treetable, 0)
sleep(config.SHORT_DELAY)
statesCheck(lvFrame.texts[0], "ListViewText", add_states=["selected"])
statesCheck(lvFrame.texts[1], "ListViewText", add_states=["selected"])

lvFrame.assertSelectionChild(lvFrame.treetable, 3)
sleep(config.SHORT_DELAY)
statesCheck(lvFrame.texts[3], "ListViewText", add_states=["selected"])
statesCheck(lvFrame.texts[4], "ListViewText", add_states=["selected"])

#clear selection to get rid of "selected" state
lvFrame.assertClearSelection(lvFrame.treetable)
sleep(config.SHORT_DELAY)
statesCheck(lvFrame.texts[3], "ListViewText")
statesCheck(lvFrame.texts[4], "ListViewText")

#check text implementation
lvFrame.assertText(lvFrame.texts)

lvFrame.assertText(lvFrame.checkbox)

#check table's table implementation
lvFrame.assertTable(lvFrame.treetable, row=6, col=2)

#check item's order after click column header
lvFrame.click(lvFrame.column_a)
sleep(config.SHORT_DELAY)
lvFrame.assertOrder(texindex=0, expect="Item5")
lvFrame.assertOrder(texindex=1, expect="5")

lvFrame.click(lvFrame.column_b)
sleep(config.SHORT_DELAY)
lvFrame.assertOrder(texindex=0, expect="Item0")
lvFrame.assertOrder(texindex=1, expect="0")

#check text is uneditable
lvFrame.enterTextValue(lvFrame.texts[0], "aaaa", oldtext="Item0")

lvFrame.enterTextValue(lvFrame.texts[1], "10", oldtext="0")

#close application frame window
lvFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
