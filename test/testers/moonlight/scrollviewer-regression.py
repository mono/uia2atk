#!/usr/bin/env python

##############################################################################
# Written by:  Calen Chen  <cachen@novell.com>
# Date:        10/20/2009
# Description: Test accessibility of scrollviewer widget
#              Use the scrollviewerframe.py wrapper script
#              Test the Moonlight ScrollViewer sample
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of scrollviewer widget
"""

# imports
from pyatspi import *
from strongwind import *
from scrollviewer import *
from helpers import *

from sys import argv
from os import path

app_path = None
try:
    app_path = argv[1]
except IndexError:
    pass  #expected

# open the scrollviewer sample application
try:
    app = launchScrollViewer(app_path)
except IOError, msg:
    print  "ERROR:  %s" % msg
    exit(2)

# make sure we got the app back
if app is None:
    exit(4)

# just an alias to make things shorter
svFrame = app.scrollViewerFrame

#######################
# Check default States
#######################
statesCheck(svFrame.scroll_viewer, "Panel", add_states=["focusable"])
# bug 556832 wouldn't be fixed
statesCheck(svFrame.label, "Label", invalid_states=["multi line"])
statesCheck(svFrame.hscrollbar, "HScrollBar", add_states=["horizontal"])
statesCheck(svFrame.vscrollbar, "VScrollBar", add_states=["vertical"])

#############
# Test Value
#############
# hscrollbar
svFrame.valueScrollBar(svFrame.hscrollbar, 20)
sleep(config.SHORT_DELAY)
svFrame.assertScrollbar(svFrame.hscrollbar, 20)

h_maximumValue = svFrame.hscrollbar._accessible.queryValue().maximumValue
svFrame.valueScrollBar(svFrame.hscrollbar, h_maximumValue)
sleep(config.SHORT_DELAY)
svFrame.assertScrollbar(svFrame.hscrollbar, h_maximumValue)

h_minimumValue = svFrame.hscrollbar._accessible.queryValue().minimumValue
svFrame.valueScrollBar(svFrame.hscrollbar, h_minimumValue)
sleep(config.SHORT_DELAY)
svFrame.assertScrollbar(svFrame.hscrollbar, h_minimumValue)

svFrame.valueScrollBar(svFrame.hscrollbar, 2000)
sleep(config.SHORT_DELAY)
svFrame.assertScrollbar(svFrame.hscrollbar, 2000)

# vscrollbar
svFrame.valueScrollBar(svFrame.vscrollbar, 20)
sleep(config.SHORT_DELAY)
svFrame.assertScrollbar(svFrame.vscrollbar, 20)

v_maximumValue = svFrame.vscrollbar._accessible.queryValue().maximumValue
svFrame.valueScrollBar(svFrame.vscrollbar, v_maximumValue)
sleep(config.SHORT_DELAY)
svFrame.assertScrollbar(svFrame.vscrollbar, v_maximumValue)

v_minimumValue = svFrame.vscrollbar._accessible.queryValue().minimumValue
svFrame.valueScrollBar(svFrame.vscrollbar, v_minimumValue)
sleep(config.SHORT_DELAY)
svFrame.assertScrollbar(svFrame.vscrollbar, v_minimumValue)

svFrame.valueScrollBar(svFrame.vscrollbar, 2000)
sleep(config.SHORT_DELAY)
svFrame.assertScrollbar(svFrame.vscrollbar, 2000)

###################
# Hidden ScrollBar
###################
# click "Hidden Vertical" button to hidden vscrollbar
svFrame.hidden_vertical.click(log=True)
sleep(config.SHORT_DELAY)
svFrame.assertHiddenScrollBar("vscrollbar", is_showing=False)
# click again to showing vscrollbar
svFrame.hidden_vertical.click(log=True)
sleep(config.SHORT_DELAY)
svFrame.assertHiddenScrollBar("vscrollbar")

# click "Hidden Horizontal" button to hidden hscrollbar
svFrame.hidden_horizontal.click(log=True)
sleep(config.SHORT_DELAY)
svFrame.assertHiddenScrollBar("hscrollbar", is_showing=False)
# click again to showing hscrollbar
svFrame.hidden_horizontal.click(log=True)
sleep(config.SHORT_DELAY)
svFrame.assertHiddenScrollBar("hscrollbar")

print 'INFO:  Log written to: %s' % config.OUTPUT_DIR

# close application frame window
quit(svFrame)
