#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        10/12/2009
# Description: Test accessibility of hyperlinkbutton widget
#              Use the hyperlinkbuttonframe.py wrapper script
#              Test the Moonlight hyperlinkbutton sample
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of hyperlinkbutton widget
"""

# imports
import sys

from strongwind import *
from hyperlinkbutton import *
from helpers import *
from sys import argv

app_path = None
try:
    app_path = argv[1]
except IndexError:
    pass #expected

# open the hyperlinkbutton sample application
try:
    app = launchHyperlinkButton(app_path)
except IOError, msg:
    print "ERROR:  %s" % msg
    exit(2)

# make sure we got the app back
if app is None:
    exit(4)

# just an alias to make things shorter
hlbFrame = app.hyperlinkButtonFrame

####################
# Check Actions
####################
# BUG555717: wrong action name
#actionsCheck(hlbFrame.hyperlink1, "HyperlinkButton")
#actionsCheck(hlbFrame.hyperlink2, "HyperlinkButton")

######################
# Check default States
######################
# BUG555726: missing multi-line
#statesCheck(hlbFrame.hyperlink1, "Label", add_states=["focusable"])
#statesCheck(hlbFrame.hyperlink2, "Label", add_states=["focusable"])

##################################
# Check Focused State by press Tab
##################################
hlbFrame.frame.mouseClick()
sleep(config.SHORT_DELAY)
#statesCheck(hlbFrame.hyperlink1, "Label", add_states=["focusable", "focused"])

hlbFrame.keyCombo("Tab", grabFocus=False)
sleep(config.SHORT_DELAY)
#statesCheck(hlbFrame.hyperlink2, "Label", add_states=["focusable", "focused"])
#statesCheck(hlbFrame.hyperlink1, "Label", add_states=["focusable"])

# XXX Atk.Hyperlink won't be implemented according to bug556410
#################################
# Check link number of each label
#################################
#hlbFrame.assertNLinks(hlbFrame.hyperlink1, 1)

#hlbFrame.assertNLinks(hlbFrame.hyperlink2, 1)

################
# Check link URL
################
#hlbFrame.assertURL(hlbFrame.hyperlink1)

#hlbFrame.assertURL(hlbFrame.hyperlink2)

#################################
# Doing jump action to invoke URL
#################################
# BUG553678: Run second sample crash the app
hlbFrame.openURL(hlbFrame.hyperlink1)

hlbFrame.openURL(hlbFrame.hyperlink2)

print "INFO:  Log written to: %s" % config.OUTPUT_DIR

#close application frame window
quit(hlbFrame)
