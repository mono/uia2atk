#!/usr/bin/env python

##############################################################################
# Written by:  Neville Gao  <nevillegao@gmail.com>
# Date:        2009/09/16
# Description: Test accessibility of passwordbox widget
#              Use the passwordboxframe.py wrapper script
#              Test the Moonlight PasswordBox sample
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of passwordbox widget
"""

# imports
from strongwind import *
from passwordbox import *
from helpers import *

from sys import argv
from os import path

app_path = None
try:
    app_path = argv[1]
except IndexError:
    pass  #expected

# open the passwordbox sample application
try:
    app = launchPasswordBox(app_path)
except IOError, msg:
    print  "ERROR:  %s" % msg
    exit(2)

# make sure we got the app back
if app is None:
    exit(4)

# just an alias to make things shorter
pbFrame = app.passwordBoxFrame

################
# Check Actions
################
actionsCheck(pbFrame.label1, 'Label')
actionsCheck(pbFrame.label2, 'Label')
actionsCheck(pbFrame.pwdBox, 'Text')

#######################
# Check default States
#######################
statesCheck(pbFrame.label1, 'Label')
statesCheck(pbFrame.label2, 'Label')
statesCheck(pbFrame.pwdBox2, 'Text')

# test PasswordBox.Text property
PWD = 'secretwords'
pbFrame.pwdBox.enterText(PWD)
sleep(config.SHORT_DELAY)
assertText(pbFrame.pwdBox, PWD)

# test PasswordBox.PasswordChanged event
assertText(pbFrame.label1, 'You changed %s times.' % str(len(PWD)))

print 'INFO:  Log written to: %s' % config.OUTPUT_DIR

# close application frame window
quit(pbFrame)
