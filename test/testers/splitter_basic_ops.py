#!/usr/bin/env python

##############################################################################
# Written by:  Brian G. Merrell <bgmerrell@novell.com>
# Date:        01/26/2009
# Description: Test accessibility of splitter widget 
#              Use the splitterframe.py wrapper script
#              Test the samples/splitter.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of splitter widget
"""

# imports
import sys
import os

from strongwind import *
from splitter import *
from helpers import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the splitter sample application
try:
  app = launchSplitter(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

sleep(config.SHORT_DELAY)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
sFrame = app.splitterFrame

sFrame.quit()
