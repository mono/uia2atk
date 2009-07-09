#!/usr/bin/env python
# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
#              Ray Wang <rawang@novell.com>
# Date:        11/10/2008
# Description: main test script for combobox simple style
#              ../samples/combobox_simple.py is the test sample script
#              combobox_simple/* are the wrappers of combobox_simple test sample
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of combobox_simple widget
"""

# imports
from combobox_simple import *
from helpers import *
from actions import *
from states import *
from sys import argv

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the combobox_simple sample application
try:
  app = launchComboBox(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
cbFrame = app.comboBoxSimpleFrame

##############################
# check Combobox children's AtkAction
##############################
# check tablecell's actions
for tablecell in cbFrame.tablecells:
    actionsCheck(tablecell, "TableCell")

##############################
# check Combobox children's AtkAccessible
##############################
# check ComboBox's states list
# TODO: BUG483223, Extraneous "focuable" and "manages descendants" states, 
# and reluctant "focused" state
#statesCheck(cbFrame.combobox, "ComboBox")
# BUG505281 - ComboBox (DropDownStyle = Simple) text field has extraneous
# "selectable" state.
#statesCheck(cbFrame.textbox, "TextBox", add_states=["focused"])
# TODO: BUG483225, Extraneous "selectable" state, 
# missing "manages descendants" state, and reluctant "focused" state
#statesCheck(cbFrame.treetable, "TreeTable")
# TODO: BUG483226, missing 'focusable', 'transient' and  'single line' states
#for tablecell in cbFrame.tablecells:
#    statesCheck(tablecell, "TableCell")

##############################
# test AtkText
##############################
# check "text" for TableCells
for i, tablecell in enumerate(cbFrame.tablecells):
    cbFrame.assertText(tablecell, str(i))

# check TextBox's Text is editable
cbFrame.textbox.typeText("2")
sleep(config.SHORT_DELAY)
cbFrame.assertText(cbFrame.textbox, "2")

cbFrame.textbox.enterText("9")
sleep(config.SHORT_DELAY)
cbFrame.assertText(cbFrame.textbox, "9")

# check "tablecell"'s Text is not editable
cbFrame.editableTextIsUnimplemented(cbFrame.tablecells[0])
sleep(config.SHORT_DELAY)
cbFrame.assertText(cbFrame.tablecells[0], "0")

#############################
# test AtkAction
#############################
cbFrame.tablecells[5].click(log=True)
sleep(config.SHORT_DELAY)
cbFrame.assertText(cbFrame.textbox, "5")
cbFrame.assertLabel("You select 5")

cbFrame.tablecells[9].click(log=True)
sleep(config.SHORT_DELAY)
cbFrame.assertText(cbFrame.textbox, "9")
cbFrame.assertLabel("You select 9")

##############################
# check Combobox AtkSelection
##############################
# selectChild(0) is called from combobox
cbFrame.selectChild(cbFrame.combobox, 0)
sleep(config.SHORT_DELAY)
cbFrame.assertText(cbFrame.textbox, "0")
cbFrame.assertLabel("You select 0")
# BUG483225: tree table is missing "manages descendants" state, and reluctant 
# "focused" state
# BUG520567: treetable can be selected but is missing selectable state
#statesCheck(cbFrame.treetable, "TreeTable", \
#                      add_states=["selected", "selectable"])
# BUG506744: is missing focusable state
#statesCheck(cbFrame.tablecells[0], "TableCell", \
#                                     add_states=["focused", "selected"])

# selectChild(1) is called from combobox
cbFrame.selectChild(cbFrame.combobox, 1)
sleep(config.SHORT_DELAY)
cbFrame.assertText(cbFrame.textbox, "1")
cbFrame.assertLabel("You select 1")
statesCheck(cbFrame.textbox, "Text", \
                           add_states=["focused", "selected", "selectable"])
# BUG506744: is missing focusable state
#statesCheck(cbFrame.tablecells[1], "TableCell", \
#                                     add_states=["focused", "selected"])

# selectChild(2) is called from combobox
cbFrame.selectChild(cbFrame.combobox, 2)
sleep(config.SHORT_DELAY)
cbFrame.assertText(cbFrame.textbox, "2")
cbFrame.assertLabel("You select 2")
statesCheck(cbFrame.textbox, "Text", \
                           add_states=["focused", "selectable"])
# BUG506744: is missing focusable state
#statesCheck(cbFrame.tablecells[2], "TableCell", \
#                                     add_states=["focused", "selected"])

##############################
# check "treetable"'s AtkSelection
##############################
# comment all statesCheck due to BUG483226
# check AtkSelecton
cbFrame.selectChild(cbFrame.treetable, 0)
sleep(config.SHORT_DELAY)
cbFrame.assertText(cbFrame.textbox, "0")
cbFrame.assertLabel("You select 0")
#statesCheck(cbFrame.tablecells[0], "TableCell", \
#                                   add_states=["selected", "focused"])

cbFrame.selectChild(cbFrame.treetable, 9)
sleep(config.SHORT_DELAY)
cbFrame.assertText(cbFrame.textbox, "9")
cbFrame.assertLabel("You select 9")
#statesCheck(cbFrame.tablecells[9], "TableCell", add_states=["selected"])

cbFrame.tablecells[1].mouseClick()
sleep(config.SHORT_DELAY)
cbFrame.assertText(cbFrame.textbox, "1")
cbFrame.assertLabel("You select 1")
#statesCheck(cbFrame.tablecells[1], "TableCell", add_states=["selected"])

cbFrame.tablecells[8].mouseClick()
sleep(config.SHORT_DELAY)
cbFrame.assertText(cbFrame.textbox, "8")
cbFrame.assertLabel("You select 8")
#statesCheck(cbFrame.tablecells[8], "TableCell", add_states=["selected"])

# press Up/Down 
cbFrame.keyCombo("Up", grabFocus=False)
sleep(config.SHORT_DELAY)
cbFrame.assertText(cbFrame.textbox, "7")
cbFrame.assertLabel("You select 7")
#statesCheck(cbFrame.tablecells[7], "TableCell", add_states=["selected"])

cbFrame.keyCombo("Down", grabFocus=False)
sleep(config.SHORT_DELAY)
cbFrame.assertText(cbFrame.textbox, "8")
cbFrame.assertLabel("You select 8")
#statesCheck(cbFrame.tablecells[8], "TableCell", add_states=["selected"])

# press PageUp/PageDown
cbFrame.keyCombo("PageUp", grabFocus=False)
sleep(config.SHORT_DELAY)
cbFrame.assertText(cbFrame.textbox, "0")
cbFrame.assertLabel("You select 0")
#statesCheck(cbFrame.tablecells[0], "TableCell", add_states=["selected"])

cbFrame.keyCombo("PageDown", grabFocus=False)
sleep(config.SHORT_DELAY)
cbFrame.assertText(cbFrame.textbox, "9")
cbFrame.assertLabel("You select 9")
#statesCheck(cbFrame.tablecells[9], "TableCell", add_states=["selected"])

##############################
# check "treetable"'s AtkTable
##############################
cbFrame.assertTable(cbFrame.treetable, 10, 1)

##############################
# End
##############################
#close application frame window
cbFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
