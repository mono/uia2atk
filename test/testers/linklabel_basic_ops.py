#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        10/22/2008
# Description: Test accessibility of linklabel widget 
#              Use the linklabelframe.py wrapper script
#              Test the samples/linklabel.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of linklabel widget
"""

# imports
import sys
import os

from strongwind import *
from linklabel import *
from helpers import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the label sample application
try:
  app = launchLinkLabel(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

sleep(config.SHORT_DELAY)

OPENSUSE = "http://www.opensuse.org/en/"
GMAIL = "http://www.gmail.com"
NOVELLMAIL = "https://gmail.novell.com/gw/webacc"
GCALCTOOL = "/usr/bin/gcalctool"

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
llFrame = app.linkLabelFrame

sleep(config.SHORT_DELAY)

#check sensitive Label's default states
statesCheck(llFrame.link1, "Label", add_states=["focusable", "focused"])
statesCheck(llFrame.link2, "Label", add_states=["focusable"])
statesCheck(llFrame.link3, "Label", add_states=["focusable"])

# check the number of links in each LinkLabel
llFrame.assertNLinks(llFrame.link1, 2)
llFrame.assertNLinks(llFrame.link2, 1)
llFrame.assertNLinks(llFrame.link3, 1)

# make sure the accessibles have the expected URIs
llFrame.assertURI(llFrame.link1, 0, OPENSUSE)
llFrame.assertURI(llFrame.link1, 1, NOVELLMAIL)
llFrame.assertURI(llFrame.link2, 0, GCALCTOOL)
llFrame.assertURI(llFrame.link3, 0, GMAIL)

# make sure we actually open the links
llFrame.openLink(llFrame.link1, "Firefox", 0, OPENSUSE, is_url = True)
llFrame.openLink(llFrame.link1, "Firefox", 1, NOVELLMAIL, is_url = True)
llFrame.openLink(llFrame.link2, "gcalctool", 0, GCALCTOOL, is_url = False)
llFrame.openLink(llFrame.link3, "Firefox", 0, GMAIL, is_url = True)

# press down arrow and check the states
llFrame.keyCombo("Down", grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(llFrame.link1, "Label", add_states=["focusable"])
statesCheck(llFrame.link2, "Label", add_states=["focusable", "focused"])
statesCheck(llFrame.link3, "Label", add_states=["focusable"])
# down arrow again should take us back to link1
llFrame.keyCombo("Down", grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(llFrame.link1, "Label", add_states=["focusable", "focused"])
statesCheck(llFrame.link2, "Label", add_states=["focusable"])
statesCheck(llFrame.link3, "Label", add_states=["focusable"])

# now try opening a link using mouseClick
llFrame.openLink(llFrame.link2, "gcalctool", 0, GCALCTOOL, is_url = False, mouse_click = True)

print "INFO:  Log written to: %s" % config.OUTPUT_DIR

#close application frame window
llFrame.quit()
