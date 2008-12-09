#!/usr/bin/env python
# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        09/08/2008
# Description: main test script of numericupdown
#              ../samples/numericupdown.py is the test sample script
#              numericupdown/* is the wrapper of numericupdown test sample
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

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
nudFrame = app.numericUpDownFrame

##############################
# check numericupdown's states
##############################
statesCheck(nudFrame.editable_numericupdown, "NumericUpDown", add_states=["focused"])
statesCheck(nudFrame.uneditable_numericupdown, "NumericUpDown", invalid_states=["editable"])

# move the focused to uneditable_numericupdown then check the states again
nudFrame.uneditable_numericupdown.mouseClick()
statesCheck(nudFrame.editable_numericupdown, "NumericUpDown")
statesCheck(nudFrame.uneditable_numericupdown, "NumericUpDown", invalid_states=["editable"], add_states=["focused"])

##############################
# input numbers from UI
##############################
# editable NumericUpDown
nudFrame.editable_numericupdown.mouseClick()
nudFrame.editable_numericupdown.typeText("20")
nudFrame.keyCombo("Return", grabFocus=False)
sleep(config.SHORT_DELAY)
nudFrame.assertValue(nudFrame.editable_numericupdown, 1020)
nudFrame.assertText(nudFrame.editable_numericupdown, "1020")

# uneditable NumericUpDown
nudFrame.uneditable_numericupdown.mouseClick()
nudFrame.uneditable_numericupdown.typeText("20")
nudFrame.keyCombo("Return", grabFocus=False)
sleep(config.SHORT_DELAY)
nudFrame.assertValue(nudFrame.uneditable_numericupdown, 10)
nudFrame.assertText(nudFrame.uneditable_numericupdown, "10")

#############################
# input numbers from AtkText
#############################
# editable NumericUpDown
nudFrame.editable_numericupdown.mouseClick()
nudFrame.enterTextValue(nudFrame.editable_numericupdown, "10")
nudFrame.keyCombo("Return", grabFocus=False)
sleep(config.SHORT_DELAY)
nudFrame.assertValue(nudFrame.editable_numericupdown, 10)
nudFrame.assertText(nudFrame.editable_numericupdown, "10")

# uneditable NumericUpDown
nudFrame.uneditable_numericupdown.mouseClick()
nudFrame.enterTextValue(nudFrame.uneditable_numericupdown, "10")
nudFrame.keyCombo("Return", grabFocus=False)
sleep(config.SHORT_DELAY)
nudFrame.assertValue(nudFrame.uneditable_numericupdown, 10)
nudFrame.assertText(nudFrame.uneditable_numericupdown, "10")

############################
# input 0 from AtkValue
############################
# editable NumericUpDown
nudFrame.editable_numericupdown.mouseClick()
nudFrame.valueNumericUpDown(nudFrame.editable_numericupdown, 0)
sleep(config.SHORT_DELAY)
nudFrame.assertValue(nudFrame.editable_numericupdown, 0)
# the text wont change until enter a "Return" 
# the behavior is similar to gtk sample
nudFrame.assertText(nudFrame.editable_numericupdown, "0")

############################
# input 100 from AtkValue
############################
# set numericupdown's value to 100
nudFrame.editable_numericupdown.mouseClick()
nudFrame.valueNumericUpDown(nudFrame.editable_numericupdown, 100)
sleep(config.SHORT_DELAY)
nudFrame.assertValue(nudFrame.editable_numericupdown, 100)
nudFrame.assertText(nudFrame.editable_numericupdown, "100")

############################
# set value to max
############################
# set numericupdown's value to maximumValue
nudFrame.editable_numericupdown.mouseClick()
nudFrame.enterTextValue(nudFrame.editable_numericupdown, str(int(nudFrame.maximumValue)))
sleep(config.SHORT_DELAY)
nudFrame.assertValue(nudFrame.editable_numericupdown, nudFrame.maximumValue)
nudFrame.assertText(nudFrame.editable_numericupdown, str(int(nudFrame.maximumValue)))

############################
# set value to max + 1
############################
# set numericupdown's value to maximumValue + 1
nudFrame.editable_numericupdown.mouseClick()
nudFrame.enterTextValue(nudFrame.editable_numericupdown, str(int(nudFrame.maximumValue + 1)))
sleep(config.SHORT_DELAY)
nudFrame.assertValue(nudFrame.editable_numericupdown, nudFrame.maximumValue)
nudFrame.assertText(nudFrame.editable_numericupdown, str(int(nudFrame.maximumValue + 1)))

############################
# set value to min
############################
# set numericupdown's value to minimumValue
nudFrame.enterTextValue(nudFrame.editable_numericupdown, str(int(nudFrame.minimumValue)))
sleep(config.SHORT_DELAY)
nudFrame.assertValue(nudFrame.editable_numericupdown, nudFrame.minimumValue)
nudFrame.assertText(nudFrame.editable_numericupdown, str(int(nudFrame.minimumValue)))

############################
# set value to min - 1
############################
#set numericupdown's value to minimumValue-1
nudFrame.enterTextValue(nudFrame.editable_numericupdown, str(int(nudFrame.minimumValue - 1)))
sleep(config.SHORT_DELAY)
nudFrame.assertValue(nudFrame.editable_numericupdown, nudFrame.minimumValue)
nudFrame.assertText(nudFrame.editable_numericupdown, str(int(nudFrame.minimumValue - 1)))

############################
# press Up/Down on editab_numericupdown
############################
# test press Up/Down action to check Text and Value by keyCombo to 
# editable_numericupdown which increment value is 20
nudFrame.keyCombo("Up", grabFocus=False)
sleep(config.SHORT_DELAY)
nudFrame.assertValue(nudFrame.editable_numericupdown, nudFrame.minimumValue + 20)
nudFrame.assertText(nudFrame.editable_numericupdown, str(int(nudFrame.minimumValue + 20)))

# press "Down" on editab_numericupdown
nudFrame.keyCombo("Down", grabFocus=False)
sleep(config.SHORT_DELAY)
nudFrame.assertValue(nudFrame.editable_numericupdown, nudFrame.minimumValue)
nudFrame.assertText(nudFrame.editable_numericupdown, str(int(nudFrame.minimumValue)))

############################
# press Up/Down on uneditab_numericupdown
############################
# test press Up/Down action to check Text and Value of 
# uneditable_numericupdown which increment value is 1
nudFrame.uneditable_numericupdown.mouseClick()
nudFrame.keyCombo("Up", grabFocus=False)
sleep(config.SHORT_DELAY)
nudFrame.assertValue(nudFrame.uneditable_numericupdown, 11)
nudFrame.assertText(nudFrame.uneditable_numericupdown, "11")

# press "Down" on uneditab_numericupdown
nudFrame.keyCombo("Down", grabFocus=False)
sleep(config.SHORT_DELAY)
nudFrame.assertValue(nudFrame.uneditable_numericupdown, 10)
nudFrame.assertText(nudFrame.uneditable_numericupdown, "10")

############################
# End
############################
# close application frame window
nudFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
