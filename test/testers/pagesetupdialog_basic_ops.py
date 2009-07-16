#!/usr/bin/env python

##############################################################################
# Written by:  Neville Gao <nevillegao@gmail.com>
# Date:        02/16/2009
# Description: Test accessibility of pagesetupdialog widget
#              Use the pagesetupdialogframe.py wrapper script
#              Test the samples/pagesetupdialog.py script
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of pagesetupdialog widget
"""

import sys

from strongwind import *
from pagesetupdialog import *
from helpers import *


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
psdFrame.click_button.click(log=True)
sleep(config.SHORT_DELAY)
psdFrame.findPageSetupDialogAccessibles()

# test actions
psdFrame.pageSetupDialogAccessiblesAction()

# test default states
psdFrame.pageSetupDialogAccessiblesStates()

# test radiobutton on Orientation panel
psdFrame.landscape_radio.click(log=True)
sleep(config.SHORT_DELAY)
statesCheck(psdFrame.landscape_radio, "RadioButton", add_states=["checked"])
statesCheck(psdFrame.portrait_radio, "RadioButton")

psdFrame.portrait_radio.click(log=True)
sleep(config.SHORT_DELAY)
statesCheck(psdFrame.portrait_radio, "RadioButton", add_states=["checked"])
statesCheck(psdFrame.landscape_radio, "RadioButton")

# mouse click area panel to raise focused
psdFrame.area_panel.mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(psdFrame.area_panel, "Panel", add_states=["focusable", "focused"])

# test editable textboxs on Margins panel
psdFrame.changeText(psdFrame.left_text, "10")

psdFrame.changeText(psdFrame.right_text, "0")

psdFrame.changeText(psdFrame.top_text, "0")

psdFrame.changeText(psdFrame.bottom_text, "20")

# close then reopen pagesetupdialog to make sure app doesn't crash
psdFrame.main_cancel_button.click(log=True)
sleep(config.SHORT_DELAY)
psdFrame.main_dialog.assertClosed()

# open pagesetupdialog and assert widgets again
psdFrame.click_button.click(log=True)
sleep(config.SHORT_DELAY)
psdFrame.findPageSetupDialogAccessibles()

# open configurepagedialog and assert widgets
psdFrame.printer_button.click(log=True)
sleep(config.SHORT_DELAY)
psdFrame.findConfigrePageDialogAccessibles()

# test actions
psdFrame.configurePageDialogAccessiblesActions()

# test default states
psdFrame.configurePageDialogAccessiblesStates()

# close configurepagedialog
psdFrame.configure_cancel_button.click(log=True)
sleep(config.SHORT_DELAY)
psdFrame.configure_dialog.assertClosed()

# close pagesetupdialog
psdFrame.main_cancel_button.click(log=True)
sleep(config.SHORT_DELAY)
psdFrame.main_dialog.assertClosed()

# close application frame window
psdFrame.quit()
