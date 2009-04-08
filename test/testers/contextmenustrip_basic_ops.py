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
cmsFrame = app.contextMenuStripFrame

# open contextmenustrip and assert widgets
cmsFrame.label.mouseClick(button=3)
sleep(config.SHORT_DELAY)
cmsFrame.assertWidgets()

##############################################################################
# STATES: CONTEXT MENU STRIP
##############################################################################
statesCheck(cmsFrame.context_menu_strip, 'ContextMenuStrip', add_states=['active'])

##############################################################################
# STATES: DEFAULT
##############################################################################
statesCheck(cmsFrame.item1, 'Menu')
statesCheck(cmsFrame.item1a, 'MenuItem', add_states=['showing'])
statesCheck(cmsFrame.item1b, 'MenuItem', add_states=['showing'])
statesCheck(cmsFrame.item2, 'MenuItem')
statesCheck(cmsFrame.item3, 'MenuItem')
statesCheck(cmsFrame.item4, 'MenuItem')
statesCheck(cmsFrame.item5, 'MenuItem')

##############################################################################
# CHECK ATKSELECTION
##############################################################################
cmsFrame.menu.selectChild(0)
# BUG 479397
#statesCheck(cmsFrame.item1, 'MenuItem', add_states=['focused', 'selected'])
# sub menu item should display automatically
statesCheck(cmsFrame.item1a, 'MenuItem')
statesCheck(cmsFrame.item1b, 'MenuItem')

# close context_menu and make it show up again
cmsFrame.mouseClick()
sleep(config.SHORT_DELAY)
cmsFrame.label.mouseClick(button=3)
sleep(config.SHORT_DELAY)
cmsFrame.assertWidgets()

##############################################################################
# STATES: WHEN CONTEXTMENUSTRIP SHOW UP
##############################################################################
# BUG 471405
#cmsFrame.context_menu_strip.keyCombo('<Down>', grabFocus=False)
#sleep(config.SHORT_DELAY)
#statesCheck(cmsFrame.item1, 'MenuItem', add_states=['focused', 'selected'])

#cmsFrame.context_menu_strip.keyCombo('<Down>', grabFocus=False)
#sleep(config.SHORT_DELAY)
#statesCheck(cmsFrame.item2, 'MenuItem', add_states=['focused', 'selected'])

#cmsFrame.context_menu_strip.keyCombo('<Down>', grabFocus=False)
#sleep(config.SHORT_DELAY)
#statesCheck(cmsFrame.item3, 'MenuItem', add_states=['focused', 'selected'])

#cmsFrame.context_menu_strip.keyCombo('<Down>', grabFocus=False)
#sleep(config.SHORT_DELAY)
#statesCheck(cmsFrame.item4, 'MenuItem', add_states=['focused', 'selected'])

#cmsFrame.context_menu_strip.keyCombo('<Down>', grabFocus=False)
#sleep(config.SHORT_DELAY)
#statesCheck(cmsFrame.item5, 'MenuItem', add_states=['focused', 'selected'])

#cmsFrame.item1.mouseClick()
#sleep(config.SHORT_DELAY)
#statesCheck(cmsFrame.item1a, 'MenuItem')
#statesCheck(cmsFrame.item1b, 'MenuItem')

# close application frame window
cmsFrame.quit()
