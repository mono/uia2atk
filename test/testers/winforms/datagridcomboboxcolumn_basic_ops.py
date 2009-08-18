#!/usr/bin/env python

###############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        3/2/2008
# Description: Test accessibility of datagridcomboboxcolumn widget 
#              Use the datagridframe.py wrapper script
#              Test the samples/winforms/datagrid.py script
###############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of datagridcomboboxcolumn widget
"""

# imports
import sys
import os

from strongwind import *
from datagrid import *
from helpers import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the datagrid sample application
try:
  app = launchDataGrid(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
dgFrame = app.dataGridFrame

#################
# states test 
#################
# BUG480237: I think the cells should be accessible with ComboBox role with Menu
# and MenuItem children, so this test is not completed
#statesCheck(dgFrame.combobox_cells[0], "TableCell")

#statesCheck(dgFrame.combobox_cells[1], "TableCell")

####################################################################
# AtkAction test, mouse click, key navigate to change label and text
####################################################################

# mouse click
dgFrame.combobox_cells[0].mouseClick()
sleep(config.SHORT_DELAY)
dgFrame.assertLabel("row:0 col:3 Value:Box0")

# key down
dgFrame.keyCombo("Down", grabFocus=False)
sleep(config.SHORT_DELAY)
dgFrame.assertLabel("row:1 col:3 Value:Box1")

# press/click action, states check for menu/menuitem
# BUG480237: missing menu and menuitems
#dgFrame.combobox_cells[2].press()
#sleep(config.SHORT_DELAY)
#menu = dgFrame.combobox_cells[2].findMenu(None)
#statesCheck(menu, "Menu")
#item_menuitems = menu.findAllMenuItems(re.compile("Item*"))
#statesCheck(item_menuitems[0], "MenuItem")

#dgFrame.click(item_menuitems[2])
#sleep(config.SHORT_DELAY)
#dgFrame.assertText(combobox_cells[2], "Item3")

##########################
# Text is uneditable
##########################
# cells under combobox shouldn't be editable, I think it relate to BUG480237 
# it is accessbile with wrong role, if bug is valid, we also need update 
# *frame to find combobox
#dgFrame.assertUnEditableText(dgFrame.combobox_cells[0], is_implemented=False, expected_text="Box0")

#close application frame window
dgFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
