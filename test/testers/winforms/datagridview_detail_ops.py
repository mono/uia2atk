#!/usr/bin/env python

###############################################################################
####    Written by:  Mario Carrion <mcarrion@novell.com>                  #####
####    Date:        02/23/2009                                           #####
####    Description: Test accessibility of DataGridView widget            #####
####                 Use the datagridviewframe.py wrapper script          #####
####                 Test the samples/winforms/datagridview.py script              #####
###############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of DataGridView widget
"""

# imports
import sys
import os

from strongwind import *
from datagridview import *
from helpers import *
from eventlistener import *
from sys import argv
from os import path
import pyatspi

app_path = None 
try:
	app_path = argv[1]
except IndexError:
	pass #expected

# open the datagridview sample application
try:
	app = launchDataGridView(app_path)
except IOError, msg:
	print "ERROR:  %s" % msg
	exit(2)

# make sure we got the app back
if app is None:
	exit(4)

# Alias to make things shorter
dtgvFrame = app.dataGridViewFrame

# -> Check DataGridView TableColumnHeader's actions and states list
for column in dtgvFrame.columns:
	actionsCheck(column, "TableColumnHeader")
	statesCheck(column, "TableColumnHeader")
	
	# Mouse clicking column won't rise "focused" because 
    #HasKeyboardFocus is False
	column.mouseClick()
	sleep(config.SHORT_DELAY)
	statesCheck(column, "TableColumnHeader")
	
# -> CheckBox cells tests
statesCheck(dtgvFrame.tablecells[0], "TreeViewTableCell") 
statesCheck(dtgvFrame.tablecells[1], "TreeViewTableCell", add_states=\
                                                                ["editable"])
statesCheck(dtgvFrame.tablecells[2], "TreeViewTableCell")

####*reg = EventListener(event_types='object:state-changed')
####*reg.start()
####*
####*checkbox = dtgvFrame.findAllCheckBoxes(None)[1]
####*checkbox.grabFocus()
####*
####*
####*assert reg.containsEvent(checkbox, 'object:state-changed:focused')
####*
####*reg.clearQueuedEvents()
####*
####*sleep(config.LONG_DELAY)
####*
####*statesCheck(checkbox, "CheckBox", add_states=[ \
####*    "selectable", "selected", "focused" \
####*])
####*
####*checkbox.click()
####*sleep(config.LONG_DELAY)
####*statesCheck(checkbox, "CheckBox", add_states=[ \
####*    "selectable", "selected", "focused", "checked" \
####*])
####*
####*assert reg.containsEvent(checkbox, 'object:state-changed:checked')
####*
####*reg.clearQueuedEvents()
####*
####*checkbox.click()
####*sleep(config.LONG_DELAY)
####*statesCheck(checkbox, "CheckBox", add_states=[ \
####*   "selectable", "selected", "focused", \
####*])
####*
####*sleep(config.LONG_DELAY)
####*assert reg.containsEvent(checkbox, 'object:state-changed:checked')
####*
####*reg.stop()
####*
# -> TextBox cell tests
dtgvFrame.assertEditsText(dtgvFrame.edits)

dtgvFrame.edits[0].mouseClick()
sleep(config.LONG_DELAY)
statesCheck(dtgvFrame.edits[0], "TextBox", add_states=[ "selectable",\
               "selected", "focused",'transient'],invalid_states=['editable'])

dtgvFrame.edits[1].mouseClick()
sleep(config.LONG_DELAY)
statesCheck(dtgvFrame.edits[1], "TextBox", add_states=["editable", \
                             "selectable","selected", "focused", "transient"])
statesCheck(dtgvFrame.edits[0], "TextBox", add_states=["selectable", \
                                     "transient"],invalid_states=["editable"])

dtgvFrame.edits[0].grabFocus()
statesCheck(dtgvFrame.edits[0], "TextBox", add_states=[ "selectable",\
                "selected","focused" ,"transient"],invalid_states=['editable'])

dtgvFrame.edits[1].grabFocus()
statesCheck(dtgvFrame.edits[1], "TextBox", add_states=["editable",\
                              "selectable","selected", "focused" ,"transient"])

# -> statecheck and actioncheck test for Button cell 

for button in dtgvFrame.buttons:
	actionsCheck(button,"Button")
	statesCheck(button,"Button", add_states=["selectable"])
	
###
dtgvFrame.buttons[0].mouseClick()
sleep(config.SHORT_DELAY)

dtgvFrame.assertCellClickValue(5, 3)
dtgvFrame.assertCurrentCellValue(5, 3)
statesCheck(dtgvFrame.buttons[0],"Button", add_states=["focused",\
                                                    "selectable", "selected"])

dtgvFrame.buttons[1].mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(dtgvFrame.buttons[0],"Button", add_states=["selectable"])

dtgvFrame.assertCellClickValue(4, 3)
dtgvFrame.assertCurrentCellValue(4, 3)

statesCheck(dtgvFrame.buttons[1],"Button", add_states=["focused",\
                                                    "selectable", "selected"])
statesCheck(dtgvFrame.buttons[0],"Button", add_states=["selectable"])
# -> statecheck and for Link cell 

for label in dtgvFrame.labels:
    statesCheck(label,"Label",  add_states=["selectable", "focusable"])

dtgvFrame.labels[0].mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(dtgvFrame.labels[0],"Label", add_states=["selectable",\
                                          "focusable", "selected", "focused"])

dtgvFrame.labels[1].grabFocus()
sleep(config.SHORT_DELAY)
statesCheck(dtgvFrame.labels[1],"Label", add_states=["selectable",\
                                          "focusable", "selected", "focused"])
statesCheck(dtgvFrame.labels[0],"Label", add_states=["selectable", \
                                                                 "focusable"])

dtgvFrame.labels[2].mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(dtgvFrame.labels[1],"Label", add_states=["selectable",\
                                                                 "focusable"])
statesCheck(dtgvFrame.labels[2],"Label", add_states=["selectable",\
                                          "focusable", "selected", "focused"])

### Testing click
dtgvFrame.buttons[0].mouseClick()
sleep(config.SHORT_DELAY)
dtgvFrame.assertCellClickValue(5, 3)

statesCheck(dtgvFrame.buttons[0],"Button", add_states=["focused",\
                                                  "selectable", "selected"])
dtgvFrame.buttons[1].mouseClick()
sleep(config.SHORT_DELAY)

statesCheck(dtgvFrame.buttons[0],"Button", add_states=["selectable"])
dtgvFrame.assertCellClickValue(4, 3)

statesCheck(dtgvFrame.buttons[1],"Button", add_states=["focused",\
                                                    "selectable", "selected"])
# -> Link cell tests
for label in dtgvFrame.labels:
	statesCheck(button,"Label", invalid_states=["multi line"],\
                                       add_states=["selectable", "focusable"])

# -> ComboBox cell tests
#BUG503078 combobox in datagridview lacks of 'sensitive' ,'enabled' state
#statesCheck(dtgvFrame.comboboxes[0],"ComboBox", add_states=["selectable"])

# -> All cells: Selection tests
# ->test after selecting many accessibles , the state of every accessible
# will correct

# test after selecting many accessibles, the "focused" state of the first
# checkbox will not lost
dtgvFrame.assertClearSelection(dtgvFrame.treetable)
sleep(config.SHORT_DELAY)

# Selecting first checkbox, and check it's state
dtgvFrame.checkboxes[0].mouseClick()
sleep(config.SHORT_DELAY)

dtgvFrame.treetable.selectChild(dtgvFrame.checkboxes[0].getIndexInParent())
sleep(config.SHORT_DELAY)

statesCheck(dtgvFrame.checkboxes[0], "CheckBox", add_states=["selected",\
                                           "checked", "selectable","focused"])
sleep(config.SHORT_DELAY)

# Selecting first tablecell, and check it's state
dtgvFrame.treetable.selectChild(dtgvFrame.edits[0].getIndexInParent())
sleep(config.SHORT_DELAY)

statesCheck(dtgvFrame.edits[0], "TableCell", add_states=["selected", \
                                                                "selectable"])
sleep(config.SHORT_DELAY)

statesCheck(dtgvFrame.checkboxes[0], "CheckBox", add_states=["selected",\
                                           "checked", "selectable","focused"])

# Selecting first combobox, and check it's state
dtgvFrame.treetable.selectChild(dtgvFrame.comboboxes[0].getIndexInParent())
sleep(config.SHORT_DELAY)

statesCheck(dtgvFrame.comboboxes[0], "ComboBox", add_states=["selected", \
                                                                "selectable"])
sleep(config.SHORT_DELAY)

statesCheck(dtgvFrame.checkboxes[0], "CheckBox", add_states=["selected",\
                                           "checked", "selectable","focused"])

# Selecting first pushbutton, and check it's state

dtgvFrame.treetable.selectChild(dtgvFrame.buttons[0].getIndexInParent())
sleep(config.SHORT_DELAY)

statesCheck(dtgvFrame.buttons[0], "Button", add_states=["selected",\
                                                                "selectable"])
sleep(config.SHORT_DELAY)

statesCheck(dtgvFrame.checkboxes[0], "CheckBox", add_states=["selected",\
                                           "checked", "selectable","focused"])

# Selecting first label, and check it's state
dtgvFrame.treetable.selectChild(dtgvFrame.labels[0].getIndexInParent())
sleep(config.SHORT_DELAY)

statesCheck(dtgvFrame.labels[0], "Label", add_states=["selected",\
                                                    "selectable","focusable"])
sleep(config.SHORT_DELAY)

statesCheck(dtgvFrame.checkboxes[0], "CheckBox", add_states=["selected",\
                                           "checked", "selectable","focused"])

# Selecting second checkbox, and check it's state
dtgvFrame.treetable.selectChild(dtgvFrame.checkboxes[1].getIndexInParent())
sleep(config.SHORT_DELAY)

statesCheck(dtgvFrame.checkboxes[1], "CheckBox", add_states=["selected",\
                                                                "selectable"])
sleep(config.SHORT_DELAY)

statesCheck(dtgvFrame.checkboxes[0], "CheckBox", add_states=["selected",\
                                           "checked", "selectable","focused"])


# Selecting second tablecell, and check it's state
dtgvFrame.treetable.selectChild(dtgvFrame.tablecells[1].getIndexInParent())
sleep(config.SHORT_DELAY)

statesCheck(dtgvFrame.tablecells[1], "TableCell", add_states=["selected",\
                            "selectable", "enabled", "editable", "sensitive"])
sleep(config.SHORT_DELAY)

statesCheck(dtgvFrame.checkboxes[0], "CheckBox", add_states=["selected",\
                                           "checked", "selectable","focused"])

# Selecting second combobox, and check it's state
dtgvFrame.treetable.selectChild(dtgvFrame.comboboxes[1].getIndexInParent())
sleep(config.SHORT_DELAY)

statesCheck(dtgvFrame.comboboxes[1], "ComboBox", add_states=["selected", \
                                                                "selectable"])
sleep(config.SHORT_DELAY)

statesCheck(dtgvFrame.checkboxes[0], "CheckBox", add_states=["selected",\
                                           "checked", "selectable","focused"])

# Selecting second combobox, and check it's state
dtgvFrame.treetable.selectChild(dtgvFrame.buttons[1].getIndexInParent())
sleep(config.SHORT_DELAY)

statesCheck(dtgvFrame.buttons[1], "Button", add_states=["selected",\
                                                                "selectable"])
sleep(config.SHORT_DELAY)

statesCheck(dtgvFrame.checkboxes[0], "CheckBox", add_states=["selected",\
                                           "checked", "selectable","focused"])

# BUG513837 - DataGridView:the "checked" check box lack the 'sensitive' 
# and 'enable' state
#statesCheck(dtgvFrame.treetable[5], "CheckBox", add_states=[ "checked",\
#                                                             "selectable"])

# BUG479126 - DataGridView. TextBox cells don't implement SelectionItem
##statesCheck(dtgvFrame.treetable[1], "TableCell", add_states=["selected",\
#                   "selectable"], invalid_states=["enabled", "sensitive"])

#check table's table implementation
dtgvFrame.assertTable(dtgvFrame.treetable, row=6, col=5)
sleep(config.SHORT_DELAY)

#close application frame window
dtgvFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR

