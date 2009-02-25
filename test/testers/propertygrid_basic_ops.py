#!/usr/bin/env python

##
# Written by:  Andres G. Aragoneses <aaragoneses@novell.com>
# Date:        02/24/2009
# Description: Test accessibility of PropertyGrid widget 
#              Use the propertygridframe.py wrapper script
#              Test the samples/propertygrid.py script
##

# The docstring below  is used in the generated log file
"""
Test accessibility of PropertyGrid widget
"""

# imports
import sys
import os

from strongwind import *
from propertygrid import *
from helpers import *
from eventlistener import *
from sys import argv
from os import path
import pyatspi

app_path = None 
try:
	app_path = argv[1]
except IndexError:
	pass #expected

# open the datagridview sample application
try:
	app = launchPropertyGrid(app_path)
except IOError, msg:
	print "ERROR:  %s" % msg
	exit(2)

# make sure we got the app back
if app is None:
	exit(4)

# Alias to make things shorter
pgFrame = app.propertyGridFrame

notImplemented = False
try:
	actionsCheck(pgFrame.visible, "TableCell")
except NotImplementedError:
	notImplemented = True
assert notImplemented == True

#uncomment this when bug 479228 is fixed
#try:
#	pgFrame.visible._accessible.queryEditableText().copyText(0, 1)
#except AttributeError:
#	notImplemented = True
#assert notImplemented == False


sleep(config.SHORT_DELAY)

pgFrame.alphabeticbutton._accessible.queryAction().doAction(0)

sleep(config.SHORT_DELAY)

notFound = False
try:
	pgFrame.findTableCell(pgFrame.CELL_A11Y, checkShowing=False)
except SearchError:
	notFound = True
assert notFound == True


pgFrame.categorizedbutton._accessible.queryAction().doAction(0)
sleep(config.SHORT_DELAY)

#this test is heavily dependant on toolstripbutton bugs, uncomment this when they get fixed:
#pgFrame.findTableCell(pgFrame.CELL_A11Y, checkShowing=False)

#use index = 1 instead of 0 when bug 479113 is fixed:

assert pgFrame.tablecells[0].name == pgFrame.CELL_ACCESSIBLE_DESC

#figure out why this doesn't work:
pgFrame.treetable._accessible.querySelection().addSelection(0)
sleep(config.SHORT_DELAY)

pgFrame.textpanel.findLabel(pgFrame.CELL_ACCESSIBLE_DESC)

#close application frame window
pgFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
