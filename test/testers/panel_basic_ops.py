#!/usr/bin/env python
# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        10/21/2008
# Description: main test script of panel
#              ../samples/panel.py is the test sample script
#              panel/* is the wrapper of panel test sample script
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of Panel widget
"""

# imports
from panel import *
from helpers import *
from states import *
from actions import *
from sys import argv

app_path = None 
try:
    app_path = argv[1]
except IndexError:
    pass #expected

# open the panel sample application
try:
    app = launchPanel(app_path)
except IOError, msg:
    print "ERROR:  %s" % msg
    exit(2)

# make sure we got the app back
if app is None:
    exit(4)

# just an alias to make things shorter
pFrame = app.panelFrame

###########################
# check children's AtkAction
###########################
# check if checkbox and radiobutton in panel still have correct actions
actionsCheck(pFrame.check1, "CheckBox")
actionsCheck(pFrame.check2, "CheckBox")
actionsCheck(pFrame.check3, "CheckBox")
actionsCheck(pFrame.check4, "CheckBox")

actionsCheck(pFrame.radio1, "RadioButton")
actionsCheck(pFrame.radio2, "RadioButton")
actionsCheck(pFrame.radio3, "RadioButton")

###########################
# check panel's AtkAccessible
###########################
# check panel states
statesCheck(pFrame.panel1, "Panel")
statesCheck(pFrame.panel2, "Panel")

###########################
# check children's AtkAccessible
###########################
# check if checkbox and radiobutton in panel still have correct states
statesCheck(pFrame.check1, "CheckBox", add_states=["focused"])
statesCheck(pFrame.check2, "CheckBox")
statesCheck(pFrame.check3, "CheckBox", add_states=["checked"])
statesCheck(pFrame.check4, "CheckBox", 
                           invalid_states=["sensitive", "enabled","focusable"])

statesCheck(pFrame.radio1, "RadioButton", add_states=["checked"])
statesCheck(pFrame.radio2, "RadioButton")
statesCheck(pFrame.radio3, "RadioButton",
                       invalid_states=["focusable", "sensitive", "enabled"])

# check if label in panel still have correct states
statesCheck(pFrame.label1, "Label")
statesCheck(pFrame.label2, "Label")
statesCheck(pFrame.label3, "Label")

# click on checkbox which is in panel1 to assert checked state rising
pFrame.check1.click(log=True)
sleep(config.SHORT_DELAY)
statesCheck(pFrame.check1, "CheckBox", add_states=["checked", "focused"])
statesCheck(pFrame.check3, "CheckBox", add_states=["checked"])

# click on radiobutton which is in panel2 to update label
pFrame.radio2.click(log=True)
sleep(config.SHORT_DELAY)
pFrame.assertText(pFrame.label3, 'You are Female')
statesCheck(pFrame.radio2, "RadioButton", add_states=["checked"])

# close application frame window
pFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
