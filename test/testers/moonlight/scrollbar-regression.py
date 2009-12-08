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
statesCheck(sbFrame.hscrollBar, 'HScrollBar', add_states=['horizontal'])
statesCheck(sbFrame.vscrollBar, 'VScrollBar', add_states=['vertical'])

for vs_button in sbFrame.vs_buttons:
    statesCheck(vs_button, 'Button', invalid_states=['focusable'])

for hs_button in sbFrame.hs_buttons:
    statesCheck(hs_button, 'Button', invalid_states=['focusable'])

#######################
# Check default Actions
#######################
# Thumb won't implement Action
for vs_button in sbFrame.vs_buttons:
    if vs_button != sbFrame.vs_buttons[2]:
        actionsCheck(vs_button, 'Button')
    else:
        sbFrame.ActionIsNotImplemented(vs_button)        

for hs_button in sbFrame.hs_buttons:
    if hs_button != sbFrame.hs_buttons[2]:
        actionsCheck(hs_button, 'Button')
    else:
        sbFrame.ActionIsNotImplemented(hs_button)        

##################
# Click Buttons
##################
sbFrame.vs_buttons[3].click(log=True)
sleep(config.SHORT_DELAY)
assertName(sbFrame.vlabel, 'Value of Vertical: 10')

sbFrame.vs_buttons[4].click(log=True)
sleep(config.SHORT_DELAY)
assertName(sbFrame.vlabel, 'Value of Vertical: 11')

sbFrame.vs_buttons[1].click(log=True)
sleep(config.SHORT_DELAY)
assertName(sbFrame.vlabel, 'Value of Vertical: 1')

sbFrame.vs_buttons[0].click(log=True)
sleep(config.SHORT_DELAY)
assertName(sbFrame.vlabel, 'Value of Vertical: 0')

sbFrame.hs_buttons[3].click(log=True)
sleep(config.SHORT_DELAY)
assertName(sbFrame.hlabel, 'Value of Horizontal: 10')

sbFrame.hs_buttons[4].click(log=True)
sleep(config.SHORT_DELAY)
assertName(sbFrame.hlabel, 'Value of Horizontal: 11')

sbFrame.hs_buttons[1].click(log=True)
sleep(config.SHORT_DELAY)
assertName(sbFrame.hlabel, 'Value of Horizontal: 1')

sbFrame.hs_buttons[0].click(log=True)
sleep(config.SHORT_DELAY)
assertName(sbFrame.hlabel, 'Value of Horizontal: 0')

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
hmaxVlaue = sbFrame.hscrollBar._accessible.queryValue().maximumValue
vmaxVlaue = sbFrame.vscrollBar._accessible.queryValue().maximumValue

sbFrame.setValue(sbFrame.hscrollBar, hminValue)
sleep(config.SHORT_DELAY)
assertName(sbFrame.hlabel, 'Value of Horizontal: %s' % int(hminValue))
sbFrame.setValue(sbFrame.hscrollBar, hmaxVlaue)
sleep(config.SHORT_DELAY)
assertName(sbFrame.hlabel, 'Value of Horizontal: %s' % int(hmaxVlaue))

sbFrame.setValue(sbFrame.vscrollBar, hminValue)
sleep(config.SHORT_DELAY)
assertName(sbFrame.vlabel, 'Value of Vertical: %s' % int(vminValue))
sbFrame.setValue(sbFrame.vscrollBar, hmaxVlaue)
sleep(config.SHORT_DELAY)
assertName(sbFrame.vlabel, 'Value of Vertical: %s' % int(vmaxVlaue))

# test scrollBar's LargeChange property
# as the thumb in scrollBar is on max value, it is safe to click
# on scrollBar to change value
sbFrame.hscrollBar.mouseClick()
sleep(config.SHORT_DELAY)
assertName(sbFrame.hlabel, 'Value of Horizontal: %s' % 90)

sbFrame.vscrollBar.mouseClick()
sleep(config.SHORT_DELAY)
assertName(sbFrame.vlabel, 'Value of Vertical: %s' % 90)

print 'INFO:  Log written to: %s' % config.OUTPUT_DIR

# close application frame window
quit(sbFrame)
