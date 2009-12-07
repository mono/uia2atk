#!/usr/bin/env python

##############################################################################
# Written by:  Neville Gao  <nevillegao@gmail.com>
# Date:        2009/09/27
# Description: Test accessibility of repeatbutton widget
#              Use the repeatbuttonframe.py wrapper script
#              Test the Moonlight RepeatButton sample
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of repeatbutton widget
"""

# imports
from pyatspi import *
from strongwind import *
from repeatbutton import *
from helpers import *

from sys import argv
from os import path

app_path = None
try:
    app_path = argv[1]
except IndexError:
    pass  #expected

# open the repeatbutton sample application
try:
    app = launchRepeatButton(app_path)
except IOError, msg:
    print  "ERROR:  %s" % msg
    exit(2)

# make sure we got the app back
if app is None:
    exit(4)

# just an alias to make things shorter
rbFrame = app.repeatButtonFrame

################
# Check Actions
################
actionsCheck(rbFrame.button, 'Button')

#######################
# Check default States
#######################
statesCheck(rbFrame.button, 'Button')

#####################################
# Mouse click action on repeatbutton
#####################################
rbFrame.button.mouseClick()
sleep(config.SHORT_DELAY)
#statesCheck(rbFrame.button, 'Button', add_states=['focused'])

####################################
# Do Click action for repeatbutton
####################################
rbFrame.keyCombo('Tab', grabFocus=False, log=False)
sleep(config.SHORT_DELAY)
rbFrame.button.click(log=True)
sleep(config.SHORT_DELAY)
statesCheck(rbFrame.button, 'Button', add_states=['focused'])
rbFrame.button.click(log=True)
sleep(config.SHORT_DELAY)
rbFrame.button.click(log=True)
sleep(config.SHORT_DELAY)
assertName(rbFrame.label, 'Number of Clicks: 4')

###################################
# Do Press action for repeatbutton
###################################
rbFrame.press(rbFrame.button, 16)
sleep(config.SHORT_DELAY)
assertName(rbFrame.label, 'Number of Clicks: 20')

print 'INFO:  Log written to: %s' % config.OUTPUT_DIR

# close application frame window
quit(rbFrame)
