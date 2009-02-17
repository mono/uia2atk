#!/usr/bin/env python

##############################################################################
# Written by:  Neville Gao <nevillegao@gmail.com>
# Date:        02/16/2009
# Description: Test accessibility of treeview widget
#              Use the pagesetupdialog.py wrapper script
#              Test the samples/pagesetupdialog.py script
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of pagesetupdialog widget
"""

import sys

from strongwind import *
from pagesetupdialog import *


app_path = None
try:
    app_path = sys.argv[1]
except IndexError:
    pass #expected

# open the pagesetupdialog sample application
try:
    app = launchPageSetupDialog(app_path)
except IOError, e:
    print 'Error: %s' % e
    exit(2)

# make sure we got the app back
if app is None:
    exit(4)

# just an alias to make things shorter
psdFrame = app.pageSetupDialogFrame

# open pagesetupdialog and assert widgets
psdFrame.click(psdFrame.click_button)
sleep(config.SHORT_DELAY)
psdFrame.assertPageSetupDialog()
# close then reopen pagesetupdialog to make sure app doesn't crash
psdFrame.click(psdFrame.main_cancel_button)
sleep(config.SHORT_DELAY)
psdFrame.click(psdFrame.click_button)
sleep(config.SHORT_DELAY)
psdFrame.assertPageSetupDialog()
# open configurepagedialog and assert widgets
psdFrame.click(psdFrame.printer_button)
sleep(config.SHORT_DELAY)
psdFrame.assertConfigrePageDialog()
# close dialogs
psdFrame.click(psdFrame.configure_cancel_button)
sleep(config.SHORT_DELAY)
psdFrame.click(psdFrame.main_cancel_button)
sleep(config.SHORT_DELAY)

#close application frame window
psdFrame.quit()
