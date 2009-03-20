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
# check TableCell's AtkAction
############################
actionsCheck(lbFrame.tablecell[0], "TableCell")
actionsCheck(lbFrame.tablecell[1], "TableCell")
actionsCheck(lbFrame.tablecell[19], "TableCell")

############################
# check List's AtkAccessible
############################
statesCheck(lbFrame.treetable, "TreeTable", add_states=["focused"])

############################
# check TableCell's AtkAccessible
############################
#check TableCell0,1's default states
statesCheck(lbFrame.tablecell[0], "TableCell", add_states=["focused", "selected"])
statesCheck(lbFrame.tablecell[1], "TableCell")
statesCheck(lbFrame.tablecell[19], "TableCell")

lbFrame.tablecell[10].mouseClick()
sleep(config.SHORT_DELAY)
lbFrame.assertLabel('10')
statesCheck(lbFrame.tablecell[10], "TableCell", add_states=["focused", "selected"])

lbFrame.tablecell[19].mouseClick()
sleep(config.SHORT_DELAY)
lbFrame.assertLabel('19')
statesCheck(lbFrame.tablecell[19], "TableCell", add_states=["focused", "selected"])

############################
# check TableCell's AtkAction
############################
lbFrame.click(lbFrame.tablecell[0])
sleep(config.SHORT_DELAY)
statesCheck(lbFrame.tablecell[0], "TableCell", add_states=["focused", "selected"])
statesCheck(lbFrame.tablecell[19], "TableCell")

lbFrame.click(lbFrame.tablecell[9])
sleep(config.SHORT_DELAY)
statesCheck(lbFrame.tablecell[9], "TableCell", add_states=["focused", "selected"])

############################
# check List's AtkSelection
############################
#check first tablecell selection implementation
lbFrame.selectChild(lbFrame.treetable, 0)
sleep(config.SHORT_DELAY)
statesCheck(lbFrame.tablecell[0], "TableCell", add_states=["focused", "selected"])

#clear first tablecell selection
lbFrame.assertClearSelection(lbFrame.treetable)
sleep(config.SHORT_DELAY)
statesCheck(lbFrame.treetable, "TreeTable", add_states=["focused"])
statesCheck(lbFrame.tablecell[0], "TableCell", add_states=["focused"])

#check last tablecell selection implemention
lbFrame.selectChild(lbFrame.treetable, 19)
sleep(config.SHORT_DELAY)
statesCheck(lbFrame.tablecell[19], "TableCell", add_states=["focused", "selected"])

#clear last tablecell selection
lbFrame.assertClearSelection(lbFrame.treetable)
sleep(config.SHORT_DELAY)
statesCheck(lbFrame.treetable, "TreeTable", add_states=["focused"])
statesCheck(lbFrame.tablecell[19], "TableCell", add_states=["focused"])

############################
# check TableCell's AtkText
############################
#check tablecell's Text Value
lbFrame.assertText()

############################
# End
############################
#close application frame window
lbFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
