#!/usr/bin/env python
# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Sandy Armstrong <saarmstrong@novell.com>
#              Calen Chen <cachen@novell.com>
# Date:        02/23/2009
# Description: Test accessibility of monthcalendar widget 
#              Use the monthcalendarframe.py wrapper script
#              Test the samples/winforms/monthcalendar_label_linklabel.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of monthcalendar widget
"""

# imports
import sys
import os

from strongwind import *
from monthcalendar import *
from helpers import *
from sys import argv
from os import path

#config.WATCHDOG_TIMEOUT=3600

app_path = None 
try:
    app_path = argv[1]
except IndexError:
    pass #expected

# open the MonthCalendar sample application
try:
    app = launchMonthCalendar(app_path)
except IOError, msg:
    print "ERROR:  %s" % msg
    exit(2)

# make sure we got the app back
if app is None:
    exit(4)

# just an alias to make things shorter
mcFrame = app.monthCalendarFrame

####################
# test actions
####################
actionsCheck(mcFrame.back_button, "Button")
actionsCheck(mcFrame.forward_button, "Button")
# BUG516725: column_header is missing click action
#for column_header in mcFrame.column_headers:
#    actionsCheck(column_header, "TableColumnHeader")
for r in xrange(6):
    for c in xrange(7):
        actionsCheck(mcFrame.table_cells[r][c], "TableCell")

####################
# test states
####################
# BUG517218: push buttons have extraneous focused and selectable states
#statesCheck(mcFrame.back_button, "Button")
#statesCheck(mcFrame.forward_button, "Button")
statesCheck(mcFrame.tree_table, "TreeTable", add_states=["focused"])
# BUG517233: column headers have extraneous 'focused' and 'focusable' states
#for columnheader in mcFrame.column_headers:
#    statesCheck(columnheader, "TableColumnHeader")

# NOTE: Sample initializes to 1/23/2008
# BUG517682: MonthCalendar: TableCells have extraneous focused state
#mcFrame.testTableCellStates(day_table_cell=23)

##############################
# test Action and navigation
##############################
# click next (24) to ensure label is updated and name of tree_table is updated
# call dayTableCell method to get the accessible of day 24
click_day_table_cell = mcFrame.dayTableCell("24")
click_day_table_cell.click(log=True)
sleep(config.SHORT_DELAY)
mcFrame.assertText(mcFrame.label, "Your selection is:\n2008-01-24")
mcFrame.testTableCellStates(day_table_cell=24)
# BUG517682:TableCells have extraneous focused state
#mcFrame.testTableCellStates(day_table_cell=23, is_selected=False)

# TODO: Test that name of top-level Filler accessible is always date string 
# (BNC 479130, 479125)
mcFrame.assertName(mcFrame.monthcalendar_filler, "1/24/2008")

# click forward month
mcFrame.forward_button.click(log=True)
sleep(config.SHORT_DELAY)
mcFrame.findAccessiblesFromTreeTable()
mcFrame.assertName(mcFrame.table_cells[0][0], "27")

sleep(config.SHORT_DELAY)
mcFrame.testTableCellStates(day_table_cell=24)
# BUG517682:TableCells have extraneous focused state
#mcFrame.testTableCellStates(day_table_cell=23, is_selected=False)

# click back month
# after BUG520122 is fixed, next line should be moved
mcFrame.back_button = mcFrame.findPushButton('Back by one month')
mcFrame.back_button.click(log=True)
sleep(config.SHORT_DELAY)
mcFrame.findAccessiblesFromTreeTable()
sleep(config.SHORT_DELAY)
mcFrame.assertName(mcFrame.table_cells[0][0], "30")

sleep(config.SHORT_DELAY)
mcFrame.testTableCellStates(day_table_cell=24)

# click date in next month
mcFrame.table_cells[-1][-1].click(log=True) # bottom-right cell
sleep(config.MEDIUM_DELAY)
mcFrame.findAccessiblesFromTreeTable()
sleep(config.SHORT_DELAY)
mcFrame.assertName(mcFrame.table_cells[0][0], "27")
sleep(config.SHORT_DELAY)
mcFrame.testTableCellStates(day_table_cell=9)

# click date in previous month
mcFrame.table_cells[0][0].click(log=True) # top-left cell
sleep(config.MEDIUM_DELAY)
mcFrame.findAccessiblesFromTreeTable()
sleep(config.SHORT_DELAY)
mcFrame.assertName(mcFrame.table_cells[0][0], "30")
sleep(config.SHORT_DELAY)
mcFrame.testTableCellStates(day_table_cell=27)

# up/down/left/right keypresses (within a month)
mcFrame.keyCombo("Up")
sleep(config.SHORT_DELAY)
mcFrame.assertText(mcFrame.label, "Your selection is:\n2008-01-20")
mcFrame.testTableCellStates(day_table_cell=20)

mcFrame.keyCombo("Down")
sleep(config.SHORT_DELAY)
mcFrame.assertText(mcFrame.label, "Your selection is:\n2008-01-27")
mcFrame.testTableCellStates(day_table_cell=27)

mcFrame.keyCombo("Left")
sleep(config.SHORT_DELAY)
mcFrame.assertText(mcFrame.label, "Your selection is:\n2008-01-26")
mcFrame.testTableCellStates(day_table_cell=26)

mcFrame.keyCombo("Right")
sleep(config.SHORT_DELAY)
mcFrame.assertText(mcFrame.label, "Your selection is:\n2008-01-27")
mcFrame.testTableCellStates(day_table_cell=27)

mcFrame.keyCombo("Right")
sleep(config.SHORT_DELAY)
mcFrame.assertText(mcFrame.label, "Your selection is:\n2008-01-28")
mcFrame.testTableCellStates(day_table_cell=28)

# up/down/left/right keypresses (between months)
mcFrame.keyCombo("Down")
sleep(config.MEDIUM_DELAY)
mcFrame.findAccessiblesFromTreeTable()
mcFrame.assertText(mcFrame.label, "Your selection is:\n2008-02-04")
mcFrame.assertName(mcFrame.table_cells[0][0], "27")
sleep(config.SHORT_DELAY)
mcFrame.testTableCellStates(day_table_cell=4)

for x in range(4):
    mcFrame.keyCombo("Left")
sleep(config.MEDIUM_DELAY)
mcFrame.findAccessiblesFromTreeTable()
mcFrame.assertText(mcFrame.label, "Your selection is:\n2008-01-31")
mcFrame.assertName(mcFrame.table_cells[0][0], "30")
sleep(config.SHORT_DELAY)
# BUG517682: MonthCalendar: TableCells have extraneous focused state
#mcFrame.testTableCellStates(day_table_cell=31)

# page-up, -down
mcFrame.keyCombo("PageDown")
sleep(config.MEDIUM_DELAY)
mcFrame.findAccessiblesFromTreeTable()
mcFrame.assertText(mcFrame.label, "Your selection is:\n2008-02-29")
mcFrame.assertName(mcFrame.table_cells[0][0], "27")
sleep(config.SHORT_DELAY)
# BUG517682: MonthCalendar: TableCells have extraneous focused state
#mcFrame.testTableCellStates(day_table_cell=29, firstReached=False) # Because there is no Feb 31st
#mcFrame.testTableCellStates(day_table_cell=15, is_selected=False)

mcFrame.keyCombo("PageUp")
sleep(config.MEDIUM_DELAY)
mcFrame.findAccessiblesFromTreeTable()
mcFrame.assertText(mcFrame.label, "Your selection is:\n2008-01-29")
mcFrame.assertName(mcFrame.table_cells[0][0], "30")
sleep(config.SHORT_DELAY)
mcFrame.testTableCellStates(day_table_cell=29)

#####################
# test Component
#####################
# mouse click table_cell
# BUG516721: all accessibles have the same position and size
'''
click_day_table_cell = \
                mcFrame.tree_table.getChildAtIndex(mcFrame.dayTableCell(6))
click_day_table_cell.mouseClick()
sleep(config.SHORT_DELAY)
mcFrame.findAccessiblesFromTreeTable()
sleep(config.SHORT_DELAY)
mcFrame.assertText(mcFrame.label, "Your selection is:\n2008-01-6")
mcFrame.testTableCellStates(day_table_cell=6)

click_day_table_cell = \
                mcFrame.tree_table.getChildAtIndex(mcFrame.dayTableCell(30))
click_day_table_cell.mouseClick()
sleep(config.SHORT_DELAY)
mcFrame.findAccessiblesFromTreeTable()
sleep(config.SHORT_DELAY)
mcFrame.assertText(mcFrame.label, "Your selection is:\n2008-01-30")
mcFrame.testTableCellStates(day_table_cell=30)
'''

#####################################
# test Month ContextMenuStrip accessibles 
#####################################
# BUG516717:title_month is not accessible
'''
# click title_month cell to expand Month ContextMenu, then find and check 
# default states for all accessibles
mcFrame.title_month.mouseClick(button=1)
sleep(config.SHORT_DELAY)
mcFrame.findMonthContextMenuAccessibles()
mcFrame.monthContextMenuAccessiblesTest()

# click menu_item will change label and title_month cell's name, day 30 still is 
# selected and focused
mcFrame.month_menu_items['December'].click(log=True)
sleep(config.SHORT_DELAY)
mcFrame.assertName(mcFrame.title_month, "December")
mcFrame.findAccessiblesFromTreeTable()
sleep(config.SHORT_DELAY)
mcFrame.assertText(mcFrame.label, "Your selection is:\n2008-12-30")
mcFrame.testTableCellStates(day_table_cell=30)

# click title_month cell to expand Month ContextMenu again, then find and check 
# states for all accessibles
mcFrame.title_month.mouseClick(button=1)
sleep(config.SHORT_DELAY)
mcFrame.findMonthContextMenuAccessibles()
mcFrame.monthContextMenuAccessiblesTest()
'''

####################################
# test Year SpinButton accessible
####################################
# BUG516717:title_year is not accessible
'''
statesCheck(mcFrame.title_year, "SpinButton")

mcFrame.title_year.mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(mcFrame.title_year, "SpinButton", add_states=["focused"])

mcFrame.keyCombo("Up", grabFocus=False)
sleep(config.SHORT_DELAY)
mcFrame.yearSpinButtonValueTest('2009')
mcFrame.assertText(mcFrame.label, "Your selection is:\n2009-12-30")

mcFrame.keyCombo("Down", grabFocus=False)
sleep(config.SHORT_DELAY)
mcFrame.yearSpinButtonValueTest('2008')
mcFrame.assertText(mcFrame.label, "Your selection is:\n2008-12-30")

mcFrame.yearSpinButtonValueTest('2007', set_value=True)
sleep(config.SHORT_DELAY)
mcFrame.assertText(mcFrame.label, "Your selection is:\n2007-12-30")
'''

#########################################
# test Today ContextMenuStrip accessibles
#########################################
# BUG516731: contextmenustrip is not accessible
'''
mcFrame.mouseClick(button=3)
sleep(config.SHORT_DELAY)
mcFrame.findTodayContextMenuAccessibles()
sleep(config.SHORT_DELAY)
mcFrame.todayContextMenuAccessiblesTest()
'''

###########################
# test Today table_cell
###########################
# BUG516719: the cell is showing current day that is not accessible
'''
# change daytime from current day to others
mcFrame.keyCombo("Down", grabFocus=False)
sleep(config.SHORT_DELAY)
# click today_table_cell change to current day
mcFrame.today_table_cell.click(log=True)
sleep(config.SHORT_DELAY)
current_daytime =  mkdate.strftime("%y-%m-%d")
mcFrame.assertText(mcFrame.label, "Your selection is:\n%s" % current_daytime)
# the current day table_cell will be focused and selected
mcFrame.findAccessiblesFromTreeTable()
sleep(config.SHORT_DELAY)
mcFrame.testTableCellStates(day_table_cell=current_day)
'''
# TODO: Test multiple selections

print "INFO:  Log written to: %s" % config.OUTPUT_DIR

# close application frame window
mcFrame.quit()
