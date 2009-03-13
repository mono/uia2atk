#!/usr/bin/env python
# -*- coding: utf8 -*-

##############################################################################
# Written by:  Brian G. Merrell <bgmerrell@novell.com>
# Date:        01/20/2009
# Description: Test accessibility of richtextbox widget 
#              Use the richtextboxframe.py wrapper script
#              Test the samples/richtextbox.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of richtextbox widget
"""

# imports
import sys
import os

from strongwind import *
from richtextbox import *
from helpers import *
from actions import *
from states import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the richtextbox sample application
try:
  app = launchRichTextBox(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
rtbFrame = app.richTextBoxFrame

# BUG478886 - RichTextBox text accessible does not receive "focused" state
# statesCheck(rtbFrame.richtextbox_top, "RichTextBox")
# statesCheck(rtbFrame.richtextbox_bottom, "RichTextBox")

# Mono WinForms BUG479646 - RichTextBox.LoadFile should not insert a newline
# at the end of the loaded text
# rtbFrame.assertEditableText(rtbFrame.richtextbox_top, rtbFrame.TEXT_TOP)
rtbFrame.assertEditableText(rtbFrame.richtextbox_bottom, rtbFrame.TEXT_BOTTOM)

# Mono WinForms BUG479646 - RichTextBox.LoadFile should not insert a newline
# rtbFrame.appendTextTest(rtbFrame.richtextbox_top, "Mono")
rtbFrame.appendTextTest(rtbFrame.richtextbox_bottom, "Mono")

# Mono WinForms BUG479646 - RichTextBox.LoadFile should not insert a newline
# rtbFrame.appendTextTest(rtbFrame.richtextbox_top, "Mono")
rtbFrame.prefixTextTest(rtbFrame.richtextbox_bottom, "Mono")

# Mono WinForms BUG479646 - RichTextBox.LoadFile should not insert a newline
# rtbFrame.deleteFromEndTest(rtbFrame.richtextbox_top, 20)
rtbFrame.deleteFromEndTest(rtbFrame.richtextbox_bottom, 5)

# Mono WinForms BUG479646 - RichTextBox.LoadFile should not insert a newline
# rtbFrame.deleteFromBeginningTest(rtbFrame.richtextbox_top, 15)
rtbFrame.deleteFromBeginningTest(rtbFrame.richtextbox_bottom, 3)

# Mono WinForms BUG479646 - RichTextBox.LoadFile should not insert a newline
# rtbFrame.deleteFromMiddleTest(rtbFrame.richtextbox_top, 5, 20)
rtbFrame.deleteFromMiddleTest(rtbFrame.richtextbox_bottom, 3, 7)

# Mono WinForms BUG479646 - RichTextBox.LoadFile should not insert a newline
# rtbFrame.insertTextTest(rtbFrame.richtextbox_top, "A!@#$%^&*Z", 7)
rtbFrame.insertTextTest(rtbFrame.richtextbox_bottom, "USD$1,000.00", 7)

sleep(10)

rtbFrame.quit()
