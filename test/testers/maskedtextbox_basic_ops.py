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


# insert some text into the first MaskedTextControl and check the results
# use two different methods of insertion
mtbFrame.insertText(mtbFrame.date_text, "11312009")
sleep(config.SHORT_DELAY)
mtbFrame.assertText(mtbFrame.date_text, "11/31/2009")
mtbFrame.date_text.deleteText()
sleep(config.SHORT_DELAY)
mtbFrame.assertText(mtbFrame.date_text, "__/__/____")
mtbFrame.typeText("01141982")
sleep(config.MEDIUM_DELAY)
mtbFrame.assertText(mtbFrame.date_text, "01/14/1982")


mtbFrame.quit()
