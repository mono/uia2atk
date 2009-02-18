#!/usr/bin/env python
# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        11/19/2008
# Description: main test script of menustrip
#              ../samples/menustrip.py is the test sample script
#              menustrip/* is the wrapper of menustrip test sample script
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of MenuStrip widget
"""

# imports
from menustrip import *
from helpers import *
from states import *
from sys import argv

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the menustrip sample application
try:
  app = launchMenuStrip(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

sleep(config.SHORT_DELAY)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
msFrame = app.menuStripFrame

##############################
# check MenuStrip and its children's AtkAccessible
##############################
# check states of menustrip
statesCheck(msFrame.menustrip, "MenuStrip")
statesCheck(msFrame.menuitem_file, "MenuItem")
statesCheck(msFrame.menuitem_edit, "MenuItem")

##############################
# check MenuStrip's AtkSelection
##############################
msFrame.assertSelectChild(msFrame.menustrip, 0)
# BUG476362
#statesCheck(msFrame.menuitem_file, "MenuItem", add_states=["selected"])
statesCheck(msFrame.menuitem_edit, "MenuItem")

msFrame.assertSelectChild(msFrame.menustrip, 1)
statesCheck(msFrame.menuitem_file, "MenuItem")
# BUG476362
#statesCheck(msFrame.menuitem_edit, "MenuItem", add_states=["selected"])

##############################
# check MenuStrip and its children's AtkComponent
##############################
msFrame.menustrip.mouseClick()
sleep(config.SHORT_DELAY)
msFrame.assertText(msFrame.label, "You are clicking ")

msFrame.menuitem_file.mouseClick()
sleep(config.SHORT_DELAY)
msFrame.assertText(msFrame.label, "You are clicking File")

msFrame.menuitem_edit.mouseClick()
sleep(config.SHORT_DELAY)
msFrame.assertText(msFrame.label, "You are clicking Edit")

##############################
# End
##############################
# close application frame window
msFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
