#!/usr/bin/env python
# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        12/10/2008
# Description: main test script of toolstriplabel
#              ../samples/toolstriplabel.py is the test sample script
#              toolstriplabel/* are the wrappers of toolstriplabel test sample 
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of ToolStripLabel widget
"""

# imports
from toolstriplabel import *
from helpers import *
from states import *
from sys import argv

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the toolstriplabel sample application
try:
  app = launchToolStripLabel(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
tslFrame = app.toolStripLabelFrame

##############################
# check toolstriplabel's AtkAccessible
##############################
statesCheck(tslFrame.toolstriplabel, "Label")
statesCheck(tslFrame.toolstriplabel_image, "Label")
sleep(config.SHORT_DELAY)

##############################
# check toolstriplabel's AtkText
##############################
tslFrame.assertText(tslFrame.toolstriplabel, "Mono\nAccessibility")
tslFrame.assertText(tslFrame.toolstriplabel_image, "ToolStripLabel with image")
sleep(config.SHORT_DELAY)

##############################
# check toolstriplabel's AtkImage
##############################
# TODO: BUG482714, should assert the label's icon information
#tslFrame.assertImage(tslFrame.toolstriplabel_image, 16, 16)

##############################
# End
##############################
# close application frame window
tslFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
