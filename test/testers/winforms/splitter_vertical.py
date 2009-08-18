#!/usr/bin/env python

##############################################################################
# Written by:  Brian G. Merrell <bgmerrell@novell.com>
# Date:        01/26/2009
# Description: Test accessibility of splitter widget 
#              Use the splitterframe.py wrapper script
#              Test the samples/winforms/splitter.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of splitter widget
"""

# LOTS of splitter bugs, going to need to get some fixed before I can figure
# out how to test this well.  Here's the list of bugs:
# BUG471757
# BUG471749
# BUG471215
# BUG471067
# BUG469569

# imports
import sys
import os

from strongwind import *
from splitter_vertical import *
from helpers import *
from sys import argv
from os import path

app_path = None 
try:
    app_path = argv[1]
except IndexError:
    pass #expected

# open the splitter sample application
try:
    app = launchSplitter(app_path)
except IOError, msg:
    print "ERROR:  %s" % msg
    exit(2)

sleep(config.SHORT_DELAY)

# make sure we got the app back
if app is None:
    exit(4)

# just an alias to make things shorter
sFrame = app.splitterFrame

#BUG471215
statesCheck(sFrame.split_pane, "Splitter", add_states=[states.HORIZONTAL])

# assign value for split pane to make sure it's moved
sFrame.assignValue(80)
sleep(config.SHORT_DELAY)
sFrame.assertValue(80)
sFrame.assertSplitterMoved(sFrame.tree_table, expected_width=80)

sFrame.assignValue(100)
sleep(config.SHORT_DELAY)
sFrame.assertValue(100)
sFrame.assertSplitterMoved(sFrame.tree_table, expected_width=100)

# assign value less than minimum, the current value should turn to minimumValue
sFrame.assignValue(0)
sleep(config.SHORT_DELAY)
sFrame.assertValue(sFrame.split_pane._accessible.queryValue().minimumValue)
sFrame.assertSplitterMoved(sFrame.tree_table,
        expected_width=sFrame.split_pane._accessible.queryValue().minimumValue)

# assign value more than minimum, the current value should turn to maximumValue
# BUG526806:assign split pane with a bigger value than maximumValue doesn't work
#sFrame.assignValue(300)
#sleep(config.SHORT_DELAY)
#sFrame.assertValue(sFrame.split_pane._accessible.queryValue().maximumValue)
#sFrame.assertSplitterMoved(sFrame.tree_table,
#        expected_width=sFrame.split_pane._accessible.queryValue().maximumValue)

sFrame.quit()
