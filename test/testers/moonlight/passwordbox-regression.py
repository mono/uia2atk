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

#######################
# Check default States
#######################
# according to bug557655 single line state won't be supported
#statesCheck(pbFrame.pwdBox, 'Text')

pbFrame.pwdBox.mouseClick()
sleep(config.SHORT_DELAY)
#statesCheck(pbFrame.pwdBox, 'Text', add_states=["focused"])

# test PasswordBox.Text propertys
ENTER_PWD = 'enter123'
TYPE_PWD = '123type'
pbFrame.pwdBox.enterText(ENTER_PWD)
sleep(config.SHORT_DELAY)
# Value pattern won't retain password value
# Text remains empty as bug564346 is won't fixed
assertText(pbFrame.pwdBox, "")
assertName(pbFrame.label2, 'Your password is: enter123')

pbFrame.pwdBox.insertText(TYPE_PWD, 0)
sleep(config.SHORT_DELAY)
# Text remains empty as bug564346 is won't fixed
assertText(pbFrame.pwdBox, "")
assertName(pbFrame.label2, 'Your password is: 123type')

print 'INFO:  Log written to: %s' % config.OUTPUT_DIR

# close application frame window
quit(pbFrame)
