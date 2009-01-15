#!/usr/bin/env python

##############################################################################
# Written by:  Brian G. Merrell <bgmerrell@novell.com>
# Date:        01/13/2009
# Description: Test accessibility of maskedtextbox widget 
#              Use the maskedtextboxframe.py wrapper script
#              Test the samples/maskedtextbox.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of maskedtextbox widget
"""

# imports
import sys
import os

from strongwind import *
from maskedtextbox import *
from helpers import *
from actions import *
from states import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the maskedtextbox sample application
try:
  app = launchMaskedTextBox(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
mtbFrame = app.maskedTextBoxFrame

# check the states
statesCheck(mtbFrame.date_text, "MaskedTextBox", add_states=[FOCUSED])
statesCheck(mtbFrame.phone_text, "MaskedTextBox")
statesCheck(mtbFrame.money_text, "MaskedTextBox")
statesCheck(mtbFrame.blank_text, "MaskedTextBox")

# insert some text into the first MaskedTextControl and check the results
# use two different methods of insertion
mtbFrame.date_text.insertText("11312009")
sleep(config.SHORT_DELAY)
mtbFrame.assertText(mtbFrame.date_text, "11/31/2009")
mtbFrame.date_text.deleteText()
sleep(config.SHORT_DELAY)
mtbFrame.assertText(mtbFrame.date_text, "__/__/____")
mtbFrame.typeText("01141982")
sleep(config.MEDIUM_DELAY)
mtbFrame.assertText(mtbFrame.date_text, "01/14/1982")

# tab down to the next MaskedTextBox
mtbFrame.keyCombo("Tab", grabFocus=False)

# check the states again now that focus has changed
statesCheck(mtbFrame.date_text, "MaskedTextBox")
statesCheck(mtbFrame.phone_text, "MaskedTextBox", add_states=[FOCUSED])
statesCheck(mtbFrame.money_text, "MaskedTextBox")
statesCheck(mtbFrame.blank_text, "MaskedTextBox")

# insert some text into this MaskedTextControl and check the results  use two
# different methods of insertion
mtbFrame.phone_text.insertText("12345678")
sleep(config.SHORT_DELAY)
mtbFrame.assertText(mtbFrame.phone_text, "(861)-234-5678")
mtbFrame.phone_text.deleteText()
sleep(config.SHORT_DELAY)
# two deletes should not crash the application BUG465467
mtbFrame.phone_text.deleteText()
sleep(config.SHORT_DELAY)
mtbFrame.assertText(mtbFrame.phone_text, "(86_)-___-____")
# arrow over to the first blank
mtbFrame.keyCombo("Right", grabFocus=False)
mtbFrame.keyCombo("Right", grabFocus=False)
mtbFrame.keyCombo("Right", grabFocus=False)
mtbFrame.typeText("12345678")
sleep(config.MEDIUM_DELAY)
mtbFrame.assertText(mtbFrame.phone_text, "(861)-234-5678")

# tab down to the next MaskedTextBox
mtbFrame.keyCombo("Tab", grabFocus=False)

# check to make sure the character count is what we expect BUG465018
mtbFrame.assertCharacterCount(money_text, 11)

# insert some text into this MaskedTextControl and check the results use two
# different methods of insertion
mtbFrame.money_text.insertText("987654")
sleep(config.SHORT_DELAY)
mtbFrame.assertText(mtbFrame.money_text, "$987,654.__")
# delete should not crash the application when the blanks still exist
# in the MaskedTextBox control BUG465467 
mtbFrame.money_text.deleteText()
sleep(config.SHORT_DELAY)
sys.exit(33)
mtbFrame.assertText(mtbFrame.money_text, "$___,___.__")
# arrow over to the first blank
mtbFrame.keyCombo("Right", grabFocus=False)
mtbFrame.typeText("987654")
sleep(config.MEDIUM_DELAY)
mtbFrame.assertText(mtbFrame.money_text, "$987,654.__")

# text a deletion of a subset of the text in the date text box BUG466598
mtbFrame.money_text.deleteText(start=0, end=2)
mtbFrame.assertText(mtbFrame.date_text, "__/14/1982")
mtbFrame.phone_text.deleteText(start=1, end=3)
sleep(config.SHORT_DELAY)
# two deletes should not crash the application BUG465467
# mtbFrame.phone_text.deleteText()
sleep(config.SHORT_DELAY)
# not sure exactly what should be the expected result here.  
# it depends on the resolution of BUG465095.  Here is a guess:
mtbFrame.assertText(mtbFrame.phone_text, "(86_)-234-5678")

# TODO: create a test case for BUG465095 once we determined the desired
# functionality

# insert a single character into the first index 
mtbFrame.blank_text.insertText(0, "A")
mtbFrame.assertText(mtbFrame.blank_text, "A______")

# should be able to insert a single character in the last index of the
# MaskedTextControl BUG465091
mtbFrame.blank_text.insertText(6, "Z", 1)
mtbFrame.assertText(mtbFrame.blank_text, "A_____Z")

mtbFrame.quit()
