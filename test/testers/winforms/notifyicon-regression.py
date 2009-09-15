#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        09/16/2008
# Description: Test accessibility of notifyicon widget 
#              Use the notifyiconframe.py wrapper script
#              Test the samples/winforms/novifyicon.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of novifyicon widget
"""

# imports
import sys
import os

from strongwind import *
from notifyicon import *
from helpers import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the notifyicon sample application
try:
  app = launchNotifyIcon(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
niFrame = app.notifyIconFrame

# click on the NotifyIcon button, which puts a notify icon in the notification
# area of the gnome-panel
niFrame.notifyicon_button.click(log=True)
sleep(config.SHORT_DELAY)

# find the new notification icon that appeared
niFrame.findNotifyIcon()

# check states of the notifycation icon
statesCheck(niFrame.notify_icon, "Panel")

# right click on the notify icon
# BUG507281 - NotifyIcon: Context menu of NotifyIcon icon is not accessible
#niFrame.notify_icon.mouseClick(button=3)

# When BUG507281 is fixed, it would probably be good to make a test case that
# uses the icon's context menu to "close" the icon (using the accessible's
# action interface) and then make sure the icon goes away.

# click button to raise balloon alert
niFrame.balloon_button.click(log=True)
sleep(config.SHORT_DELAY)

# Alert with wrong name BUG476859
#niFrame.findBalloonAccessibles()

# check states
# missing "mobal" and has extra "resizable" BUG476862
#statesCheck(niFrame.balloon_alert, "Alert")
# incorrect states BUG476906
#statesCheck(niFrame.label, "Label")
# incorrect states BUG476871
#statesCheck(niFrame.icon, "Icon")

# balloon alert disappeared after mouse click it
# niFrame.balloon_alert can be used here (instead of calling findAlert again)
# when BUG476859 is fixed
balloon = niFrame.app.findAlert(None)
balloon.mouseClick()
sleep(config.SHORT_DELAY)
balloon.assertClosed()

# close application frame window
niFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
