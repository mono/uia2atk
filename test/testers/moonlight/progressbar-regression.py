#!/usr/bin/env python

##############################################################################
# Written by:  Neville Gao  <nevillegao@gmail.com>
# Date:        2009/09/21
# Description: Test accessibility of progressbar widget
#              Use the progressbarframe.py wrapper script
#              Test the Moonlight ProgressBar sample
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of progressbar widget
"""

# imports
from strongwind import *
from progressbar import *
from helpers import *

from sys import argv
from os import path

app_path = None
try:
    app_path = argv[1]
except IndexError:
    pass  #expected

# open the progressbar sample application
try:
    app = launchProgressBar(app_path)
except IOError, msg:
    print  "ERROR:  %s" % msg
    exit(2)

# make sure we got the app back
if app is None:
    exit(4)

# just an alias to make things shorter
pbFrame = app.progressBarFrame

################
# Check Actions
################
actionsCheck(pbFrame.button, 'Button')

#######################
# Check default States
#######################
statesCheck(pbFrame.progressBar, 'ProgressBar')

# click button to assert label if value of progressBar is correct
pbFrame.button.click(log=True)
sleep(config.SHORT_DELAY)
assertName(pbFrame.label, "It is 20 out of 100%.")
pbFrame.assertValue(20)

pbFrame.setValue(50)
sleep(config.SHORT_DELAY)
# Value property of progressbar is read-only, so Value shouldn't be updated
assertName(pbFrame.label, "It is 20 out of 100%.")
pbFrame.assertValue(20)

print 'INFO:  Log written to: %s' % config.OUTPUT_DIR

# close application frame window
quit(pbFrame)
