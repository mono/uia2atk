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
statesCheck(cmFrame.context_menu, 'ContextMenuStrip')

##############################################################################
# STATES: DEFAULT
##############################################################################
statesCheck(cmFrame.orig_item, 'MenuItem')
statesCheck(cmFrame.radio_item, 'MenuItem')
statesCheck(cmFrame.check_item, 'MenuItem')
statesCheck(cmFrame.orig_item, 'MenuItem')

##############################################################################
# STATES: WHEN CONTEXTMENU SHOW UP
##############################################################################
cmFrame.context_menu.keyCombo('<Down>', grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(cmFrame.orig_item, 'MenuItem', add_states=['focused'])

cmFrame.context_menu.keyCombo('<Down>', grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(cmFrame.radio_item, 'MenuItem', add_states=['focused', 'checked'])

cmFrame.context_menu.keyCombo('<Down>', grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(cmFrame.check_item, 'MenuItem', add_states=['focused', 'checked'])

cmFrame.context_menu.keyCombo('<Down>', grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(cmFrame.orig_item, 'MenuItem', add_states=['focused'])

##############################################################################
# STATES: CHECKED ITEMS
##############################################################################
# close context_menu and make it show up again
cmFrame.mouseClick()
sleep(config.SHORT_DELAY)
cmFrame.label.mouseClick(button=3)
sleep(config.SHORT_DELAY)

statesCheck(cmFrame.radio_item, 'MenuItem', add_states=['checked'])
cmFrame.radio_item.click()
sleep(config.SHORT_DELAY)
statesCheck(cmFrame.radio_item, 'MenuItem')

# click exit_item to close application frame window
cmFrame.exit_item.click()

# close application frame window
cmFrame.quit()
