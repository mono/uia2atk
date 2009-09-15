#!/usr/bin/env python
# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        12/10/2008
# Description: main test script of toolstripcombobox
#              ../samples/winforms/toolstripcombobox.py is the test sample script
#              toolstripcombobox/* are the wrappers of toolstripcombobox test sample 
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of ToolStripComboBox widget
"""

# imports
from toolstripcombobox import *
from helpers import *
from actions import *
from states import *
from sys import argv

app_path = None 
try:
    app_path = argv[1]
except IndexError:
    pass #expected

# open the toolstripcombobox sample application
try:
    app = launchToolStripComboBox(app_path)
except IOError, msg:
    print "ERROR:  %s" % msg
    exit(2)

# make sure we got the app back
if app is None:
    exit(4)

# just an alias to make things shorter
tscbFrame = app.toolStripComboBoxFrame

#############################################
# check toolstripcombobox elements' AtkAction
#############################################
actionsCheck(tscbFrame.toolstripcombobox, "ComboBox")
actionsCheck(tscbFrame.menuitem_6, "MenuItem")
actionsCheck(tscbFrame.menuitem_8, "MenuItem")
actionsCheck(tscbFrame.menuitem_10, "MenuItem")
actionsCheck(tscbFrame.menuitem_12, "MenuItem")
actionsCheck(tscbFrame.menuitem_14, "MenuItem")

##################################################
# check toolstripcombobox elements' default states
##################################################
statesCheck(tscbFrame.toolbar, "ToolBar")
statesCheck(tscbFrame.toolstripcombobox, "ComboBox")
# BUG501269: visible state should be exist
#statesCheck(tscbFrame.menu, "Menu", invalid_states=["showing"])
# BUG512261: ToolStripCombBox menu items receive "focused" state, but do not have "focusable" state
#statesCheck(tscbFrame.menuitem_6, "MenuItem", invalid_states=["showing"])
#statesCheck(tscbFrame.menuitem_8, "MenuItem", add_states=["selected","focused"])
#statesCheck(tscbFrame.menuitem_10, "MenuItem", invalid_states=["showing"])
#statesCheck(tscbFrame.menuitem_12, "MenuItem", invalid_states=["showing"])
#statesCheck(tscbFrame.menuitem_14, "MenuItem", invalid_states=["showing"])

###############################################################
# check toolstripcombobox elements' states after press combobox
###############################################################
# expand combobox
tscbFrame.toolstripcombobox.press(log=True)
sleep(config.SHORT_DELAY)
statesCheck(tscbFrame.toolbar, "ToolBar")
statesCheck(tscbFrame.toolstripcombobox, "ComboBox")
statesCheck(tscbFrame.menu, "Menu")
# BUG512261: ToolStripCombBox menu items receive "focused" state, but do not have "focusable" state
#statesCheck(tscbFrame.menuitem_6, "MenuItem")
#statesCheck(tscbFrame.menuitem_8, "MenuItem", add_states=["selected","focused"])
#statesCheck(tscbFrame.menuitem_10, "MenuItem")
#statesCheck(tscbFrame.menuitem_12, "MenuItem")
#statesCheck(tscbFrame.menuitem_14, "MenuItem")

###########################
# check elements' AtkAction
###########################
tscbFrame.menuitem_6.click(log=True)
sleep(config.SHORT_DELAY)
tscbFrame.assertText(tscbFrame.label, "The font size is 6")
# BUG512261: ToolStripCombBox menu items receive "focused" state, but do not have "focusable" state
#statesCheck(tscbFrame.menuitem_6, "MenuItem", add_states=["selected","focused"])

tscbFrame.menuitem_14.click(log=True)
sleep(config.SHORT_DELAY)
tscbFrame.assertText(tscbFrame.label, "The font size is 14")
# BUG512261: ToolStripCombBox menu items receive "focused" state, but do not have "focusable" state
#statesCheck(tscbFrame.menuitem_14, "MenuItem", add_states=["selected","focused"])

tscbFrame.menuitem_8.click(log=True)
sleep(config.SHORT_DELAY)
tscbFrame.assertText(tscbFrame.label, "The font size is 8")
# BUG512261: ToolStripCombBox menu items receive "focused" state, but do not have "focusable" state
#statesCheck(tscbFrame.menuitem_8, "MenuItem", add_states=["selected","focused"])

# contract combobox
tscbFrame.toolstripcombobox.press(log=True)
sleep(config.SHORT_DELAY)
## BUG501269: visible state should be exist
#statesCheck(tscbFrame.menu, "Menu", invalid_states=["showing"])

########################################################################
# mouseClick and keyCombo navigation to check states and label's changed
########################################################################
tscbFrame.toolstripcombobox.mouseClick()
sleep(config.SHORT_DELAY)
tscbFrame.keyCombo("Up", grabFocus=False)
sleep(config.SHORT_DELAY)
tscbFrame.assertText(tscbFrame.label, "The font size is 6")
# BUG512261: ToolStripCombBox menu items receive "focused" state, but do not have "focusable" state
#statesCheck(tscbFrame.menuitem_6, "MenuItem", add_states=["selected","focused"])
#statesCheck(tscbFrame.menuitem_8, "MenuItem")

tscbFrame.keyCombo("Down", grabFocus=False)
sleep(config.SHORT_DELAY)
tscbFrame.assertText(tscbFrame.label, "The font size is 8")
# BUG512261: ToolStripCombBox menu items receive "focused" state, but do not have "focusable" state
#statesCheck(tscbFrame.menuitem_8, "MenuItem", add_states=["selected","focused"])
#statesCheck(tscbFrame.menuitem_6, "MenuItem")

tscbFrame.menuitem_10.mouseClick()
sleep(config.SHORT_DELAY)
tscbFrame.assertText(tscbFrame.label, "The font size is 10")
# BUG512261: ToolStripCombBox menu items receive "focused" state, but do not have "focusable" state
#statesCheck(tscbFrame.menuitem_6, "MenuItem", invalid_states=["showing"])
#statesCheck(tscbFrame.menuitem_10, "MenuItem", add_states=["selected","focused"])

tscbFrame.toolstripcombobox.mouseClick()
sleep(config.SHORT_DELAY)
tscbFrame.menuitem_12.mouseClick()
sleep(config.SHORT_DELAY)
tscbFrame.assertText(tscbFrame.label, "The font size is 12")
# BUG512261: ToolStripCombBox menu items receive "focused" state, but do not have "focusable" state
#statesCheck(tscbFrame.menuitem_12, "MenuItem", add_states=["selected","focused"])
#statesCheck(tscbFrame.menuitem_10, "MenuItem", invalid_states=["showing"])

##############################
# check toolstripcombobox's AtkSelection
##############################
# select item from combobox
tscbFrame.selectChild(tscbFrame.menu, 0)
sleep(config.SHORT_DELAY)
tscbFrame.assertText(tscbFrame.label, "The font size is 6")
# BUG512261: ToolStripCombBox menu items receive "focused" state, but do not have "focusable" state
#statesCheck(tscbFrame.menuitem_6, "MenuItem", add_states=["selected", "focused"])
#statesCheck(tscbFrame.menuitem_10, "MenuItem", invalid_states=["showing"])

# select the last item from combobox
tscbFrame.selectChild(tscbFrame.menu, 4)
sleep(config.SHORT_DELAY)
tscbFrame.assertText(tscbFrame.label, "The font size is 14")
# BUG512261: ToolStripCombBox menu items receive "focused" state, but do not have "focusable" state
#statesCheck(tscbFrame.menuitem_14, "MenuItem", add_states=["selected", "focused"])
#statesCheck(tscbFrame.menuitem_6, "MenuItem", invalid_states=["showing"])

##############################
# check menu item's AtkText 
##############################
tscbFrame.editableTextIsNotImplemented(tscbFrame.menuitem_6)
sleep(config.SHORT_DELAY)
tscbFrame.assertText(tscbFrame.menuitem_6, "6")

tscbFrame.editableTextIsNotImplemented(tscbFrame.menuitem_14)
sleep(config.SHORT_DELAY)
tscbFrame.assertText(tscbFrame.menuitem_14, "14")

##############################
# End
##############################
# close application frame window
tscbFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
