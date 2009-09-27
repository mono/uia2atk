#!/usr/bin/env python

##############################################################################
# Written by:  Neville Gao  <nevillegao@gmail.com>
# Date:        2009/09/27
# Description: Test accessibility of thumb widget
#              Use the thumbframe.py wrapper script
#              Test the Moonlight Thumb sample
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of thumb widget
"""

# imports
from pyatspi import *
from strongwind import *
from thumb import *
from helpers import *

from sys import argv
from os import path

app_path = None
try:
    app_path = argv[1]
except IndexError:
    pass  #expected

# open the thumb sample application
try:
    app = launchThumb(app_path)
except IOError, msg:
    print  "ERROR:  %s" % msg
    exit(2)

# make sure we got the app back
if app is None:
    exit(4)

# just an alias to make things shorter
tFrame = app.thumbFrame

#######################
# Check default States
#######################
statesCheck(tFrame.thumb, 'Thumb')

print 'INFO:  Log written to: %s' % config.OUTPUT_DIR

# close application frame window
quit(tFrame)
