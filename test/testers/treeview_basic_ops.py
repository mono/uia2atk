#!/usr/bin/env python

##############################################################################
# Written by:  Brian G. Merrell <bgmerrell@novell.com>
# Date:        02/12/2009
# Description: Test accessibility of treeview widget 
#              Use the treeviewframe.py wrapper script
#              Test the samples/treeview.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of treeview widget
"""

# imports
import sys
import os

from strongwind import *
from treeview import *
from helpers import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the label sample application
try:
  app = launchTreeView(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

# just an alias to make things shorter
tvFrame = app.treeViewFrame

sleep(config.SHORT_DELAY)

#close application frame window
tvFrame.quit()
