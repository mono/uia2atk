#!/usr/bin/env python
# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Brian G. Merrell <bgmerrell@novell.com>
# Date:        12/03/2008
# Description: Test accessibility of HelpProvider widget 
#              Use the helpproviderframe.py wrapper script
#              Test the samples/helpprovider.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of HelpProvider widget
"""
# imports
from helpprovider import *
from helpers import *
from states import *
from actions import *
from sys import argv

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the treeview sample application
try:
  app = launchHelpProvider(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

# make sure we got the app back
if app is None:
  exit(4)


# just an alias to make things shorter
hpFrame = app.helpProviderFrame

sleep(config.MEDIUM_DELAY)

hpFrame.quit()
