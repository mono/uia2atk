#!/usr/bin/env python

##############################################################################
# Written by:  Neville Gao  <nevillegao@gmail.com>
# Date:        2009/09/22
# Description: Test accessibility of scrollBar widget
#              Use the scrollbarframe.py wrapper script
#              Test the Moonlight ScrollBar sample
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of scrollBar widget
"""

# imports
from pyatspi import *
from strongwind import *
from scrollbar import *
from helpers import *

from sys import argv
from os import path

app_path = None
try:
    app_path = argv[1]
except IndexError:
    pass  #expected

# open the scrollBar sample application
try:
    app = launchScrollBar(app_path)
except IOError, msg:
    print  "ERROR:  %s" % msg
    exit(2)

# make sure we got the app back
if app is None:
    exit(4)

# just an alias to make things shorter
sbFrame = app.scrollBarFrame

#######################
# Check default States
#######################
statesCheck(sbFrame.hlabel, 'Label')
statesCheck(sbFrame.vlabel, 'Label')
statesCheck(sbFrame.hscrollBar, 'ScrollBar')
statesCheck(sbFrame.vscrollBar, 'ScrollBar')

# set scrollBar's value and assert label's text
sbFrame.setValue(sbFrame.hscrollBar, 20)
sleep(config.SHORT_DELAY)
assertText(hlabel, 'Value of Horizontal: 20')

sbFrame.setValue(sbFrame.vscrollBar, 20)
sleep(config.SHORT_DELAY)
assertText(vlabel, 'Value of Vertical: 20')

# test scrollBar's minValue and maxValue
hminValue = sbFrame.hscrollBar._accessible.queryValue().minimumValue
vminValue = sbFrame.vscrollBar._accessible.queryValue().minimumValue
hmaxVlaue = sbFrame.hscrollBar._accessible.queryValue().maximunValue
vmaxVlaue = sbFrame.vscrollBar._accessible.queryValue().maximunValue

sbFrame.setValue(sbFrame.hscrollBar, hminValue)
sleep(config.SHORT_DELAY)
assertText(sbFrame.hscrollBar, 'Value of Horizontal: %s' % hminValue)
sbFrame.setValue(sbFrame.hscrollBar, hmaxVlaue)
sleep(config.SHORT_DELAY)
assertText(sbFrame.hscrollBar, 'Value of Horizontal: %s' % hmaxVlaue)

sbFrame.setValue(sbFrame.vscrollBar, hminValue)
sleep(config.SHORT_DELAY)
assertText(sbFrame.vscrollBar, 'Value of Vertical: %s' % vminValue)
sbFrame.setValue(sbFrame.vscrollBar, hmaxVlaue)
sleep(config.SHORT_DELAY)
assertText(sbFrame.vscrollBar, 'Value of Vertical: %s' % vmaxVlaue)

# test scrollBar's LargeChange property
# as the thumb in scrollBar is on max value, it is safe to click
# on scrollBar to change value
sbFrame.hscrollBar.click(log=True)
sleep(config.SHORT_DELAY)
assertText(sbFrame.hscrollBar, 'Value of Horizontal: %s' % 90)

sbFrame.vscrollBar.click(log=True)
sleep(config.SHORT_DELAY)
assertText(sbFrame.vscrollBar, 'Value of Vertical: %s' % 90)

print 'INFO:  Log written to: %s' % config.OUTPUT_DIR

# close application frame window
quit(sbFrame)
