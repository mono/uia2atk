#!/usr/bin/env python

##############################################################################
# Written by:  Brian G. Merrell <bgmerrell@novell.com>
# Date:        01/20/2009
# Description: Test accessibility of richtextbox widget 
#              Use the richtextboxframe.py wrapper script
#              Test the samples/richtextbox.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of richtextbox widget
"""

# imports
import sys
import os

from strongwind import *
from richtextbox import *
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

# open the richtextbox sample application
try:
  app = launchRichTextBox(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
rtbFrame = app.richTextBoxFrame

sleep(config.SHORT_DELAY)

rtbFrame.quit()
