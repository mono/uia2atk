#!/usr/bin/env python
# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        09/08/2008
# Description: main test script of numericupdown
#              ../samples/winforms/numericupdown.py is the test sample script
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
sleep(config.SHORT_DELAY)
nudFrame.assertValue(nudFrame.editable_numericupdown, 1020)
nudFrame.assertText(nudFrame.editable_numericupdown, "1020")

# uneditable NumericUpDown
nudFrame.uneditable_numericupdown.mouseClick()
nudFrame.uneditable_numericupdown.typeText("20")
sleep(config.SHORT_DELAY)
nudFrame.assertValue(nudFrame.uneditable_numericupdown, 10)
nudFrame.assertText(nudFrame.uneditable_numericupdown, "10")

#############################
# input numbers from AtkText
#############################
# editable NumericUpDown
nudFrame.enterTextValue(nudFrame.editable_numericupdown, "10")
sleep(config.SHORT_DELAY)
nudFrame.assertValue(nudFrame.editable_numericupdown, 10)
nudFrame.assertText(nudFrame.editable_numericupdown, "10")

# uneditable NumericUpDown
nudFrame.enterTextValue(nudFrame.uneditable_numericupdown, "100")
nudFrame.keyCombo("Enter", grabFocus=False)
sleep(config.SHORT_DELAY)
nudFrame.assertValue(nudFrame.uneditable_numericupdown, 10)
nudFrame.assertText(nudFrame.uneditable_numericupdown, "10")

############################
# change value from AtkValue
############################
# editable NumericUpDown
nudFrame.assignValue(nudFrame.editable_numericupdown, 0)
sleep(config.SHORT_DELAY)
nudFrame.assertValue(nudFrame.editable_numericupdown, 0)
nudFrame.assertText(nudFrame.editable_numericupdown, "0")

nudFrame.assignValue(nudFrame.editable_numericupdown, 100)
sleep(config.SHORT_DELAY)
nudFrame.assertValue(nudFrame.editable_numericupdown, 100)
nudFrame.assertText(nudFrame.editable_numericupdown, "100")

# uneditable NumericUpDown
nudFrame.assignValue(nudFrame.uneditable_numericupdown, 50)
sleep(config.SHORT_DELAY)
nudFrame.assertValue(nudFrame.uneditable_numericupdown, 50)
nudFrame.assertText(nudFrame.uneditable_numericupdown, "50")

############################
# set value to max
############################
# set numericupdown's value to maximumValue
# enter text value as a float, which is what we get back from
# queryValue().currentValue
nudFrame.enterTextValue(nudFrame.editable_numericupdown,
                        str(nudFrame.editableMaximumValue))
nudFrame.mouseClick()
nudFrame.assertValue(nudFrame.editable_numericupdown,
                     nudFrame.editableMaximumValue)
nudFrame.assertText(nudFrame.editable_numericupdown,
                    str(int(nudFrame.editableMaximumValue)))

# try to set the uneditable numericupdown control's text to maximumValue, but
# ensure that it doesn't change (since it is readonly)
nudFrame.enterTextValue(nudFrame.uneditable_numericupdown,
                        str(int(nudFrame.uneditableMaximumValue)))
nudFrame.assertValue(nudFrame.uneditable_numericupdown, 50)
nudFrame.assertText(nudFrame.uneditable_numericupdown, "50")

############################
# set value to max + 1
############################
# set numericupdown's value to maximumValue + 1
nudFrame.enterTextValue(nudFrame.editable_numericupdown,
                        str(int(nudFrame.editableMaximumValue+1)))
sleep(config.MEDIUM_DELAY)
nudFrame.keyCombo("Enter", grabFocus=False)
sleep(config.SHORT_DELAY)
nudFrame.assertValue(nudFrame.editable_numericupdown,
                     nudFrame.editableMaximumValue)
nudFrame.assertText(nudFrame.editable_numericupdown,
                    str(int(nudFrame.editableMaximumValue)))

############################
# set value to min
############################
# set numericupdown's value to minimumValue
nudFrame.enterTextValue(nudFrame.editable_numericupdown,
                        str(int(nudFrame.editableMinimumValue)))
nudFrame.assertValue(nudFrame.editable_numericupdown,
                     nudFrame.editableMinimumValue)
nudFrame.assertText(nudFrame.editable_numericupdown,
                    str(int(nudFrame.editableMinimumValue)))

nudFrame.enterTextValue(nudFrame.uneditable_numericupdown,
                        str(int(nudFrame.uneditableMinimumValue)))
nudFrame.assertValue(nudFrame.uneditable_numericupdown, 50)
nudFrame.assertText(nudFrame.uneditable_numericupdown, "50")

############################
# set value to min - 1
############################
#set numericupdown's value to minimumValue-1
nudFrame.enterTextValue(nudFrame.editable_numericupdown,
                        str(int(nudFrame.editableMinimumValue - 1)))
sleep(config.MEDIUM_DELAY)
nudFrame.keyCombo("Enter", grabFocus=False)
sleep(config.SHORT_DELAY)
nudFrame.assertValue(nudFrame.editable_numericupdown,
                     nudFrame.editableMinimumValue)
nudFrame.assertText(nudFrame.editable_numericupdown,
                    str(int(nudFrame.editableMinimumValue)))

############################
# press Up/Down on editab_numericupdown
############################
# test press Up/Down action to check Text and Value by keyCombo to 
# editable_numericupdown which increment value is 20
nudFrame.keyCombo("Up", grabFocus=False)
sleep(config.SHORT_DELAY)
nudFrame.assertValue(nudFrame.editable_numericupdown,
                     nudFrame.editableMinimumValue + 20)
nudFrame.assertText(nudFrame.editable_numericupdown,
                    str(int(nudFrame.editableMinimumValue + 20)))

# press "Down" on editable_numericupdown
nudFrame.keyCombo("Down", grabFocus=False)
sleep(config.SHORT_DELAY)
nudFrame.assertValue(nudFrame.editable_numericupdown,
                     nudFrame.editableMinimumValue)
nudFrame.assertText(nudFrame.editable_numericupdown,
                    str(int(nudFrame.editableMinimumValue)))

# press "Down" again on editab_numericupdown and make sure the accessible
# text and value do not change (since the control is at its minimum value)
nudFrame.keyCombo("Down", grabFocus=False)
sleep(config.SHORT_DELAY)
nudFrame.assertValue(nudFrame.editable_numericupdown,
                     nudFrame.editableMinimumValue)
nudFrame.assertText(nudFrame.editable_numericupdown,
                    str(int(nudFrame.editableMinimumValue)))

############################
# press Up/Down on uneditab_numericupdown
############################
# test press Up/Down action to check Text and Value of 
# uneditable_numericupdown which increment value is 1
nudFrame.uneditable_numericupdown.mouseClick()
nudFrame.keyCombo("Up", grabFocus=False)
sleep(config.SHORT_DELAY)
nudFrame.assertValue(nudFrame.uneditable_numericupdown, 51)
nudFrame.assertText(nudFrame.uneditable_numericupdown, "51")

# press "Down" on uneditab_numericupdown
nudFrame.keyCombo("Down", grabFocus=False)
sleep(config.SHORT_DELAY)
nudFrame.assertValue(nudFrame.uneditable_numericupdown, 50)
nudFrame.assertText(nudFrame.uneditable_numericupdown, "50")

############################
# Use assignValue to change NumericUpDown control values
############################

# try to set each of the controls to something crazy, after which they should
# both remained unchanged
nudFrame.assignValue(nudFrame.editable_numericupdown, 100000)
sleep(config.SHORT_DELAY)
nudFrame.assertValue(nudFrame.editable_numericupdown,
                     nudFrame.editableMinimumValue)
nudFrame.assertText(nudFrame.editable_numericupdown,
                    str(int(nudFrame.editableMinimumValue)))

nudFrame.assignValue(nudFrame.uneditable_numericupdown, -100000)
sleep(config.SHORT_DELAY)
nudFrame.assertValue(nudFrame.uneditable_numericupdown, 50)
nudFrame.assertText(nudFrame.uneditable_numericupdown, "50")

# set them to min-1 and max+1.  Again, they should remain unchanged
nudFrame.assignValue(nudFrame.editable_numericupdown,
                     nudFrame.editableMinimumValue)
sleep(config.SHORT_DELAY)
nudFrame.assertValue(nudFrame.editable_numericupdown,
                     nudFrame.editableMinimumValue)
nudFrame.assertText(nudFrame.editable_numericupdown,
                    str(int(nudFrame.editableMinimumValue)))

# set both NumericUpDown controls' values to 0
nudFrame.assignValue(nudFrame.editable_numericupdown, 0)
sleep(config.SHORT_DELAY)
nudFrame.assertValue(nudFrame.editable_numericupdown, 0)
nudFrame.assertText(nudFrame.editable_numericupdown, "0")

nudFrame.assignValue(nudFrame.uneditable_numericupdown, 0)
sleep(config.SHORT_DELAY)
nudFrame.assertValue(nudFrame.uneditable_numericupdown, 0)
nudFrame.assertText(nudFrame.uneditable_numericupdown, "0")

# set both NumericUpDown controls' values to 1
nudFrame.assignValue(nudFrame.editable_numericupdown, 1)
sleep(config.SHORT_DELAY)
nudFrame.assertValue(nudFrame.editable_numericupdown, 1)
nudFrame.assertText(nudFrame.editable_numericupdown, "1")

nudFrame.assignValue(nudFrame.uneditable_numericupdown, 1)
sleep(config.SHORT_DELAY)
nudFrame.assertValue(nudFrame.uneditable_numericupdown, 1)
nudFrame.assertText(nudFrame.uneditable_numericupdown, "1")

# set both NumericUpDown controls' values to -1
nudFrame.assignValue(nudFrame.editable_numericupdown, -1)
sleep(config.SHORT_DELAY)
nudFrame.assertValue(nudFrame.editable_numericupdown, -1)
nudFrame.assertText(nudFrame.editable_numericupdown, "-1")

nudFrame.assignValue(nudFrame.uneditable_numericupdown, -1)
sleep(config.SHORT_DELAY)
nudFrame.assertValue(nudFrame.uneditable_numericupdown, -1)
nudFrame.assertText(nudFrame.uneditable_numericupdown, "-1")

# set both NumericUpDown controls' values to 10
nudFrame.assignValue(nudFrame.editable_numericupdown, 10)
sleep(config.SHORT_DELAY)
nudFrame.assertValue(nudFrame.editable_numericupdown, 10)
nudFrame.assertText(nudFrame.editable_numericupdown, "10")

nudFrame.assignValue(nudFrame.uneditable_numericupdown, 10)
sleep(config.SHORT_DELAY)
nudFrame.assertValue(nudFrame.uneditable_numericupdown, 10)
nudFrame.assertText(nudFrame.uneditable_numericupdown, "10")

# set both NumericUpDown controls' values to -10
nudFrame.assignValue(nudFrame.editable_numericupdown, -10)
sleep(config.SHORT_DELAY)
nudFrame.assertValue(nudFrame.editable_numericupdown, -10)
nudFrame.assertText(nudFrame.editable_numericupdown, "-10")

nudFrame.assignValue(nudFrame.uneditable_numericupdown, -10)
sleep(config.SHORT_DELAY)
nudFrame.assertValue(nudFrame.uneditable_numericupdown, -10)
nudFrame.assertText(nudFrame.uneditable_numericupdown, "-10")

############################
# End
############################
# close application frame window
nudFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
