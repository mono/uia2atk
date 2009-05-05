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
for i in range(10):
    actionsCheck(cbFrame.tablecell[i], "TableCell")

##############################
# check Combobox children's AtkAccessible
##############################
# check ComboBox's states list
# TODO: BUG483223, Extraneous "focuable" and "manages descendants" states, 
# and reluctant "focused" state
#statesCheck(cbFrame.combobox, "ComboBox")
statesCheck(cbFrame.textbox, "TextBox", add_states=["selectable", "focused"])
# TODO: BUG483225, Extraneous "selectable" state, 
# missing "manages descendants" state, and reluctant "focused" state
#statesCheck(cbFrame.treetable, "TreeTable")
# TODO: BUG483236, missing 'focusable', 'transient' and  'single line' states
#for i in range(10):
#    statesCheck(cbFrame.tablecell[i], "TableCell")

##############################
# test AtkText
##############################
# check "text" for TableCells
for i in range(10):
    cbFrame.assertText(cbFrame.tablecell[i], str(i), actionlog=True)

# check TextBox's Text is editable
cbFrame.textbox.typeText("2")
sleep(config.SHORT_DELAY)
cbFrame.assertText(cbFrame.textbox, "2")

cbFrame.textbox.enterText("9")
sleep(config.SHORT_DELAY)
cbFrame.assertText(cbFrame.textbox, "9")

# check "tablecell"'s Text is not editable
cbFrame.editableTextIsUnimplemented(cbFrame.tablecell[0])
sleep(config.SHORT_DELAY)
cbFrame.assertText(cbFrame.tablecell[0], "0")

#############################
# test AtkAction
#############################
cbFrame.click(cbFrame.tablecell[5])
sleep(config.SHORT_DELAY)
cbFrame.assertText(cbFrame.textbox, "5")
cbFrame.assertLabel("You select 5")

cbFrame.click(cbFrame.tablecell[9])
sleep(config.SHORT_DELAY)
cbFrame.assertText(cbFrame.textbox, "9")
cbFrame.assertLabel("You select 9")

##############################
# check Combobox AtkSelection
##############################
# set index 0 to select text
#cbFrame.selectChild(cbFrame.combobox, 0)
#sleep(config.SHORT_DELAY)
## BUG488474, selectChild called the selection interface's selectChild
## method, which is not working.
#cbFrame.assertText(cbFrame.textbox, "0")
## doesn't rise 'selected' state for Menu and Text due to BUG456341
#statesCheck(cbFrame.textbox, "Text", add_states=["focused", "selected"])

##############################
# check "treetable"'s AtkSelection
##############################
# TODO: mark all statesCheck due to BUG483226
# check AtkSelecton
cbFrame.selectChild(cbFrame.treetable, 0)
sleep(config.SHORT_DELAY)
cbFrame.assertText(cbFrame.textbox, "0")
cbFrame.assertLabel("You select 0")
#statesCheck(cbFrame.tablecell[0], "TableCell", add_states=["selected"])

cbFrame.selectChild(cbFrame.treetable, 9)
sleep(config.SHORT_DELAY)
cbFrame.assertText(cbFrame.textbox, "9")
cbFrame.assertLabel("You select 9")
#statesCheck(cbFrame.tablecell[9], "TableCell", add_states=["selected"])

cbFrame.tablecell[1].mouseClick()
sleep(config.SHORT_DELAY)
cbFrame.assertText(cbFrame.textbox, "1")
cbFrame.assertLabel("You select 1")
#statesCheck(cbFrame.tablecell[1], "TableCell", add_states=["selected"])

cbFrame.tablecell[8].mouseClick()
sleep(config.SHORT_DELAY)
cbFrame.assertText(cbFrame.textbox, "8")
cbFrame.assertLabel("You select 8")
#statesCheck(cbFrame.tablecell[8], "TableCell", add_states=["selected"])

# press Up/Down 
cbFrame.keyCombo("Up", grabFocus=False)
sleep(config.SHORT_DELAY)
cbFrame.assertText(cbFrame.textbox, "7")
cbFrame.assertLabel("You select 7")
#statesCheck(cbFrame.tablecell[7], "TableCell", add_states=["selected"])

cbFrame.keyCombo("Down", grabFocus=False)
sleep(config.SHORT_DELAY)
cbFrame.assertText(cbFrame.textbox, "8")
cbFrame.assertLabel("You select 8")
#statesCheck(cbFrame.tablecell[8], "TableCell", add_states=["selected"])

# press PageUp/PageDown
cbFrame.keyCombo("PageUp", grabFocus=False)
sleep(config.SHORT_DELAY)
cbFrame.assertText(cbFrame.textbox, "0")
cbFrame.assertLabel("You select 0")
#statesCheck(cbFrame.tablecell[0], "TableCell", add_states=["selected"])

cbFrame.keyCombo("PageDown", grabFocus=False)
sleep(config.SHORT_DELAY)
cbFrame.assertText(cbFrame.textbox, "9")
cbFrame.assertLabel("You select 9")
#statesCheck(cbFrame.tablecell[9], "TableCell", add_states=["selected"])

# TODO: mark all statesCheck due to BUG483225
# check clear Selection
#cbFrame.clearSelection(cbFrame.treetable)
#sleep(config.SHORT_DELAY)
#statesCheck(cbFrame.tree, "TreeTable")

# TODO: mark all statesCheck due to BUG483223
#cbFrame.clearSelection(cbFrame.combobox)
#sleep(config.SHORT_DELAY)
#statesCheck(cbFrame.combobox, "ComboBox")

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
