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
#cmFrame.label.mouseClick(button=3)
cmFrame.mClick(cmFrame.label)
sleep(config.SHORT_DELAY)
cmFrame.assertWidgets()
# close application frame window
#cmFrame.click(cmFrame.exit_item)
cmFrame.exit_item.click()

# close application frame window
#cmFrame.quit()
