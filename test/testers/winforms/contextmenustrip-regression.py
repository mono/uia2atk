#!/usr/bin/env python

##############################################################################
# Written by:  Mike Gorse <mgorse@novell.com>
# Date:        02/23/2009
# Description: Test accessibility of contextmenustrip widget
#              Use the contextmenustripframe.py wrapper script
#              Test the samples/winforms/contextmenustrip.py script
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
try:
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
# BUG486335: extraneous "showing" state of menu item when it is not showing
#statesCheck(cmsFrame.item1a, 'MenuItem', invalid_states=['showing'])
#statesCheck(cmsFrame.item1b, 'MenuItem', invalid_states=['showing'])
# BUG509872: menu items receive "focused" state, but do not have "focusable" state
#statesCheck(cmsFrame.item2, 'MenuItem')
#statesCheck(cmsFrame.item3, 'MenuItem')
#statesCheck(cmsFrame.item4, 'MenuItem')
#statesCheck(cmsFrame.item5, 'MenuItem')

##############################################################################
# CHECK ATKSELECTION
##############################################################################
cmsFrame.menu.selectChild(cmsFrame.item1.getIndexInParent())
statesCheck(cmsFrame.item1, 'Menu', add_states=['focused', 'selected'])
# sub menu item should be displayed automatically
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
cmsFrame.context_menu_strip.keyCombo('<Down>', grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(cmsFrame.item1, 'Menu', add_states=['focused', 'selected']) 

cmsFrame.context_menu_strip.keyCombo('<Right>', grabFocus=False)
sleep(config.SHORT_DELAY)
# BUG509872: menu items receive "focused" state, but do not have "focusable" state
#statesCheck(cmsFrame.item1a, 'MenuItem', add_states=['focused', 'selected'])

cmsFrame.context_menu_strip.keyCombo('<Down>', grabFocus=False)
sleep(config.SHORT_DELAY)
# BUG509872: menu items receive "focused" state, but do not have "focusable" state
#statesCheck(cmsFrame.item1b, 'MenuItem', add_states=['focused', 'selected'])

cmsFrame.context_menu_strip.keyCombo('<Up>', grabFocus=False)
sleep(config.SHORT_DELAY)
# BUG509872: menu items receive "focused" state, but do not have "focusable" state
#statesCheck(cmsFrame.item1a, 'MenuItem', add_states=['focused', 'selected'])
statesCheck(cmsFrame.item1b, 'MenuItem')

cmsFrame.context_menu_strip.keyCombo('<Left>', grabFocus=False)
sleep(config.SHORT_DELAY)

cmsFrame.context_menu_strip.keyCombo('<Down>', grabFocus=False)
sleep(config.SHORT_DELAY)
# BUG509872: menu items receive "focused" state, but do not have "focusable" state
#statesCheck(cmsFrame.item2, 'MenuItem', add_states=['focused', 'selected'])

cmsFrame.context_menu_strip.keyCombo('<Down>', grabFocus=False)
sleep(config.SHORT_DELAY)
# BUG509872: menu items receive "focused" state, but do not have "focusable" state
#statesCheck(cmsFrame.item3, 'MenuItem', add_states=['focused', 'selected'])

cmsFrame.context_menu_strip.keyCombo('<Down>', grabFocus=False)
sleep(config.SHORT_DELAY)
# BUG509872: menu items receive "focused" state, but do not have "focusable" state
#statesCheck(cmsFrame.item4, 'MenuItem', add_states=['focused', 'selected'])

cmsFrame.context_menu_strip.keyCombo('<Down>', grabFocus=False)
sleep(config.SHORT_DELAY)
# BUG509872: menu items receive "focused" state, but do not have "focusable" state
#statesCheck(cmsFrame.item5, 'MenuItem', add_states=['focused', 'selected'])

cmsFrame.item1.mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(cmsFrame.item1a, 'MenuItem')
statesCheck(cmsFrame.item1b, 'MenuItem')
cmsFrame.assertText(cmsFrame.label, 'You have clicked Apple')

# use keyboard to navigate menu items and hit "Return" to select a menu item
cmsFrame.mouseClick()
sleep(config.SHORT_DELAY)
cmsFrame.label.mouseClick(button=3)
sleep(config.SHORT_DELAY)
cmsFrame.assertWidgets()
cmsFrame.context_menu_strip.keyCombo('<Down>', grabFocus=False)
sleep(config.SHORT_DELAY)
cmsFrame.context_menu_strip.keyCombo('<Down>', grabFocus=False)
sleep(config.SHORT_DELAY)
cmsFrame.context_menu_strip.keyCombo('<Return>', grabFocus=False)
sleep(config.SHORT_DELAY)
# BUG512103: when the ContextMenu have been destroied, "focused" state still remain on the menu item of the ContextMenu
#statesCheck(cmsFrame.item2, 'MenuItem', add_states=['defunct'], \
#    invalid_states=['visible', 'focusable', 'showing', 'enabled', 'sensitive', 'selectable'])
cmsFrame.assertText(cmsFrame.label, 'You have clicked Banana')

##############################################################################
# CHECK ATKIMAGE 
##############################################################################
# BUG 486290: menu item's image is not implemented
#cmsFrame.assertImage(cmsFrame.item1, 16, 16)

# close application frame window
cmsFrame.quit()
