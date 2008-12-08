#!/usr/bin/env python
# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        08/18/2008
# Description: main test script of listbox
#              ../samples/listbox.py is the test sample script
#              listbox/* is the wrapper of listbox test sample script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of listbox widget
"""
# imports
from listbox import *
from helpers import *
from states import *
from actions import *
from sys import argv

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the treeview sample application
try:
  app = launchListBox(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
lbFrame = app.listBoxFrame

############################
# check ListItem's AtkAction
############################
actionsCheck(lbFrame.listitem[0], "ListItem")

############################
# check List's AtkAccessible
############################
statesCheck(lbFrame.listbox, "List", add_states=["focused"])

############################
# check ListItem's AtkAccessible
############################
#check ListItem0,1's default states
statesCheck(lbFrame.listitem[0], "ListItem", add_states=["selected"])
statesCheck(lbFrame.listitem[1], "ListItem")

#mouse click ListItem to change label value
#lbFrame.mouseClick(log=False)
lbFrame.listitem[10].mouseClick()
sleep(config.SHORT_DELAY)
lbFrame.assertLabel('10')

lbFrame.listitem[19].mouseClick()
sleep(config.SHORT_DELAY)
lbFrame.assertLabel('19')

############################
# check ListItem's AtkAction
############################
#click action to select listitem0 to rise selected state
lbFrame.click(lbFrame.listitem[0])
sleep(config.SHORT_DELAY)
statesCheck(lbFrame.listitem[0], "ListItem", add_states=["selected"])
#listitem19 still focused but not selected
statesCheck(lbFrame.listitem[19], "ListItem")

############################
# check List's AtkSelection
############################
#check first listitem selection implementation
lbFrame.assertSelectionChild(lbFrame.listbox, 0)
sleep(config.SHORT_DELAY)
statesCheck(lbFrame.listitem[0], "ListItem", add_states=["selected"])

#clear first listitem selection
lbFrame.assertClearSelection(lbFrame.listbox)
sleep(config.SHORT_DELAY)
statesCheck(lbFrame.listitem[0], "ListItem")

#check last listitem selection implemention
lbFrame.assertSelectionChild(lbFrame.listbox, 19)
sleep(config.SHORT_DELAY)
statesCheck(lbFrame.listitem[19], "ListItem", add_states=["selected"])

#clear last listitem selection
lbFrame.assertClearSelection(lbFrame.listbox)
sleep(config.SHORT_DELAY)
statesCheck(lbFrame.listitem[19], "ListItem")

############################
# check ListItem's AtkText
############################
#check listitem's Text Value
lbFrame.assertText()

############################
# End
############################
#close application frame window
lbFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
