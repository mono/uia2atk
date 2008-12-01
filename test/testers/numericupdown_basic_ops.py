#!/usr/bin/env python
# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        09/08/2008
# Description: Test accessibility of numericupdown widget 
#              Use the numericupdownframe.py wrapper script
#              Test the samples/numericupdown.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of numericupdown widget
"""
# imports
from numericupdown import *
from helpers import *
from states import *
from actions import *
from sys import argv
import pdb
app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the numericupdown sample application
try:
  app = launchNumericUpDown(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

pdb.set_trace()
# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
nudFrame = app.numericUpDownFrame

# check numericupdown's states
statesCheck(nudFrame.editable_numericupdown, "NumericUpDown", add_states=["focused"])
statesCheck(nudFrame.uneditable_numericupdown, "NumericUpDown")
# move the focused to uneditable_numericupdown then check the states again
nudFrame.uneditable_numericupdown.mouseClick()
statesCheck(nudFrame.editable_numericupdown, "NumericUpDown")
statesCheck(nudFrame.uneditable_numericupdown, "NumericUpDown", add_states=["focused"])

# move focused back to editable_numericupdown, 
# type numerber into editable_numericupdown which is editable, 
# check Value and Text
nudFrame.editable_numericupdown.mouseClick()
nudFrame.editable_numericupdown.typeText("20")
nudFrame.keyCombo("Return", grabFocus=False)
sleep(config.SHORT_DELAY)
nudFrame.assertValue(nudFrame.editable_numericupdown, 1020)
nudFrame.assertText(nudFrame.editable_numericupdown, "1020")
# enter text from accerciser, press 'return' in app widget to confirm enter to 
# update Value and Text
#nudFrame.enterTextValue(nudFrame.editable_numericupdown, "10")
#sleep(config.SHORT_DELAY)
#nudFrame.assertValue(nudFrame.editable_numericupdown, 10)
#nudFrame.assertText(nudFrame.editable_numericupdown, "10")

# movo focused to uneditable_numericupdown, 
# type numerber into uneditable_numericupdown which is uneditable
nudFrame.uneditable_numericupdown.mouseClick()
nudFrame.uneditable_numericupdown.typeText("20")
nudFrame.keyCombo("Return", grabFocus=False)
sleep(config.SHORT_DELAY)
nudFrame.assertValue(nudFrame.uneditable_numericupdown, 10)
nudFrame.assertText(nudFrame.uneditable_numericupdown, "10")
# can't enter text from accerciser, Value and Text unchanged
#nudFrame.enterTextValue(nudFrame.uneditable_numericupdown, "50")
#sleep(config.SHORT_DELAY)
#nudFrame.assertValue(nudFrame.uneditable_numericupdown, 10)
#nudFrame.assertText(nudFrame.uneditable_numericupdown, "10")

# movo focused to editable_numericupdown, set numericupdown's value to 0
nudFrame.editable_numericupdown.mouseClick()
nudFrame.valueNumericUpDown(nudFrame.editable_numericupdown, 0)
sleep(config.SHORT_DELAY)
nudFrame.assertValue(nudFrame.editable_numericupdown, 0)
nudFrame.assertText(nudFrame.editable_numericupdown, "0")

# set numericupdown's value to 100
nudFrame.valueNumericUpDown(nudFrame.editable_numericupdown, 100)
sleep(config.SHORT_DELAY)
nudFrame.assertValue(nudFrame.editable_numericupdown, 100)
nudFrame.assertText(nudFrame.editable_numericupdown, "100")

# set numericupdown's value to maximumValue
nudFrame.valueNumericUpDown(nudFrame.editable_numericupdown, nudFrame.maximumValue)
sleep(config.SHORT_DELAY)
nudFrame.assertValue(nudFrame.editable_numericupdown, nudFrame.maximumValue)
nudFrame.assertText(nudFrame.editable_numericupdown, str(int(nudFrame.maximumValue)))

# set numericupdown's value to maximumValue + 1
nudFrame.valueNumericUpDown(nudFrame.editable_numericupdown, nudFrame.maximumValue + 1)
sleep(config.SHORT_DELAY)
nudFrame.assertValue(nudFrame.editable_numericupdown, nudFrame.maximumValue + 1)
nudFrame.assertText(nudFrame.editable_numericupdown, str(int(nudFrame.maximumValue)))

# set numericupdown's value to minimumValue
nudFrame.valueNumericUpDown(nudFrame.editable_numericupdown, nudFrame.minimumValue)
sleep(config.SHORT_DELAY)
nudFrame.assertValue(nudFrame.editable_numericupdown, nudFrame.minimumValue)
nudFrame.assertText(nudFrame.editable_numericupdown, str(int(nudFrame.minimumValue)))

#set numericupdown's value to minimumValue-1
nudFrame.valueNumericUpDown(nudFrame.editable_numericupdown, nudFrame.minimumValue - 1)
sleep(config.SHORT_DELAY)
nudFrame.assertValue(nudFrame.editable_numericupdown, nudFrame.minimumValue - 1)
nudFrame.assertText(nudFrame.editable_numericupdown, str(int(nudFrame.minimumValue)))

# test press Up/Down action to check Text and Value by keyCombo to 
# editable_numericupdown which increment value is 20
nudFrame.keyCombo("Up", grabFocus=False)
sleep(config.SHORT_DELAY)
nudFrame.assertValue(nudFrame.editable_numericupdown, nudFrame.minimumValue + 20)
nudFrame.assertText(nudFrame.editable_numericupdown, str(int(nudFrame.minimumValue + 20)))

nudFrame.keyCombo("Down", grabFocus=False)
sleep(config.SHORT_DELAY)
nudFrame.assertValue(nudFrame.editable_numericupdown, nudFrame.minimumValue)
nudFrame.assertText(nudFrame.editable_numericupdown, str(int(nudFrame.minimumValue)))

# test press Up/Down action to check Text and Value of 
# uneditable_numericupdown which increment value is 1
nudFrame.uneditable_numericupdown.mouseClick()
nudFrame.uneditable_numericupdown.keyCombo("Up", grabFocus=True)
sleep(config.SHORT_DELAY)
nudFrame.assertValue(nudFrame.uneditable_numericupdown, 11)
nudFrame.assertText(nudFrame.uneditable_numericupdown, "11")

nudFrame.uneditable_numericupdown.keyCombo("Down", grabFocus=False)
sleep(config.SHORT_DELAY)
nudFrame.assertValue(nudFrame.uneditable_numericupdown, 10)
nudFrame.assertText(nudFrame.uneditable_numericupdown, "10")

# close application frame window
nudFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
