#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        12/11/2008
# Description: Test accessibility of toolstriptextbox widget 
#              Use the toolstriptextboxframe.py wrapper script
#              Test the samples/toolstriptextbox.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of toolstriptextbox widget
"""

# imports
import sys
import os

from strongwind import *
from toolstriptextbox import *
from helpers import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the toolstriptextbox sample application
try:
  app = launchToolStripTextBox(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
tstbFrame = app.toolStripTextBoxFrame

# check states list for text box
statesCheck(tstbFrame.singleline, "ToolStripTextBox", \
                           add_states=["single line"])
statesCheck(tstbFrame.multiline, "ToolStripTextBox", \
                           add_states=["multi line"])
statesCheck(tstbFrame.readonly, "ToolStripTextBox", \
                           add_states=["single line"], \
                           invalid_states=["editable"])
# mouse click textbox to rise focused
tstbFrame.singleline.mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(tstbFrame.singleline, "ToolStripTextBox", \
                           add_states=["single line", "focused"])

tstbFrame.multiline.mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(tstbFrame.multiline, "ToolStripTextBox", \
                           add_states=["multi line", "focused"])

tstbFrame.readonly.mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(tstbFrame.readonly, "ToolStripTextBox", \
                           add_states=["single line", "focused"], \
                           invalid_states=["editable"])

# in textbox1(single line), input characters in application or change text value
# will change label and text, just 10 characters is valid
tstbFrame.singleline.mouseClick()
sleep(config.SHORT_DELAY)
tstbFrame.singleline.typeText("toolstriptextbox1")
sleep(config.SHORT_DELAY)
tstbFrame.assertLabel("toolstript")
tstbFrame.assertText(tstbFrame.singleline,"toolstript")

tstbFrame.setTextValue(tstbFrame.singleline, "setTextValue")
sleep(config.SHORT_DELAY)
tstbFrame.assertLabel("setTextVal")

# in textbox2(multi line), input characters in application or change text value
# will change label and text with multi line
tstbFrame.multiline.mouseClick()
sleep(config.SHORT_DELAY)
tstbFrame.multiline.typeText("toolstrip")
tstbFrame.multiline.keyCombo("Return", grabFocus=True)
tstbFrame.multiline.typeText("textbox2")
sleep(config.SHORT_DELAY)
## false moving to next line, MWF BUG486716
#tstbFrame.assertLabel("toolstrip\ntextbox2")
#tstbFrame.assertText(tstbFrame.multiline,"toolstrip\ntextbox2")

tstbFrame.setTextValue(tstbFrame.multiline, "multi\r\nline")
sleep(config.SHORT_DELAY)
tstbFrame.assertLabel("multi\r\nline")

# in textbox3(read only), input characters in application or change text value 
# won't change label and text
tstbFrame.readonly.mouseClick()
sleep(config.SHORT_DELAY)
tstbFrame.readonly.typeText("toolstriptextbox3")
sleep(config.SHORT_DELAY)
tstbFrame.assertLabel("multi\r\nline")
tstbFrame.assertText(tstbFrame.readonly,"")

tstbFrame.setTextValue(tstbFrame.readonly, "readonly")
sleep(config.SHORT_DELAY)
tstbFrame.assertLabel("multi\r\nline")

# Text doesn't show AccessibleName and AccessibleDescription via MSDN
tstbFrame.assertNameAndDescription(tstbFrame.singleline, "", "")

tstbFrame.assertNameAndDescription(tstbFrame.multiline, "", "")

tstbFrame.assertNameAndDescription(tstbFrame.readonly, "", "")

# test Streamable Contents for three textbox
tstbFrame.assertStreamableContent(tstbFrame.singleline)

tstbFrame.assertStreamableContent(tstbFrame.multiline)

tstbFrame.assertStreamableContent(tstbFrame.readonly)


print "INFO:  Log written to: %s" % config.OUTPUT_DIR

# close application frame window
tstbFrame.quit()
