#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        09/16/2009
# Description: Test accessibility of image widget 
#              Use the imageframe.py wrapper script
#              Test the Moonlight image sample
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of image widget
"""

# imports
from strongwind import *
from image import *
from helpers import *
from sys import argv
from os import path

app_path = None 
try:
    app_path = argv[1]
except IndexError:
    pass  #expected

# open the image sample application
try:
    app = launchImage(app_path)
except IOError, msg:
    print "ERROR:  %s" % msg
    exit(2)

# make sure we got the app back
if app is None:
    exit(4)

# just an alias to make things shorter
iFrame = app.imageFrame

######################
# Image default States
######################
statesCheck(iFrame.image1, "Icon")
statesCheck(iFrame.image2, "Icon")

###########################
# Image Implementation test
###########################
# Atk.Image won't be implemented according to bug553176
#assertImageSize(iFrame.image1, expected_width=100, expected_height=100)
#assertImageSize(iFrame.image2, expected_width=200, expected_height=200)

print "INFO:  Log written to: %s" % config.OUTPUT_DIR

#close application frame window
quit(iFrame)
