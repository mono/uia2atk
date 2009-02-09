#!/usr/bin/env python
# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        02/05/2009
# Description: main test script of trackbar
#              ../samples/trackbar.py is the test sample script
#              trackbar/* is the wrapper of trackbar test sample script
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of TrackBar widget
"""

# imports
from trackbar import *
from helpers import *
from states import *
from sys import argv

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the trackbar sample application
try:
  app = launchTrackBar(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

sleep(config.SHORT_DELAY)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
tbFrame = app.trackBarFrame

##############################
# check TrackBars' AtkAccessible
##############################
statesCheck(tbFrame.trackbar_ver, "TrackBar", add_states=["focused", "vertical"])
statesCheck(tbFrame.trackbar_hor, "TrackBar", add_states=["horizontal"])
# switch focus from vertical trackbar to horizontal trackbar
tbFrame.keyCombo("Tab", grabFocus=False)
statesCheck(tbFrame.trackbar_ver, "TrackBar", add_states=["vertical"])
statesCheck(tbFrame.trackbar_hor, "TrackBar", add_states=["focused", "horizontal"])

##############################
# check TrackBars' AtkText is editable or not
##############################
tbFrame.inputText(tbFrame.trackbar_hor, "50")
sleep(config.SHORT_DELAY)
# its Text should be uneditable, so the value should be the init one
tbFrame.assertText(tbFrame.trackbar_hor, "1")

tbFrame.inputText(tbFrame.trackbar_ver, "50")
sleep(config.SHORT_DELAY)
# its Text should be uneditable, so the value should be the init one
tbFrame.assertText(tbFrame.trackbar_ver, "1")

##############################
# check TrackBars' AtkValue
##############################
# input -1 (less then min)
tbFrame.inputValue(tbFrame.trackbar_hor, -1)
sleep(config.SHORT_DELAY)
tbFrame.assertValue(tbFrame.trackbar_hor, 1)
tbFrame.assertText(tbFrame.trackbar_hor, "1")
tbFrame.assertText(tbFrame.label_hor, "The value of TrackBar(Horizontal) is: %s" % "1")

tbFrame.inputValue(tbFrame.trackbar_ver, -1)
sleep(config.SHORT_DELAY)
tbFrame.assertValue(tbFrame.trackbar_ver, 1)
tbFrame.assertText(tbFrame.trackbar_ver, "1")
tbFrame.assertText(tbFrame.label_ver, "The value of TrackBar(Vertical) is: %s" % "1")

# input 101 (greater then max)
tbFrame.inputValue(tbFrame.trackbar_hor, 101)
sleep(config.SHORT_DELAY)
tbFrame.assertValue(tbFrame.trackbar_hor, 100)
tbFrame.assertText(tbFrame.trackbar_hor, "100")
tbFrame.assertText(tbFrame.label_hor, "The value of TrackBar(Horizontal) is: %s" % "100")

tbFrame.inputValue(tbFrame.trackbar_ver, 101)
sleep(config.SHORT_DELAY)
tbFrame.assertValue(tbFrame.trackbar_ver, 100)
tbFrame.assertText(tbFrame.trackbar_ver, "100")
tbFrame.assertText(tbFrame.label_ver, "The value of TrackBar(Vertical) is: %s" % "100")

# input middle value of trackbar
tbFrame.inputValue(tbFrame.trackbar_hor, 50)
sleep(config.SHORT_DELAY)
tbFrame.assertValue(tbFrame.trackbar_hor, 50)
tbFrame.assertText(tbFrame.trackbar_hor, "50")
tbFrame.assertText(tbFrame.label_hor, "The value of TrackBar(Horizontal) is: %s" % "50")

tbFrame.inputValue(tbFrame.trackbar_ver, 50)
sleep(config.SHORT_DELAY)
tbFrame.assertValue(tbFrame.trackbar_ver, 50)
tbFrame.assertText(tbFrame.trackbar_ver, "50")
tbFrame.assertText(tbFrame.label_ver, "The value of TrackBar(Vertical) is: %s" % "50")

# press Up/Down button to change trackbars' value
tbFrame.keyCombo("Up", grabFocus=False)
sleep(config.SHORT_DELAY)
tbFrame.assertValue(tbFrame.trackbar_hor, 51)
tbFrame.assertText(tbFrame.trackbar_hor, "51")
tbFrame.assertText(tbFrame.label_hor, "The value of TrackBar(Horizontal) is: %s" % "51")

tbFrame.keyCombo("Down", grabFocus=False)
sleep(config.SHORT_DELAY)
tbFrame.assertValue(tbFrame.trackbar_hor, 50)
tbFrame.assertText(tbFrame.trackbar_hor, "50")
tbFrame.assertText(tbFrame.label_hor, "The value of TrackBar(Horizontal) is: %s" % "50")

# switch focuse from horizontal trackbar to vertical trackbar
tbFrame.keyCombo("Tab", grabFocus=False)
sleep(config.SHORT_DELAY)
tbFrame.keyCombo("Up", grabFocus=False)
sleep(config.SHORT_DELAY)
tbFrame.assertValue(tbFrame.trackbar_ver, 51)
tbFrame.assertText(tbFrame.trackbar_ver, "51")
tbFrame.assertText(tbFrame.label_ver, "The value of TrackBar(Vertical) is: %s" % "51")

tbFrame.keyCombo("Down", grabFocus=False)
sleep(config.SHORT_DELAY)
tbFrame.assertValue(tbFrame.trackbar_ver, 50)
tbFrame.assertText(tbFrame.trackbar_ver, "50")
tbFrame.assertText(tbFrame.label_ver, "The value of TrackBar(Vertical) is: %s" % "50")

##############################
# End
##############################
tbFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
