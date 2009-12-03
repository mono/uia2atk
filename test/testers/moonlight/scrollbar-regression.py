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
##Bug 556832
#statesCheck(sbFrame.hlabel, 'Label')
##Bug 556832
#statesCheck(sbFrame.vlabel, 'Label')
##Bug 559825
#statesCheck(sbFrame.hscrollBar, 'HScrollBar', add_states=['horizontal'])
##Bug 559825
#statesCheck(sbFrame.vscrollBar, 'VScrollBar', add_states=['vertical'])
'''
for vs_button in vs_buttons:
    statesCheck(sbFrame.vs_button, 'Button', invalid_states=['focusable'])

for hs_button in hs_buttons:
    statesCheck(sbFrame.vs_button, 'Button', invalid_states=['focusable'])

#######################
# Check default Actions
#######################
# Thumb won't implement Action
for vs_button in vs_buttons:
    if vs_button != vs_buttons[2]:
        actionsCheck(sbFrame.vs_button, 'Button')
    else:
        try:
            actionsCheck(sbFrame.vs_button, 'Button')
        except NotImplementedError:
            return
        assert False, "Action shouldn't be implemented"

for hs_button in hs_buttons:
    if hs_button != hs_buttons[2]:
        actionsCheck(sbFrame.hs_button, 'Button')
    else:
        try:
            actionsCheck(sbFrame.hs_button, 'Button')
        except NotImplementedError:
            return
        assert False, "Action shouldn't be implemented"

##################
# Click Buttons
##################
sbFrame.vs_button[3].click(log=True)
sleep(config.SHORT_DELAY)
assertName(sbFrame.vlabel, 'Value of Vertical: 10')

sbFrame.vs_button[4].click(log=True)
sleep(config.SHORT_DELAY)
assertName(sbFrame.vlabel, 'Value of Vertical: 11')

sbFrame.vs_button[1].click(log=True)
sleep(config.SHORT_DELAY)
assertName(sbFrame.vlabel, 'Value of Vertical: 1')

sbFrame.vs_button[0].click(log=True)
sleep(config.SHORT_DELAY)
assertName(sbFrame.vlabel, 'Value of Vertical: 0')

sbFrame.hs_button[3].click(log=True)
sleep(config.SHORT_DELAY)
assertName(sbFrame.vlabel, 'Value of Horizontal: 10')

sbFrame.hs_button[4].click(log=True)
sleep(config.SHORT_DELAY)
assertName(sbFrame.hlabel, 'Value of Horizontal: 11')

sbFrame.hs_button[1].click(log=True)
sleep(config.SHORT_DELAY)
assertName(sbFrame.vlabel, 'Value of Horizontal: 1')

sbFrame.hs_button[0].click(log=True)
sleep(config.SHORT_DELAY)
assertName(sbFrame.vlabel, 'Value of Horizontal: 0')
'''
##TODO: commented out most of the tests caused by Bug 559825, as we can't correctly find scrollbars.
"""
# set scrollBar's value and assert label's text
sbFrame.setValue(sbFrame.hscrollBar, 20)
sleep(config.SHORT_DELAY)
assertName(sbFrame.hlabel, 'Value of Horizontal: 20')

sbFrame.setValue(sbFrame.vscrollBar, 20)
sleep(config.SHORT_DELAY)
assertName(sbFrame.vlabel, 'Value of Vertical: 20')

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
"""

print 'INFO:  Log written to: %s' % config.OUTPUT_DIR

# close application frame window
quit(sbFrame)
