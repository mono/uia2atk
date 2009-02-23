#!/usr/bin/env python

##############################################################################
# Written by:  Mike Gorse <mgorse@novell.com>
# Date:        02/23/2009
# Description: Test accessibility of contextmenustrip widget
#              Use the contextmenustripframe.py wrapper script
#              Test the samples/contextmenustrip.py script
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of contextmenustrip widget
"""

import sys

from strongwind import *
from contextmenustrip import *
from helpers import *


app_path = None
try:
    app_path = sys.argv[1]
except IndexError:
    pass #expected

# open the contextmenustrip sample application
    app = launchContextMenuStrip(app_path)
except IOError, e:
    print 'Error: %s' % e
    exit(2)

# make sure we got the app back
if app is None:
    exit(4)

# just an alias to make things shorter
cmFrame = app.contextMenuStripFrame

# open contextmenustrip and assert widgets
cmFrame.label.mouseClick(button=3)
sleep(config.SHORT_DELAY)
cmFrame.assertWidgets()

##############################################################################
# STATES: CONTEXT MENU STRIP
##############################################################################
##############################################################################
# STATES: DEFAULT
##############################################################################
statesCheck(cmFrame.item1, 'Menu')
statesCheck(cmFrame.item1a, 'MenuItem', invalid_states = ["showing"])
statesCheck(cmFrame.item1b, 'MenuItem', invalid_states = ["showing"])
statesCheck(cmFrame.item2, 'MenuItem')
statesCheck(cmFrame.item3, 'MenuItem')
statesCheck(cmFrame.item4, 'MenuItem')
statesCheck(cmFrame.item5, 'MenuItem')

cmFrame.item1.mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(cmFrame.item1a, 'MenuItem')
statesCheck(cmFrame.item1b, 'MenuItem')

# close context_menu and make it show up again
cmFrame.mouseClick()
sleep(config.SHORT_DELAY)
cmFrame.label.mouseClick(button=3)
sleep(config.SHORT_DELAY)

# close application frame window
cmFrame.quit()
