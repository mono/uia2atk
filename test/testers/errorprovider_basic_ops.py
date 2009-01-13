#!/usr/bin/env python
# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Brian G. Merrell <bgmerrell@novell.com>
# Date:        12/03/2008
# Description: Test accessibility of ErrorProvider widget 
#              Use the errorproviderframe.py wrapper script
#              Test the samples/errorprovider.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of ErrorProvider widget
"""
# imports
from errorprovider import *
from helpers import *
from states import *
from actions import *
from sys import argv
import sys

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the treeview sample application
try:
  app = launchErrorProvider(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

# make sure we got the app back
if app is None:
  exit(4)


# just an alias to make things shorter
epFrame = app.errorProviderFrame

# tab to the second text box, which should raise an error provider icon
epFrame.keyCombo("Tab", grabFocus=False)
sleep(config.SHORT_DELAY)

# assert that the error appeared
epFrame.assertSingleErrorAppeared(0, "Name required")

# keep tabbing through the other text boxes
epFrame.keyCombo("Tab", grabFocus=False)
sleep(config.SHORT_DELAY)

# assert that the error appeared
epFrame.assertSingleErrorAppeared(1, "Age required")

epFrame.keyCombo("Tab", grabFocus=False)
sleep(config.SHORT_DELAY)

# assert that the error appeared
epFrame.assertSingleErrorAppeared(2, "Weight required")

epFrame.keyCombo("Tab", grabFocus=False)
sleep(config.SHORT_DELAY)

# assert that the error appeared
epFrame.assertSingleErrorAppeared(3, "Height required")

epFrame.keyCombo("Tab", grabFocus=False)
sleep(config.SHORT_DELAY)

# assert that the error appeared
epFrame.assertSingleErrorAppeared(4, "Depth required")

# since all of the panels are up, let's check their states
epFrame.checkErrorProviderStates(5)

# now type some text and make sure the error providers go away
# we are at the first text box at this point

# type some text, issue a tab keystroke, and make sure the 0th error provider
# goes away

epFrame.typeText("Julio")
epFrame.keyCombo("Tab", grabFocus=False)
sleep(config.SHORT_DELAY)

# now the 0th panel should be the 1st panel from before (i.e., age)
# assert that the error appeared
epFrame.assertSingleErrorAppeared(0, "Age required")

# and so on...
epFrame.typeText("21")
epFrame.keyCombo("Tab", grabFocus=False)
sleep(config.SHORT_DELAY)
epFrame.assertSingleErrorAppeared(0, "Weight required")

epFrame.typeText("200")
epFrame.keyCombo("Tab", grabFocus=False)
sleep(config.SHORT_DELAY)
epFrame.assertSingleErrorAppeared(0, "Height required")

epFrame.typeText("6 foot 2 inches")
epFrame.keyCombo("Tab", grabFocus=False)
sleep(config.SHORT_DELAY)
epFrame.assertSingleErrorAppeared(0, "Depth required")

epFrame.typeText("4")
epFrame.keyCombo("Tab", grabFocus=False)
sleep(config.SHORT_DELAY)

# there shouldn't be any panels left
epFrame.assertNoPanels()

epFrame.quit()
