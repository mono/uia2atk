#!/usr/bin/env python

##############################################################################
# Written by:  Calen Chen  <cachen@gmail.com>
# Date:        2009/09/23
# Description: Test accessibility of slider widget
#              Use the sliderframe.py wrapper script
#              Test the Moonlight slider sample
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of slider widget
"""

# imports
from strongwind import *
from slider import *
from helpers import *

from sys import argv
from os import path

app_path = None
try:
    app_path = argv[1]
except IndexError:
    pass  #expected

# open the slider sample application
try:
    app = launchSlider(app_path)
except IOError, msg:
    print  "ERROR:  %s" % msg
    exit(2)

# make sure we got the app back
if app is None:
    exit(4)

# just an alias to make things shorter
sFrame = app.sliderFrame

#######################
# Check default States
#######################
statesCheck(sFrame.horizontal_slider, "Slider")
##BUG553160
#statesCheck(sFrame.horizontal_thumb, "Thumb")

statesCheck(sFrame.vertical_slider, "Slider")
##BUG553160
#statesCheck(sFrame.vertical_thumb, "Thumb")

###################################
# Test horizontal slider and thumb
###################################
# test horizontal_thumb is focusable
sFrame.horizontal_thumb.grabFocus()
sleep(config.SHORT_DELAY)
statesCheck(sFrame.horizontal_slider, "Slider")
##BUG553160
#statesCheck(sFrame.horizontal_thumb, "Thumb", add_states=["focused"])

# test Value implementation
sFrame.setValue(sFrame.horizontal_slider, 10)
sleep(config.SHORT_DELAY)
assertName(sFrame.label1, 'Horizontal Slider Value: 10')
sFrame.assertValue(sFrame.horizontal_slider, 10)

sFrame.setValue(sFrame.horizontal_slider, 5)
sleep(config.SHORT_DELAY)
assertName(sFrame.label1, 'Horizontal Slider Value: 5')
sFrame.assertValue(sFrame.horizontal_slider, 5)

sFrame.setValue(sFrame.horizontal_slider, 0)
sleep(config.SHORT_DELAY)
assertName(sFrame.label1, 'Horizontal Slider Value: 0')
sFrame.assertValue(sFrame.horizontal_slider, 0)

# test navigation
sFrame.keyCombo("Right", grabFocus=False)
sleep(config.SHORT_DELAY)
assertName(sFrame.label1, 'Horizontal Slider Value: 0.1')
##Bug 558289
#sFrame.assertValue(sFrame.horizontal_slider, 0.1)

sFrame.keyCombo("Down", grabFocus=False)
sleep(config.SHORT_DELAY)
assertName(sFrame.label1, 'Horizontal Slider Value: 0')
sFrame.assertValue(sFrame.horizontal_slider, 0)

################################
# Test vertical slider and thumb
################################
# test vertical_thumb is focusable
sFrame.vertical_thumb.grabFocus()
sleep(config.SHORT_DELAY)
statesCheck(sFrame.vertical_slider, "Slider")
##BUG553160
#statesCheck(sFrame.vertical_thumb, "Thumb", add_states=["focused"])
##BUG553160
#statesCheck(sFrame.horizontal_thumb, "Thumb")

# test Value implementation
sFrame.setValue(sFrame.vertical_slider, 20)
sleep(config.SHORT_DELAY)
assertName(sFrame.label2, 'Vertical Slider Value: 20')
sFrame.assertValue(sFrame.vertical_slider, 20)

sFrame.setValue(sFrame.vertical_slider, 10)
sleep(config.SHORT_DELAY)
assertName(sFrame.label2, 'Vertical Slider Value: 10')
sFrame.assertValue(sFrame.vertical_slider, 10)

# test navigation
sFrame.keyCombo("Right", grabFocus=False)
sleep(config.SHORT_DELAY)
##Bug 558289
#assertName(sFrame.label2, 'Vertical Slider Value: 10.1')
##Bug 558289
#sFrame.assertValue(sFrame.vertical_slider, 10.1)

sFrame.keyCombo("Down", grabFocus=False)
sleep(config.SHORT_DELAY)
assertName(sFrame.label2, 'Vertical Slider Value: 10')
##Bug 558289
#sFrame.assertValue(sFrame.horizontal_slider, 10)

###############################################
# test vertical slider with IsDirectionReversed
###############################################
sFrame.checkbox.click(log=True)
sleep(config.SHORT_DELAY)
sFrame.keyCombo("Down", grabFocus=False)
sleep(config.SHORT_DELAY)
##Bug 558289
#assertName(sFrame.label2, 'Vertical Slider Value: 10.1')
##Bug 558289
#sFrame.assertValue(sFrame.vertical_slider, 10.1)

sFrame.keyCombo("Left", grabFocus=False)
sleep(config.SHORT_DELAY)
##Bug 558289
#assertName(sFrame.label2, 'Vertical Slider Value: 10.2')
##Bug 558289
#sFrame.assertValue(sFrame.horizontal_slider, 10.2)

sFrame.keyCombo("Up", grabFocus=False)
sleep(config.SHORT_DELAY)
##Bug 558289
#assertName(sFrame.label2, 'Vertical Slider Value: 10.1')
##Bug 558289
#sFrame.assertValue(sFrame.horizontal_slider, 10.1)

sFrame.keyCombo("Right", grabFocus=False)
sleep(config.SHORT_DELAY)
##Bug 558289
#assertName(sFrame.label2, 'Vertical Slider Value:10.2')
##Bug 558289
#sFrame.assertValue(sFrame.horizontal_slider, 10.2)

################################
# Check maxmum and mininum value
################################
sFrame.assertMaximumValue(sFrame.horizontal_slider, 10)
sFrame.assertMinimumValue(sFrame.horizontal_slider, 0)

sFrame.assertMaximumValue(sFrame.vertical_slider, 20)
sFrame.assertMinimumValue(sFrame.vertical_slider, 10)

print 'INFO:  Log written to: %s' % config.OUTPUT_DIR

# close application frame window
quit(sFrame)
