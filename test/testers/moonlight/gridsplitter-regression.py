#!/usr/bin/env python

##############################################################################
# Written by:  Calen Chen  <cachen@gmail.com>
# Date:        2009/09/29
# Description: Test accessibility of gridsplitter widget
#              Use the gridsplitterframe.py wrapper script
#              Test the Moonlight gridsplitter sample
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of gridsplitter widget
"""

# imports
from strongwind import *
from gridsplitter import *
from helpers import *

from sys import argv
from os import path

app_path = None
try:
    app_path = argv[1]
except IndexError:
    pass  #expected

# open the gridsplitter sample application
try:
    app = launchGridSplitter(app_path)
except IOError, msg:
    print  "ERROR:  %s" % msg
    exit(2)

# make sure we got the app back
if app is None:
    exit(4)

# just an alias to make things shorter
gsFrame = app.gridSplitterFrame

#######################
# Check default States
#######################
statesCheck(gsFrame.vertical_thumb, "Button")
statesCheck(gsFrame.horizontal_thumb, "Button")

###################################
# test navigation of vertical_thumb
###################################
gsFrame.vertical_thumb.grabFocus()
sleep(config.SHORT_DELAY)
statesCheck(gsFrame.vertical_thumb, "Button", add_states=["focused"])

gsFrame.changePosition(gsFrame.vertical_thumb, "Up")
gsFrame.changePosition(gsFrame.vertical_thumb, "Up")
gsFrame.changePosition(gsFrame.vertical_thumb, "Down")
gsFrame.changePosition(gsFrame.vertical_thumb, "Down")

#####################################
# test navigation of horizontal_thumb
#####################################
gsFrame.keyCombo("Tab", grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(gsFrame.horizontal_thumb, "Button", add_states=["focused"])
statesCheck(gsFrame.vertical_thumb, "Button")

gsFrame.changePosition(gsFrame.horizontal_thumb, "Right")
gsFrame.changePosition(gsFrame.horizontal_thumb, "Right")
gsFrame.changePosition(gsFrame.horizontal_thumb, "Left")
gsFrame.changePosition(gsFrame.horizontal_thumb, "Left")

print 'INFO:  Log written to: %s' % config.OUTPUT_DIR

# close application frame window
quit(gsFrame)
