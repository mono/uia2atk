#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        09/22/2009
# Description: Test accessibility of textblock widget 
#              Use the textblockframe.py wrapper script
#              Test the Moonlight textblock sample
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of textblock widget
"""

# imports
from strongwind import *
from textblock import *
from helpers import *
from sys import argv
from os import path

app_path = None 
try:
    app_path = argv[1]
except IndexError:
    pass  #expected

# open the textblock sample application
try:
    app = launchTextBlock(app_path)
except IOError, msg:
    print "ERROR:  %s" % msg
    exit(2)

# make sure we got the app back
if app is None:
    exit(4)

# just an alias to make things shorter
tbFrame = app.textBlockFrame

##########################
# TextBlock default States
##########################
statesCheck(tbFrame.text1, "Label")
statesCheck(tbFrame.text2, "Label")

#####################
# Text implementation
#####################
assertText(tbFrame.text1, "simply TextBlock")
assertText(tbFrame.text2, 
                   "sample with LinkBreak and Run\r\nline2\r\nline3\r\nline4")

print "INFO:  Log written to: %s" % config.OUTPUT_DIR

#close application frame window
quit(tbFrame)
