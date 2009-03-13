#!/usr/bin/env python

##
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        3/2/2008
# Description: Test accessibility of datagridcomboboxcolumn widget 
#              Use the datagridframe.py wrapper script
#              Test the samples/datagrid.py script
##

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
##states test 
#################

statesCheck(dgFrame.combobox_cells[0], "TabelCell")

statesCheck(dgFrame.combobox_cells[1], "TabelCell")

#####################################################################################
##AtkAction test, mouse click, key navigate to change label and text
#####################################################################################

#mouse click
dgFrame.combobox_cells[0].mouseClick()
sleep(config.SHORT_DELAY)
dgFrame.assertLabel("row:0 col:3 Value:Box0")

#key down
dgFrame.keyCombo("Down", grabFocus=False)
sleep(config.SHORT_DELAY)
dgFrame.assertLabel("row:1 col:3 Value:Box1")

#press/click action, states check for menu/menuitem
dgFrame.combobox_cells[2].press()
sleep(config.SHORT_DELAY)
menu = dgFrame.combobox_cells[2].findMenu(None)
statesCheck(menu, "Menu")
item_menuitems = menu.findAllMenuItems(re.compile("Item*"))
statesCheck(item_menuitems[0], "MenuItem")

dgFrame.click(item_menuitems[2])
sleep(config.SHORT_DELAY)
dgFrame.assertText(item_menuitems[2], "Item3")

##########################
##Text is uneditable
##########################

dgFrame.enterTextValue(dgFrame.combobox_cells[0], "uneditable", oldtext="Box0")

#close application frame window
dgFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
