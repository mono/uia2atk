#!/usr/bin/env python

##############################################################################
# Written by:  Neville Gao  <nevillegao@gmail.com>
# Date:        2009/09/14
# Description: Test accessibility of button widget
#              Use the buttonframe.py wrapper script
#              Test the Moonlight Button sample
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of checkbox widget
"""

# imports
from strongwind import *
from button import *
from helpers import *

from sys import argv
from os import path

app_path = None
try:
    app_path = argv[1]
except IndexError:
    pass  #expected

# open the button sample application
try:
    app = launchButton(app_path)
except IOError, msg:
    print  "ERROR:  %s" % msg
    exit(2)

# make sure we got the app back
if app is None:
    exit(4)

# just an alias to make things shorter
bFrame = app.buttonFrame

#############
# Check Name
#############
assertName(bFrame.button1, 'Button1')
assertName(bFrame.button2, 'Button2')
assertName(bFrame.button3, 'Button3')
assertName(bFrame.button4, '')
assertName(bFrame.button5, '')

################
# Check Actions
################
actionsCheck(bFrame.button1, 'Button')
actionsCheck(bFrame.button2, 'Button')
actionsCheck(bFrame.button3, 'Button')
actionsCheck(bFrame.button4, 'Button')
actionsCheck(bFrame.button5, 'Button')

#######################
# Check default States
#######################
## BUG553160: missing focusable state
#statesCheck(bFrame.button1, 'Button')
#statesCheck(bFrame.button2, 'Button')
statesCheck(bFrame.button3, 'Button', invalid_states=['enabled', 'focusable', 'sensitive'])
#statesCheck(bFrame.button4, 'Button')
#statesCheck(bFrame.button5, 'Button')

##############################
# Do Click action for button1
##############################
# BUG553170: MessageBox is not accessible now.
#bFrame.button1.click(log=True)
#sleep(config.SHORT_DELAY)
#bFrame.assertDialog()
#statesCheck(bFrame.button1, 'Button', add_states=['focused'])
#sleep(config.SHORT_DELAY)

##############################
# Do Click action for button2
##############################
# click button2 three times and check the label.
bFrame.button2.click(log=True)
sleep(config.SHORT_DELAY)
bFrame.button2.click(log=True)
sleep(config.SHORT_DELAY)
bFrame.button2.click(log=True)
sleep(config.SHORT_DELAY)
#statesCheck(bFrame.button2, 'Button', add_states=['focused'])
assertName(bFrame.label, 'Button2 is clicked 3 times.')

# assert Button image implementations.
## BUG553176: Image is not implemented
#asserImageSize(bFrame.b4_image, 20, 20)
#asserImageSize(bFrame.b5_image, 12, 12)

###################
# 'TAB' Navigation
###################
# click button2 to use 'TAB' key to navigate to button3 next step
bFrame.button2.click(log=True)
sleep(config.SHORT_DELAY)
bFrame.button2.keyCombo('Tab', grabFocus=True)
sleep(config.SHORT_DELAY)
# 'foucused' state should skip button3 and be on button4
statesCheck(bFrame.button3, 'Button', invalid_states=['enabled', 'focusable', 'sensitive'])
#statesCheck(bFrame.button4, 'Button', add_states=['focused'])

print 'INFO:  Log written to: %s' % config.OUTPUT_DIR

# close application frame window
quit(bFrame)
