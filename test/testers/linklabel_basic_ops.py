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

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
llFrame = app.linkLabelFrame

#check sensitive Label's default states
statesCheck(llFrame.link1, "Label", add_states=["focusable", "focused"])
statesCheck(llFrame.link2, "Label", add_states=["focusable"])
statesCheck(llFrame.link3, "Label", add_states=["focusable"])

#implement Hypertext, check the link number
llFrame.showLink(llFrame.link1, 'www.opensuse.org', 2)
llFrame.showLink(llFrame.link2, 'calculator')
llFrame.showLink(llFrame.link3, 'gedit')

#do 'jump' action for link1 to invoke firefox, then close firefox
llFrame.openLink(llFrame.link1)
sleep(config.MEDIUM_DELAY)
llFrame.assertLinkable("Firefox")
newapp = launchNewApp("Firefox")
newframe = newapp.findFrame(re.compile("openSUSE.org"))
newframe.mouseClick()
newframe.altF4()

#do 'jump' action for link2 to invoke gclctool, then close gclctool
llFrame.openLink(llFrame.link2)
sleep(config.MEDIUM_DELAY)
llFrame.assertLinkable("gcalctool")
newapp = launchNewApp("gcalctool")
newapp.findFrame("Calculator").altF4()

#doing 'jump' action for link3 doesn't invoke gedit
llFrame.openLink(llFrame.link3)
sleep(config.MEDIUM_DELAY)
llFrame.assertLinkable("gmail")

#invoke firefox from linklabel1, then close it
llFrame.keyCombo("Return", grabFocus=False)
sleep(config.MEDIUM_DELAY)
llFrame.assertLinkable("Firefox")
newapp = launchNewApp("Firefox")
newframe = newapp.findFrame(re.compile("openSUSE.org"))
newframe.mouseClick()
newframe.altF4()

#invoke gcalctool from linklabel2, then close it
llFrame.link2.mouseClick()
sleep(config.MEDIUM_DELAY)
llFrame.assertLinkable("gcalctool")
newapp = launchNewApp("gcalctool")
newapp.findFrame("Calculator").altF4()

#un-invoke gedit from linklabel3
llFrame.link3.mouseClick()
sleep(config.MEDIUM_DELAY)
llFrame.assertLinkable("gmail")

print "INFO:  Log written to: %s" % config.OUTPUT_DIR

#close application frame window
llFrame.quit()
