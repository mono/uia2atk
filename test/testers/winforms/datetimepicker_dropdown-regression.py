#!/usr/bin/env python
# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        01/13/2008
# Description: main test script of datetimepicker_dropdown
#              ../samples/winforms/datetimepicker_dropdown.py is the test sample script
#              datetimepicker_dropdown/* are the wrappers of datetimepicker_dropdown test sample 
##############################################################################

# WONTFIX506159 - DateTimePicker spin button accessibles erroneously
# implemented the EditableText interface

# The docstring below is used in the generated log file
"""
Test accessibility of DateTimePicker widget
"""

# imports
from datetimepicker_dropdown import *
from helpers import *
from actions import *
from states import *
from sys import argv
import datetime

app_path = None 
try:
    app_path = argv[1]
except IndexError:
    pass #expected

# open the datetimepicker_dropdown sample application
try:
    app = launchDateTimePicker(app_path)
except IOError, msg:
    print "ERROR:  %s" % msg
    exit(2)

# make sure we got the app back
if app is None:
    exit(4)


# just an alias to make things shorter
dtpddFrame = app.dateTimePickerDropDownFrame

##############################
# check datetimepicker_dropdown  AtkAction
##############################
actionsCheck(dtpddFrame.checkbox, "CheckBox")
for i in range(7):
    actionsCheck(dtpddFrame.weekdays[i], "TableCell")
for i in range(12):
    actionsCheck(dtpddFrame.months[i], "TableCell")
actionsCheck(dtpddFrame.dropdownbutton, "Button")

##############################
# check datetimepicker_dropdown AtkAccessible
##############################
# BUG506082 DateTimePicker panel accessible should not have "focusable" and
# "focused" states
#statesCheck(dtpddFrame.panel, "Panel")
statesCheck(dtpddFrame.weekday, "TreeTable")

NUM_MONTHS = 12
NUM_DAYS = 7
# subtract 1 because so January is 0 and December is 11 so we can iterate 
# through range(NUM_MONTHS)
CURRENT_MONTH = dtpddFrame.localtime[1] - 1
CURRENT_WEEKDAY = dtpddFrame.localtime[6]
CURRENT_YEAR = dtpddFrame.localtime[0]
CURRENT_DAY = dtpddFrame.localtime[2]

# check the status of all days
for i in range(NUM_DAYS):
    if i == CURRENT_WEEKDAY:
        add_states=["focused", "selected"]
        invalid_states=[]
    else:
        add_states=[]
        invalid_states=["showing", "visible"]
    statesCheck(dtpddFrame.weekdays[i], "TableCell", invalid_states, add_states)

statesCheck(dtpddFrame.commas[0], "Label")
statesCheck(dtpddFrame.spaces[0], "Label")
statesCheck(dtpddFrame.month, "TreeTable")

# check the status of all months
for i in range(NUM_MONTHS):
    if i == CURRENT_MONTH:
        add_states=["focused", "selected"]
        invalid_states=[]
    else:
        add_states=[]
        invalid_states=["showing", "visible"]
    statesCheck(dtpddFrame.months[i], "TableCell", invalid_states, add_states)

statesCheck(dtpddFrame.spaces[1], "Label")
statesCheck(dtpddFrame.day, "DateTimePicker_Spin")
statesCheck(dtpddFrame.commas[1], "Label")
statesCheck(dtpddFrame.spaces[2], "Label")
statesCheck(dtpddFrame.year, "DateTimePicker_Spin")
statesCheck(dtpddFrame.checkbox, "CheckBox", add_states=["checked", "focused"])
statesCheck(dtpddFrame.dropdownbutton, "Button", invalid_states=["focusable"])
sleep(config.SHORT_DELAY)

dtpddFrame.assertUneditableText(dtpddFrame.spaces[0], "test")
dtpddFrame.assertText(dtpddFrame.spaces[0], " ")
dtpddFrame.assertUneditableText(dtpddFrame.commas[0], "test")
dtpddFrame.assertText(dtpddFrame.commas[0], ",")
dtpddFrame.assertUneditableText(dtpddFrame.checkbox, "test")
dtpddFrame.assertText(dtpddFrame.checkbox, "")
dtpddFrame.assertUneditableText(dtpddFrame.dropdownbutton, "test")
dtpddFrame.assertText(dtpddFrame.dropdownbutton, "Drop Down")

# BUG506252 DateTimePicker table cell accessibles erroneously implement the
# EditableText interface
#dtpddFrame.assertUneditableText(dtpddFrame.months[0], "test")
#dtpddFrame.assertText(dtpddFrame.months[0], "January")
#dtpddFrame.assertUneditableText(dtpddFrame.weekdays[0], "test")
#dtpddFrame.assertText(dtpddFrame.weekdays[0], "Monday")

##############################
# Value
##############################
# change the value of the day of the month and year spin buttons, then make
# sure that the GUI updated appropriately.
dtpddFrame.assignValue(dtpddFrame.day, 20)
mkdate = datetime.date(CURRENT_YEAR, CURRENT_MONTH + 1, 20)
DATE = mkdate.strftime("%A, %B %d, %Y")
sleep(config.SHORT_DELAY)
dtpddFrame.assertText(dtpddFrame.label, "The date you select is: %s" % DATE)

dtpddFrame.assignValue(dtpddFrame.year, 2008)
mkdate = datetime.date(2008, CURRENT_MONTH + 1, 20)
DATE = mkdate.strftime("%A, %B %d, %Y")
sleep(config.SHORT_DELAY)
dtpddFrame.assertText(dtpddFrame.label, "The date you select is: %s" % DATE)

##############################
# Selection
##############################
# we make the value of DateTimePicker to 2009-1-1
dtpddFrame.selectChild(dtpddFrame.month, 0)
sleep(config.SHORT_DELAY)

dtpddFrame.assignValue(dtpddFrame.day, 1)
sleep(config.SHORT_DELAY)

dtpddFrame.assignValue(dtpddFrame.year, 2009)
sleep(config.SHORT_DELAY)

# then we set weekday to Tuesday for instance, 
# the date will be Tuesday, December 30, 2008
dtpddFrame.selectChild(dtpddFrame.weekday, 1)
sleep(config.SHORT_DELAY)

mkdate = datetime.date(2008, 12, 30)
DATE = mkdate.strftime("%A, %B %d, %Y")
sleep(config.SHORT_DELAY)
dtpddFrame.assertText(dtpddFrame.label, "The date you select is: %s" % DATE)

##############################
# checkbox is disabled (so the date should not change when we try to change it)
##############################
dtpddFrame.checkbox.click()
sleep(config.SHORT_DELAY)

PREVIOUS_DATE = DATE
dtpddFrame.assignValue(dtpddFrame.day, 02)
sleep(config.SHORT_DELAY)
dtpddFrame.assertText(dtpddFrame.label, "The date you select is: %s" % PREVIOUS_DATE)

dtpddFrame.assignValue(dtpddFrame.year, 1980)
sleep(config.SHORT_DELAY)
dtpddFrame.assertText(dtpddFrame.label, "The date you select is: %s" % PREVIOUS_DATE)

dtpddFrame.selectChild(dtpddFrame.month, 0)
sleep(config.SHORT_DELAY)
dtpddFrame.assertText(dtpddFrame.label, "The date you select is: %s" % PREVIOUS_DATE)

##############################
# Test AtkAction
##############################
# click dropdown button, then press right twice to navigate to 2009-1-1
dtpddFrame.dropdownbutton.click()
sleep(config.SHORT_DELAY)
dtpddFrame.keyCombo("Right", grabFocus=False)
sleep(config.SHORT_DELAY)
dtpddFrame.keyCombo("Right", grabFocus=False)
sleep(config.SHORT_DELAY)

mkdate = datetime.date(2009, 1, 1)
DATE = mkdate.strftime("%A, %B %d, %Y")
sleep(config.SHORT_DELAY)
dtpddFrame.assertText(dtpddFrame.label, "The date you select is: %s" % DATE)

# close the drop down calendar
dtpddFrame.dropdownbutton.click()
sleep(config.SHORT_DELAY)

# click weekday item
dtpddFrame.click(dtpddFrame.weekdays[6])
sleep(config.SHORT_DELAY)

mkdate = datetime.date(2008, 12, 28)
DATE = mkdate.strftime("%A, %B %d, %Y")
sleep(config.SHORT_DELAY)
dtpddFrame.assertText(dtpddFrame.label, "The date you select is: %s" % DATE)

# click month item
dtpddFrame.click(dtpddFrame.months[0])
sleep(config.SHORT_DELAY)

mkdate = datetime.date(2008, 1, 28)
DATE = mkdate.strftime("%A, %B %d, %Y")
sleep(config.SHORT_DELAY)
dtpddFrame.assertText(dtpddFrame.label, "The date you select is: %s" % DATE)

##############################
# keyCombo navigation
##############################
# navigate to weekday
dtpddFrame.keyCombo("Right", grabFocus=False)
sleep(config.SHORT_DELAY)
dtpddFrame.keyCombo("Up", grabFocus=False)
sleep(config.SHORT_DELAY)
mkdate = datetime.date(2008, 1, 29)
DATE = mkdate.strftime("%A, %B %d, %Y")
dtpddFrame.assertText(dtpddFrame.label, "The date you select is: %s" % DATE)

# navigate to month
dtpddFrame.keyCombo("Right", grabFocus=False)
sleep(config.SHORT_DELAY)
dtpddFrame.keyCombo("Up", grabFocus=False)
sleep(config.SHORT_DELAY)
mkdate = datetime.date(2008, 2, 29)
DATE = mkdate.strftime("%A, %B %d, %Y")
dtpddFrame.assertText(dtpddFrame.label, "The date you select is: %s" % DATE)

# navigate to day
dtpddFrame.keyCombo("Right", grabFocus=False)
sleep(config.SHORT_DELAY)
dtpddFrame.keyCombo("Down", grabFocus=False)
sleep(config.SHORT_DELAY)
mkdate = datetime.date(2008, 2, 28)
DATE = mkdate.strftime("%A, %B %d, %Y")
dtpddFrame.assertText(dtpddFrame.label, "The date you select is: %s" % DATE)
statesCheck(dtpddFrame.day, "DateTimePicker_Spin", add_states=["focused"])
dtpddFrame.assertName(dtpddFrame.day, '28')

# navigate to year
dtpddFrame.keyCombo("Right", grabFocus=False)
sleep(config.SHORT_DELAY)
dtpddFrame.keyCombo("Down", grabFocus=False)
sleep(config.SHORT_DELAY)
mkdate = datetime.date(2007, 2, 28)
DATE = mkdate.strftime("%A, %B %d, %Y")
dtpddFrame.assertText(dtpddFrame.label, "The date you select is: %s" % DATE)
statesCheck(dtpddFrame.year, "DateTimePicker_Spin", add_states=["focused"])
dtpddFrame.assertName(dtpddFrame.year, '2007')

##############################
# End
##############################
# close application frame window
dtpddFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
