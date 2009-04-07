#!/usr/bin/env python

##############################################################################
# Written by:  Neville Gao <nevillegao@gmail.com>
# Date:        02/18/2009
# Description: Test accessibility of contextmenu widget
#              Use the contextmenuframe.py wrapper script
#              Test the samples/contextmenu_menuitem.py script
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of contextmenu widget
"""

import sys

from strongwind import *
from contextmenu import *
from helpers import *


app_path = None
try:
    app_path = sys.argv[1]
except IndexError:
    pass #expected

# open the contextmenu sample application
try:
    app = launchContextMenu(app_path)
except IOError, e:
    print 'Error: %s' % e
    exit(2)

# make sure we got the app back
if app is None:
    exit(4)

# just an alias to make things shorter
cmFrame = app.contextMenuFrame

# open contextmenu and assert widgets
cmFrame.label.mouseClick(button=3)
sleep(config.SHORT_DELAY)
cmFrame.assertWidgets()

##############################################################################
# STATES: CONTEXT MENU
##############################################################################
statesCheck(cmFrame.context_menu, 'ContextMenu', add_states=['active'])

##############################################################################
# STATES: DEFAULT
##############################################################################
statesCheck(cmFrame.orig_item, 'MenuItem', add_states=['focusable'])
statesCheck(cmFrame.radio_item, 'MenuItem', add_states=['focusable', 'checked'])
statesCheck(cmFrame.check_item, 'MenuItem', add_states=['focusable', 'checked'])
statesCheck(cmFrame.exit_item, 'MenuItem', add_states=['focusable'])

##############################################################################
# CHECK ATKSELECTION
##############################################################################
# BUG 492784
#cmFrame.menu.selectChild(0)
#statesCheck(cmFrame.orig_item, 'MenuItem', add_states=['focused', 'focusable', 'selected'])

# close context_menu and make it show up again
cmFrame.mouseClick()
sleep(config.SHORT_DELAY)
cmFrame.label.mouseClick(button=3)
sleep(config.SHORT_DELAY)
cmFrame.assertWidgets()
sleep(config.SHORT_DELAY)

##############################################################################
# STATES: WHEN CONTEXTMENU SHOW UP
##############################################################################
# BUG
cmFrame.context_menu.keyCombo('<Down>', grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(cmFrame.orig_item, 'MenuItem', add_states=['focused', 'focusable'])

cmFrame.context_menu.keyCombo('<Down>', grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(cmFrame.radio_item, 'MenuItem', add_states=['focused', 'focusable', 'checked'])

cmFrame.context_menu.keyCombo('<Down>', grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(cmFrame.check_item, 'MenuItem', add_states=['focused', 'focusable', 'checked'])

cmFrame.context_menu.keyCombo('<Down>', grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(cmFrame.exit_item, 'MenuItem', add_states=['focused', 'focusable'])

##############################################################################
# STATES: CHECKED ITEMS
##############################################################################
# BUG 492770
#statesCheck(cmFrame.radio_item, 'MenuItem', add_states=['focusable', 'checked'])
#cmFrame.radio_item.mouseClick()
#sleep(config.SHORT_DELAY)
#cmFrame.label.mouseClick(button=3)
#sleep(config.SHORT_DELAY)
#cmFrame.assertWidgets()
#sleep(config.SHORT_DELAY)
#statesCheck(cmFrame.radio_item, 'MenuItem', add_states=['focusable'])

#statesCheck(cmFrame.check_item, 'MenuItem', add_states=['focusable', 'checked'])
#cmFrame.check_item.mouseClick()
#sleep(config.SHORT_DELAY)
#cmFrame.label.mouseClick(button=3)
#sleep(config.SHORT_DELAY)
#cmFrame.assertWidgets()
#sleep(config.SHORT_DELAY)
#statesCheck(cmFrame.check_item, 'MenuItem', add_states=['focusable'])

# close application frame window
cmFrame.quit()
