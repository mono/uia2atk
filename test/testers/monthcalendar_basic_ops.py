#!/usr/bin/env python
# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Sandy Armstrong <saarmstrong@novell.com>
# Date:        02/23/2009
# Description: Test accessibility of monthcalendar widget 
#              Use the monthcalendarframe.py wrapper script
#              Test the samples/monthcalendar_label_linklabel.py script
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
from datetime import datetime

config.WATCHDOG_TIMEOUT=3600

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

initialrow = -1
initialcol = -1

def checkCells(selectedDateNums=[], checkStates=True, checkActions=True):
    currentfirstreached = False
    nextfirstreached = False
    for r in range(len(mcFrame.tablecells)):
        for c in range(len(mcFrame.tablecells[r])):
            day = int(str(mcFrame.tablecells[r][c]))
            if day == 1 and not currentfirstreached:
                currentfirstreached = True
            elif day == 1 and currentfirstreached:
                nextfirstreached = True
            addstates = ["focused"] #extraneous: focused
            if currentfirstreached and not nextfirstreached and day in selectedDateNums:
                addstates.append("selected")
                global initialrow
                global initialcol
                if initialrow==-1:
                    initialrow = r
                if initialcol==-1:
                    initialcol = c
            if checkStates:
                statesCheck(mcFrame.tablecells[r][c], "TableCell", add_states=addstates, pause=False)
            if checkActions:
                actionsCheck(mcFrame.tablecells[r][c], "TableCell", pause=False)

#check states and actions
statesCheck(mcFrame.backbutton, "Button", add_states=["selectable","focused"], pause=False) #extraneous: selectable, focused
actionsCheck(mcFrame.backbutton, "Button", pause=False)

statesCheck(mcFrame.forwardbutton, "Button", add_states=["selectable","focused"], pause=False)
actionsCheck(mcFrame.forwardbutton, "Button", pause=False)

statesCheck(mcFrame.treetable, "TreeTable", pause=False)

for columnheader in mcFrame.columnheaders:
    statesCheck(columnheader, "TableColumnHeader", add_states=["focusable","focused"], pause=False) #extraneous: focusable,focused
    actionsCheck(columnheader, "TableColumnHeader", invalid_actions=["click"], pause=False)

#NOTE: Sample initializes to 1/23/2008
checkCells(selectedDateNums=[23])

# click next (24)
nextcol = (initialcol+1) % 7
nextrow = initialrow
if nextcol < initialcol:
    nextrow += 1
mcFrame.click(mcFrame.tablecells[nextrow][nextcol])
statesCheck(mcFrame.tablecells[nextrow][nextcol], "TableCell", add_states=["focused","selected"])
statesCheck(mcFrame.tablecells[initialrow][initialcol], "TableCell", add_states=["focused"])

#TODO: Test that name of top-level Filler accessible is always date string (BNC 479130, 479125)

# click forward month
mcFrame.click(mcFrame.forwardbutton)
mcFrame.refreshAccessibleRefs()
assert str(mcFrame.tablecells[0][0])=="27"
sleep(config.SHORT_DELAY)
checkCells(selectedDateNums=[24], checkActions=False)

#click back month
mcFrame.click(mcFrame.backbutton)
mcFrame.refreshAccessibleRefs()
assert str(mcFrame.tablecells[0][0])=="30"
sleep(config.SHORT_DELAY)
checkCells(selectedDateNums=[24], checkActions=False)

#click date in next month
mcFrame.click(mcFrame.tablecells[5][6]) # bottom-right cell
mcFrame.refreshAccessibleRefs()
assert str(mcFrame.tablecells[0][0])=="27"
sleep(config.SHORT_DELAY)
checkCells(selectedDateNums=[9], checkActions=False)

#click date in previous month
mcFrame.click(mcFrame.tablecells[0][0]) # top-left cell
mcFrame.refreshAccessibleRefs()
assert str(mcFrame.tablecells[0][0])=="30"
sleep(config.SHORT_DELAY)
checkCells(selectedDateNums=[27], checkActions=False)

#up/down/left/right keypresses (within a month)
mcFrame.keyCombo("Up")
checkCells(selectedDateNums=[20], checkActions=False)
mcFrame.keyCombo("Down")
checkCells(selectedDateNums=[27], checkActions=False)
mcFrame.keyCombo("Left")
checkCells(selectedDateNums=[26], checkActions=False)
mcFrame.keyCombo("Right")
checkCells(selectedDateNums=[27], checkActions=False)
mcFrame.keyCombo("Right")
checkCells(selectedDateNums=[28], checkActions=False)

#up/down/left/right keypresses (between months)
mcFrame.keyCombo("Down")
mcFrame.refreshAccessibleRefs()
assert str(mcFrame.tablecells[0][0])=="27"
sleep(config.SHORT_DELAY)
checkCells(selectedDateNums=[4], checkActions=False)
for x in range(4):
    mcFrame.keyCombo("Left")
mcFrame.refreshAccessibleRefs()
assert str(mcFrame.tablecells[0][0])=="30"
sleep(config.SHORT_DELAY)
checkCells(selectedDateNums=[31], checkActions=False)

#page-up, -down
mcFrame.keyCombo("PageDown")
mcFrame.refreshAccessibleRefs()
assert str(mcFrame.tablecells[0][0])=="27"
sleep(config.SHORT_DELAY)
checkCells(selectedDateNums=[29], checkActions=False) # Because there is no Feb 31st
mcFrame.keyCombo("PageUp")
mcFrame.refreshAccessibleRefs()
assert str(mcFrame.tablecells[0][0])=="30"
sleep(config.SHORT_DELAY)
checkCells(selectedDateNums=[29], checkActions=False)

#TODO: Test multiple selections

print "INFO:  Log written to: %s" % config.OUTPUT_DIR

#close application frame window
mcFrame.quit()
