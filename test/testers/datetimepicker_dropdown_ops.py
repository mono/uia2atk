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

# XXX: we should do some testing before clicking here (bgmerrell)
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
NUM_MONTHS = 12
NUM_DAYS = 7
# subtract 1 because so January is 0 and December is 11 so we can iterate 
# through range(NUM_MONTHS)
CURRENT_MONTH = dtpddFrame.localtime[1] - 1
CURRENT_DAY = dtpddFrame.localtime[6]

# check the status of all months
for i in range(NUM_MONTHS):
    if i == CURRENT_MONTH:
        add_states=["focused", "selected"]
        invalid_states=[]
    else:
        add_states=[]
        invalid_states=["showing", "visible"]
    statesCheck(dtpddFrame.months[i], "ListItem", invalid_states, add_states)

# check the status of all days
for i in range(NUM_DAYS):
    if i == CURRENT_MONTH:
        add_states=["focused", "selected"]
        invalid_states=[]
    else:
        add_states=[]
        invalid_states=["showing", "visible"]
    statesCheck(dtpddFrame.weekdays[i], "ListItem", invalid_states, add_states)

# XXX: the focusable problem is under discussion
statesCheck(dtpddFrame.checkbox, "CheckBox", add_states=["checked"], invalid_states=["focusable"])
statesCheck(dtpddFrame.dropdownbutton, "Button", invalid_states=["focusable"])
sleep(config.SHORT_DELAY)

##############################
# End
##############################
# close application frame window
dtpddFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
