#!/usr/bin/env python
# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Brian G. Merrell <bgmerrell@novell.com>
# Date:        03/12/2009
# Description: main test script of combobox_stylechanges
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of combobox_stylechanges application
"""

# imports
from combobox_stylechanges import *
from helpers import *
from actions import *
from states import *
from sys import argv

app_path = None 
try:
    app_path = argv[1]
except IndexError:
    pass #expected

# open the combobox_stylechanges sample application
try:
    app = launchComboBox(app_path)
except IOError, msg:
    print "ERROR:  %s" % msg
    exit(2)

# make sure we got the app back
if app is None:
    exit(4)

# just an alias to make things shorter
cbFrame = app.comboBoxStyleChangesFrame

# test the drop down style, which is the style when the app starts


# switch to the simple style and test it
cbFrame.startSimpleStyle()

# switch to the drop down list style and test it
cbFrame.startDropDownListStyle()

# close application frame window
cbFrame.quit()
