#!/usr/bin/env python
# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        02/09/2009
# Description: main test script of containercontrol
#              ../samples/winforms/containercontrol.py is the test sample script
#              containercontrol/* is the wrapper of containercontrol test sample script
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of ContainerControl widget
"""

# imports
from containercontrol import *
from helpers import *
from states import *
from sys import argv

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the containercontrol sample application
try:
  app = launchContainerControl(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

sleep(config.SHORT_DELAY)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
ccFrame = app.containerControlFrame

##############################
# check ContainerControl and its children's AtkAccessible
##############################
statesCheck(ccFrame.panel_top, "ContainerControl", add_states=["focused"])
statesCheck(ccFrame.panel_bottom, "ContainerControl")
statesCheck(ccFrame.label_top, "Label")
statesCheck(ccFrame.label_bottom, "Label")

# move focus to the bottom panel
ccFrame.keyCombo("Tab", grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(ccFrame.panel_top, "ContainerControl")
statesCheck(ccFrame.panel_bottom, "ContainerControl", add_states=["focused"])
statesCheck(ccFrame.label_top, "Label")
statesCheck(ccFrame.label_bottom, "Label")
ccFrame.assertText(ccFrame.label_top, "I lose focus")
ccFrame.assertText(ccFrame.label_bottom, "I got it")

# move focus to the top panel
ccFrame.keyCombo("Down", grabFocus=False)
sleep(config.SHORT_DELAY)
statesCheck(ccFrame.panel_top, "ContainerControl", add_states=["focused"])
statesCheck(ccFrame.panel_bottom, "ContainerControl")
statesCheck(ccFrame.label_top, "Label")
statesCheck(ccFrame.label_bottom, "Label")
ccFrame.assertText(ccFrame.label_top, "I got it")
ccFrame.assertText(ccFrame.label_bottom, "I lose focus")

##############################
# End
##############################
ccFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
