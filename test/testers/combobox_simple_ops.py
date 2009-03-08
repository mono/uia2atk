#!/usr/bin/env python
# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
#              Ray Wang <rawang@novell.com>
# Date:        11/10/2008
# Description: main test script of toolstripcombobox
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
#check tablecell's actions
#for i in range(10):
#    actionsCheck(cbFrame.tablecell[i], "TableCell")

##############################
# check Combobox children's AtkAccessible
##############################
#check ComboBox's states list
# TODO: BUG483233, Extraneous "focuable" and "manages descendants" states, 
# and reluctant "focused" state
#statesCheck(cbFrame.combobox, "ComboBox")
# TODO: BUG483234, Extraneous "focuable" state
#statesCheck(cbFrame.textbox, "TextBox", add_states=["focused"])
# TODO: BUG483235, Extraneous "selectable" state, 
# missing "manages descendants" state, and reluctant "focused" state
#statesCheck(cbFrame.tree, "TreeTable")
# TODO: BUG483236, missing 'focusable', 'transient' and  'single line' states
#for i in range(10):
#    statesCheck(cbFrame.tablecell[i], "TableCell")

##############################
# check Combobox AtkSelection
##############################
# TODO: put some tests here

##############################
# check "text" in combo box
##############################
# TODO: put some tests for Screamable Content here

# check text's "Text(Editable)"
cbFrame.textbox.typeText("2")
sleep(config.SHORT_DELAY)
cbFrame.assertText(cbFrame.textbox, "2")

cbFrame.textbox.enterText("9")
sleep(config.SHORT_DELAY)
cbFrame.assertText(cbFrame.textbox, "9")

##############################
# check "treetable"'s AtkSelection and AtkTable
##############################
# TODO: mark all statesCheck due to BUG483236
# check AtkSelecton
cbFrame.selectChild(cbFrame.tree, 0)
sleep(config.SHORT_DELAY)
cbFrame.assertText(cbFrame.textbox, "0")
#statesCheck(cbFrame.tablecell[0], "TableCell", add_states=["selected"])

cbFrame.selectChild(cbFrame.tree, 9)
sleep(config.SHORT_DELAY)
cbFrame.assertText(cbFrame.textbox, "9")
#statesCheck(cbFrame.tablecell[9], "TableCell", add_states=["selected"])

cbFrame.tablecell[1].mouseClick()
sleep(config.SHORT_DELAY)
cbFrame.assertText(cbFrame.textbox, "1")
#statesCheck(cbFrame.tablecell[1], "TableCell", add_states=["selected"])

cbFrame.tablecell[8].mouseClick()
sleep(config.SHORT_DELAY)
cbFrame.assertText(cbFrame.textbox, "8")
#statesCheck(cbFrame.tablecell[8], "TableCell", add_states=["selected"])

# press Up/Down 
cbFrame.keyCombo("Up", grabFocus=False)
sleep(config.SHORT_DELAY)
cbFrame.assertText(cbFrame.textbox, "7")
#statesCheck(cbFrame.tablecell[7], "TableCell", add_states=["selected"])

cbFrame.keyCombo("Down", grabFocus=False)
sleep(config.SHORT_DELAY)
cbFrame.assertText(cbFrame.textbox, "8")
#statesCheck(cbFrame.tablecell[8], "TableCell", add_states=["selected"])

# press PageUp/PageDown
cbFrame.keyCombo("PageUp", grabFocus=False)
sleep(config.SHORT_DELAY)
cbFrame.assertText(cbFrame.textbox, "0")
#statesCheck(cbFrame.tablecell[0], "TableCell", add_states=["selected"])

cbFrame.keyCombo("PageDown", grabFocus=False)
sleep(config.SHORT_DELAY)
cbFrame.assertText(cbFrame.textbox, "9")
#statesCheck(cbFrame.tablecell[9], "TableCell", add_states=["selected"])

# TODO: mark all statesCheck due to BUG483235
# check clear Selection
cbFrame.clearSelection(cbFrame.tree)
sleep(config.SHORT_DELAY)
#statesCheck(cbFrame.tree, "TreeTable")

# TODO: mark all statesCheck due to BUG483233
cbFrame.clearSelection(cbFrame.combobox)
sleep(config.SHORT_DELAY)
#statesCheck(cbFrame.combobox, "ComboBox")

# check AtkTable
cbFrame.assertTable(cbFrame.tree, 10, 1)

##############################
# check "tablecell"'s AtkText
##############################
cbFrame.inputText(cbFrame.tablecell[0], "10")
sleep(config.SHORT_DELAY)
cbFrame.assertText(cbFrame.tablecell[0], "0")

##############################
# End
##############################
#close application frame window
cbFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
