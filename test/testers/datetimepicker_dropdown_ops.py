#!/usr/bin/env python
# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        01/13/2008
# Description: main test script of datetimepicker_dropdown
#              ../samples/datetimepicker_dropdown.py is the test sample script
#              datetimepicker_dropdown/* are the wrappers of datetimepicker_dropdown test sample 
##############################################################################

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
dtpddFrame.click(dtpddFrame.dropdownbutton)

##############################
# check datetimepicker_dropdown  AtkAction
##############################
actionsCheck(dtpddFrame.checkbox, "CheckBox")
actionsCheck(dtpddFrame.months[0], "ListItem")
actionsCheck(dtpddFrame.months[11], "ListItem")
actionsCheck(dtpddFrame.weekdays[0], "ListItem")
actionsCheck(dtpddFrame.weekdays[6], "ListItem")
actionsCheck(dtpddFrame.dropdownbutton, "Button")

##############################
# check datetimepicker_dropdown AtkAccessible
##############################
today = datetime.date.today()
ACTIVE_STATES = 'add_states=["focused", "selected"]'
INACTIVE_STATES = 'invalid_states=["showing", "visible"]'
if today.strftime("%B") == 'January' or today.strftime("%B") == 'December':
    JAN_STATES = ACTIVE_STATES
    DEC_STATES = ACTIVE_STATES
else:
    JAN_STATES = INACTIVE_STATES
    DEC_STATES = INACTIVE_STATES

if today.strftime("%A") == 'Monday' or today.strftime("%A") == 'Sunday':
    MON_STATES = ACTIVE_STATES
    SUN_STATES = ACTIVE_STATES
else:
    MON_STATES = INACTIVE_STATES
    SUN_STATES = INACTIVE_STATES

# XXX: the focusable problem is under discussion
statesCheck(dtpddFrame.checkbox, "CheckBox", add_states=["checked"], invalid_states=["focusable"])
statesCheck(dtpddFrame.months[0], "ListItem", eval(JAN_STATES))
statesCheck(dtpddFrame.months[11], "ListItem", eval(DEC_STATES))
statesCheck(dtpddFrame.weekdays[0], "ListItem", eval(MON_STATES))
statesCheck(dtpddFrame.weekdays[6], "ListItem", eval(SUN_STATES))
statesCheck(dtpddFrame.dropdownbutton, "Button", invalid_states=["focusable"])
sleep(config.SHORT_DELAY)

##############################
# End
##############################
# close application frame window
dtpddFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
