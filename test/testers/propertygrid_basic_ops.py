#!/usr/bin/env python

#########################################################################
# Written by:  Felica Mu <fxmu@novell.com>
#              Calen Chen <cachen@novell.com>
# Date:        06/25/2009
# Description: Test accessibility of PropertyGrid widget 
#              Use the propertygridframe.py wrapper script
#              Test the samples/propertygrid.py script
#########################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of PropertyGrid widget
"""

# imports
import sys
import os

from strongwind import *
from propertygrid import *
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
	app = launchPropertyGrid(app_path)
except IOError, msg:
	print "ERROR:  %s" % msg
	exit(2)

# make sure we got the app back
if app is None:
	exit(4)

# Alias to make things shorter
pgFrame = app.propertyGridFrame

######################################
# check the states of all accessibles
######################################

# check tree table states
statesCheck(pgFrame.tree_table, "TreeTable",)

# check table cells states, the selected cell should include selected and 
# focused states
# BUB516376 - PropertyGrid: tree table cell lack the state of 'focusable'
# BUG519139: missing focused states
#for table_cell in pgFrame.all_table_cells:
#    if table_cell.selected:
#        statesCheck(table_cells, "TreeViewTableCell", add_states=["selected", 
#                                                                     "focused"]
#    else:
#        statesCheck(table_cells, "TreeViewTableCell")

# check vscrollbar states
statesCheck(pgFrame.vscrollbar, "VScrollBar")

# check toolbar states
statesCheck(pgFrame.toolbar, "ToolBar")

# check toggle button states
# the toggle buttons on the tool bar should not have focusable state, 
# because it can't be focused
# BUG519507: implemented wrong role and missing armed checked states
#statesCheck(pgFrame.categorized_button, "Button", invalid_states=['focusable'])
#statesCheck(pgFrame.alphabetic_button, "Button", invalid_states=['focusable'])
#statesCheck(pgFrame.property_button, "Button", \
#                           invalid_states=['focusable','sensitive','enabled'])

# check separator states
statesCheck(pgFrame.separator_style, "ToolStripSeparator")

# check split pane states
statesCheck(pgFrame.split_pane, "Splitter", add_states=['vertical'], 
                                                 invalid_states=['focusable'])

# check labels states
for label in pgFrame.labels:
    statesCheck(label, "Label")

######################################
#check the actions list of all accessibles
######################################

# check child table cells actions 
for table_cells in pgFrame.child_table_cells:
    actionsCheck(table_cells, "TreeViewTableCell")

# check parent table cells actions
# BUG516398 - PropertyGrid :TreeView table cells with children should have 
# "expand or contract" action
#for table_cells in pgFrame.parent_table_cells:
#     actionsCheck(table_cells, "TreeViewTableCell", 
#                                            add_actions=['expand or contract'])

# check toggle button actions
# BUG519507: implemented wrong role and missing armed checked states
#actionsCheck(pgFrame.categorized_button, "Button")
#actionsCheck(pgFrame.alphabetic_button, "Button")
# BUG516425 - PropertyGrid :The push button without 'sensitive' and 'enable' 
# states should have not "click" action
#actionsCheck(pgFrame.property_button, "Button", invalid_actions=['click'])

########################################
# check the Component of accessibles
########################################
# mouseClick the showing table cell to make sure its position and size 
# is correct
# BUG519711: mouseClick table cell on UI will rebuilded table cells, so we will
# get table_cell with 'defunct'
#for table_cell in pgFrame.all_table_cells:
#    if table_cell.showing:
#        table_cell.mouseClick()
#        sleep(config.SHORT_DELAY)
        # BUG516376: missing focusable state
        #statesCheck(table_cell, "TreeViewTableCell", \
        #                add_states=["selected", "focused"])
         

# check ToolBarButtons
# BUG519507: implemented wrong role and missing armed checked states
'''
pgFrame.alphabetic_button.mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(pgFrame.alphabetic_button, "Button", \
                  add_states=["armed", "checked"], invalid_states=["focusable"])

pgFrame.categorized_button.mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(pgFrame.categorized_button, "Button", \
                  add_states=["armed", "checked"], invalid_states=["focusable"])
'''

###########################
# test Click Action
###########################
# check ToolBarButtons 
# BUG519507: implemented wrong role and missing armed checked states
'''
pgFrame.alphabetic_button.click(log=True)
sleep(config.SHORT_DELAY)
statesCheck(pgFrame.alphabetic_button, "Button", \
                  add_states=["armed", "checked"], invalid_states=["focusable"])

pgFrame.categorized_button.click(log=True)
sleep(config.SHORT_DELAY)
statesCheck(pgFrame.categorized_button, "Button", \
                  add_states=["armed", "checked"], invalid_states=["focusable"])
'''
# click table_cell should raise selected and focused states, also 
# help_title_label on help panel should show the cells name
# BUG519099: click action doesn't work for cells
'''
for table_cell in pgFrame.all_table_cells:
    table_cell.click(log=True)
    sleep(config.SHORT_DELAY)
    # if BUG519412 is solved this test should be uncommented
    #pgFrame.assertText(pgFrame.help_title_label, table_cell.name)
    statesCheck(table_cell, "TreeViewTableCell", \
                        add_states=["selected", "focused"])
'''

######################################
# expand or contract the treeview 
######################################

# BUG516398 - PropertyGrid :TreeView table cells with children 
# should have "expand or contract" action, the 'expandOrContract' method
# doesn't work
# pgFrame.testExpandOrContract("Font", "Bold", is_expand=True)

pgFrame.testExpandOrContract("Appearance", "BackColor", is_expand=False)
pgFrame.testExpandOrContract("Appearance", "BackColor", is_expand=True)


#######################################################
#test context menu item's state
#######################################################
pgFrame.toolbar.mouseClick(button=3)
sleep(config.SHORT_DELAY)
pgFrame.findContextMenuAccessibles()

# check default states for menu_items
statesCheck(pgFrame.context_window, "ContextMenuStrip", add_states=["active"])
statesCheck(pgFrame.reset_menu_item, "MenuItem", add_states=["focusable"])
# BUG520153 - PropertyGrid: menu items with '-' text on ContexMenu 
# should not have 'focusable' state and 'selectable' state
#statesCheck(pgFrame.dash_menu_item, "MenuItem", invalid_states=["selectable"])
statesCheck(pgFrame.description_menu_item, "MenuItem", \
                                         add_states=["focusable","checked"])

# key press navigation should raise 'focused' and selected', except for 
# dash_menu_item
pgFrame.keyCombo("Down")
sleep(config.SHORT_DELAY)
# BUG520133: menu items on ContexMenu can't raise 'focused' and 'selected' state
#statesCheck(pgFrame.reset_menu_item, "MenuItem", \
#                                      add_states=["focusable","focused"])
# BUG 520153: extraneous selectable and focusable
#statesCheck(pgFrame.dash_menu_item, "MenuItem", invalid_states=["selectable"])
statesCheck(pgFrame.description_menu_item, "MenuItem", \
                                          add_states=["focusable","checked"])

pgFrame.keyCombo("Down")
sleep(config.SHORT_DELAY)
statesCheck(pgFrame.reset_menu_item, "MenuItem", add_states=["focusable"])
# BUG 520153: extraneous selectable and focusable
#statesCheck(pgFrame.dash_menu_item, "MenuItem", add_states=["selectable"])
# BUG520133: menu items on ContexMenu can't raise 'focused' and 'selected' state
#statesCheck(pgFrame.description_menu_item, "MenuItem", \
#                               add_states=["focusable","checked","focused"])

# close ContextMenu for next test
pgFrame.mouseClick(log=False)
sleep(config.SHORT_DELAY)

# check the function of menu_items
# find the first table_cell that is named ""
accessible = pgFrame.findTableCell("")
sleep(config.SHORT_DELAY)
# update the Text for accessible
pgFrame.changeText(accessible, "New Text")
sleep(config.SHORT_DELAY)
# reset the Text to ""
pgFrame.testReset(accessible, "")

# find 'Default' cell and update the text to 'Sound', then reset it to 'Default'
accessible = pgFrame.findTableCell("Default")
pgFrame.changeText(accessible, "Sound")
sleep(config.SHORT_DELAY)
pgFrame.testReset(accessible, "Default")

# uncheck the Description menu_item
# BUG521003: click action doesn't work 
#pgFrame.testDescription(is_checked=False)

# check it again
#pgFrame.testDescription(is_checked=True)

######################################################
# test the AtkText
######################################################
for table_cell in pgFrame.all_table_cells:
    # parent tree cell is uneditable
    if table_cell.name == "Appearance":
        pgFrame.changeText(table_cell,"Control")
        pgFrame.assertText(table_cell,"Appearance")
    # cells in first column is uneditable
    elif table_cell.name == "AccessibleRole":
        pgFrame.changeText(table_cell,"Control")
        pgFrame.assertText(table_cell,"AccessibleRole")
    # cells in second column and not empty that is uneditable for invalid value
    elif table_cell.name == "Default":
        pgFrame.changeText(table_cell,"Control")
        pgFrame.assertText(table_cell,"Default")
        # but is editable for valid value
        pgFrame.changeText(table_cell,"Sound")
        pgFrame.assertText(table_cell,"Sound")
    else:
        pass

#####################################################
# test AtkSelection for TreeTable
#####################################################
# BUB516376 - PropertyGrid: tree table cell lack the state of 'focusable'
# BUG519139: missing focused states
# if the two bug is fixed please uncomment the following
# BUG519711: TableCells shouldn't be rebuilded if click cell, after it's fixed 
# pgFrame.refreshTreeTable() can be removed from below tests
pgFrame.refreshTreeTable()
pgFrame.selectChild(pgFrame.all_table_cells[0].getIndexInParent())
sleep(config.SHORT_DELAY)
pgFrame.refreshTreeTable()
#statesCheck(pgFrame.all_table_cells[0],"TreeViewTableCell", 
#                                      add_states=["focused", "selected"])

pgFrame.selectChild(pgFrame.all_table_cells[1].getIndexInParent())
sleep(config.SHORT_DELAY)
pgFrame.refreshTreeTable()
#statesCheck(pgFrame.all_table_cells[1],"TreeViewTableCell", 
#                                       add_states=["focused", "selected"])

pgFrame.selectChild(pgFrame.all_table_cells[2].getIndexInParent())
sleep(config.SHORT_DELAY)
pgFrame.refreshTreeTable()
#statesCheck(pgFrame.all_table_cells[2],"TreeViewTableCell", \
#                                       add_states=["focused", "selected"])

# TODO: add tests for combobox and its menuitems if BUG519412 fixed

###########################################
# assert Image of accessibles
###########################################

# check toggle button' Image Size
# after BUG519507 fixed, these tests should be uncomment 
#pgFrame.assertImageSize(pgFrame.categorized_button, 16, 16 )

#pgFrame.assertImageSize(pgFrame.alphabetic_button, 16, 16 )

#pgFrame.assertImageSize(pgFrame.property_button, 16, 16 )

###########################################
# check the change of split pane
###########################################
# there is a log BUG515936 - PropertyGrid: move the split 
# on Gui will crash the application", but when change the split's pos,
# the app won't crash.
# BUG519532: set value will move the splitter to contrary way
'''
# value to minimum
pgFrame.testSpitterValue(pgFrame.split_pane._accessible.queryValue().minimumValue, \
                                                    treetable_is_enlarged=False)

# value to maximum
pgFrame.testSpitterValue(pgFrame.split_pane._accessible.queryValue().maximumValue, \
                                                     treetable_is_enlarged=True)

# set value to 60
pgFrame.testSpitterValue(60, treetable_is_enlarged=False)
'''

# TODO: add tests for these two bugs
# BUG479142 - No Relations to Parent in the table cells shown by PropertyGrid
# BUG513097 - PropertyGrid: characters in EditableText of TableCells 
# can't be deleted by "Backspace" key

#close application frame window
pgFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
