#!/usr/bin/env python
# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        12/10/2008
# Description: main test script of toolstripcombobox
#              ../samples/toolstripcombobox.py is the test sample script
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
tscbFrame.press(tscbFrame.toolstripcombobox)

##############################
# check toolstripcombobox children's AtkAction
##############################
actionsCheck(tscbFrame.toolstripcombobox_menu, "Menu")
actionsCheck(tscbFrame.toolstripcombobox_menuitem_6, "MenuItem")
actionsCheck(tscbFrame.toolstripcombobox_menuitem_8, "MenuItem")
actionsCheck(tscbFrame.toolstripcombobox_menuitem_10, "MenuItem")
actionsCheck(tscbFrame.toolstripcombobox_menuitem_12, "MenuItem")
actionsCheck(tscbFrame.toolstripcombobox_menuitem_14, "MenuItem")

##############################
# check toolstripcombobox's AtkAccessible
##############################
statesCheck(tscbFrame.toolstripcombobox, "ComboBox")
statesCheck(tscbFrame.toolstripcombobox_menu, "Menu")
statesCheck(tscbFrame.toolstripcombobox_menuitem_6, "MenuItem")
statesCheck(tscbFrame.toolstripcombobox_menuitem_8, "MenuItem")
statesCheck(tscbFrame.toolstripcombobox_menuitem_10, "MenuItem")
statesCheck(tscbFrame.toolstripcombobox_menuitem_12, "MenuItem")
statesCheck(tscbFrame.toolstripcombobox_menuitem_14, "MenuItem")
sleep(config.SHORT_DELAY)

# select item from combobox
tscbFrame.assertSelectionChild(tscbFrame.toolstripcombobox_menuitem_6, 0)
sleep(config.SHORT_DELAY)
statesCheck(tscbFrame.toolstripcombobox_menuitem_6, "MenuItem", add_states=["selected"])

# select the last item from combobox
tscbFrame.assertSelectionChild(tscbFrame.toolstripcombobox_menuitem_14, 0)
sleep(config.SHORT_DELAY)
statesCheck(tscbFrame.toolstripcombobox_menuitem_14, "MenuItem", add_states=["selected"])

##############################
# check toolstripcombobox's AtkSelection
##############################
tscbFrame.assertSelectionChild(tscbFrame.toolstripcombobox_menuitem_6, 0)
sleep(config.SHORT_DELAY)
tscbFrame.assertText(tscbFrame.label, "The font size is 6")

# select the last item from combobox
tscbFrame.assertSelectionChild(tscbFrame.toolstripcombobox_menuitem_14, 4)
sleep(config.SHORT_DELAY)
tscbFrame.assertText(tscbFrame.label, "The font size is 14")

##############################
# End
##############################
# close application frame window
tscbFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
