#!/usr/bin/env python

############################################################################## 
# Written by:  Cachen Chen <cachen@novell.com> 
#              Felicia Mu <fxmu@novell.com> 
# Date:        07/16/2009
# Description: Test accessibility of printpreviewdialog widget 
#              Use the printpreviewdialogframe.py wrapper script
#              Test the samples/printpreviewdialog.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of printpreviewdialog widget 

"""

# imports
import sys
import os

from strongwind import *
from printpreviewdialog import *
from helpers import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the printpreviewdialog sample application
try:
  app = launchPrintPreviewDialog(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
ppdFrame = app.printPreviewDialogFrame

# click button to show PrintPreviewDialog page
ppdFrame.start_button.click(log=True)
sleep(config.MEDIUM_DELAY)

# BUG465958 - PrintPreviewControl: printing window is hided that is
# inconsistant with Vista does
# find the Printing frame
# ppdFrame.findPringtingFrameAccessibles()

# find controls under PrintPreviewDialog                           
ppdFrame.findLaunchedDialogAccessibles()

###########################################
# assert the number of accessibles
###########################################
ppdFrame.assertNumOfAccessibles(ppdFrame.pushbuttons,\
                                    ppdFrame.NUM_DIALOG_PUSH_BUTTONS)
ppdFrame.assertNumOfAccessibles(ppdFrame.panels, \
                                          ppdFrame.NUM_DIALOG_PANELS)
ppdFrame.assertNumOfAccessibles(ppdFrame.separators, \
                                      ppdFrame.NUM_DIALOG_SEPARATORS)

######################################
# check the  default states of all accessibles
######################################
# check the states of start button

# test dialog states
statesCheck(ppdFrame.dialog, "Dialog", add_states=["modal"])

# test panels states
for panel in ppdFrame.panels:
   statesCheck(panel, "Panel")    

statesCheck(ppdFrame.print_preview_panel, "Panel" ,add_states=["focusable"])

# BUG473757 - PrintPreviewControl in PrintPreviewDialog does not receive focused state
#ppdFrame.print_preview_panel.mouseClick()
#sleep(config.MEDIUM_DELAY)
#
#statesCheck(ppdFrame.print_preview_panel, "Panel" ,add_states=["focusable", "focused"])
   
# test tool bar states
statesCheck(ppdFrame.toolbar, "ToolBar")

# test push buttons states
for push_button in ppdFrame.pushbuttons:
    statesCheck(push_button, "Button")

# test toggle button states
statesCheck(ppdFrame.toggle, "ToggleButton")

# test spin button states
statesCheck(ppdFrame.spinbutton, "Button", add_states=["single line", "editable"])

# test separators states
for separator in ppdFrame.separators:
    statesCheck(separator, "ToolStripSeparator")

# test label states
statesCheck(ppdFrame.label, "Label")

# test tooltips states
# if BUG508593 is fixed , please uncomment the following codes
#for tooltip in ppdFrame.tooltips:
#statesCheck(tooltip, "ToolTip")

####################################################
# check actions of controls under PrintPreviewDialog                           
####################################################
# test push buttons actions
for push_button in ppdFrame.pushbuttons:
    actionsCheck(push_button, "Button", add_actions=["click"])

# test toggle button actions
actionsCheck(ppdFrame.toggle, "ToggleButton", add_actions=["click"])

# test spin button actions
actionsCheck(ppdFrame.spinbutton, "Button", add_actions=["activate"], \
                                                      invalid_actions=["click"])

################################################
# test buttons on tool bar can raise 'focused'
################################################
# test push buttons states
ppdFrame.close_button.grabFocus()
sleep(config.SHORT_DELAY)
statesCheck(ppdFrame.close_button, "Button", add_states=['focused'])

# 521948 - PrintPreivewDialog: pushbuttons on toolbar 
# are key focusable but lack 'focused' state
# for push_button in ppdFrame.pushbuttons:
#    if push_button.name != ppdFrame.close_button.name:
#        push_button.mouseClick()
#        sleep(config.SHORT_DELAY)
#        statesCheck(push_button, "Button", add_states=['focused'])

# test toggle button states
# BUG522614 - PrintPreviewDialog: the 'toggle button' on 
# tool bar doesn't rise 'focused' and 'armed', 'checked' states
#ppdFrame.toggle.mouseClick()
#sleep(config.SHORT_DELAY)
#statesCheck(ppdFrame.toggle, "ToggleButton", \
#                                add_states = ['focused','armed','checked'])

# BUG522636 - Toggle Button(with drop down style): the 
# "click" action performs different from mouse click action.
#ppdFrame.toggle.click()
#sleep(config.SHORT_DELAY)
#statesCheck(ppdFrame.toggle, "ToggleButton", \
#                                add_states = ['focused','armed','checked'])
# BUG509344 - PrintPreivewDialog: clicking zoom toggle button 
# the second time doesn't close the menu
# ppdFrame.zoom_window.assertClosed()
#
# test spin button states
ppdFrame.spinbutton.grabFocus()
statesCheck(ppdFrame.spinbutton, "Button", \
                            add_states=["single line", "editable","focused"])

ppdFrame.spinbutton.mouseClick()
sleep(config.SHORT_DELAY)
statesCheck(ppdFrame.spinbutton, "Button", \
                            add_states=["single line", "editable","focused"])

###########################################
# test ZoomDropDown Accessibles
###########################################
# click the toggle button to raise the drop down menu
ppdFrame.toggle.click(log=True)
sleep(config.SHORT_DELAY)

# find all the accessibles on the the menu
ppdFrame.findZoomDropDownAccessibles()

# check their default states
statesCheck(ppdFrame.zoom_window,"Form",invalid_states=["resizable"],\
                                                      add_states=["active"])
# BUG509165 PrintPreviewDialog: zoom "menu" accessible has extraneous
# "focused" state
#statesCheck(self.zoom_menu, "Menu", invalid_states=["selectable"])

for menu_item in ppdFrame.zoom_menu_items:
    if menu_item.name == "Auto":
        statesCheck(menu_item, "MenuItem", add_states=["checked"])
    # BUG509276 - PrintPreviewDialog: All zoom menu items have the
    # "checked" state
    #else:
    #    statesCheck(menu_item, "MenuItem")

# check their default actions
for menu_item in ppdFrame.zoom_menu_items:
      actionsCheck(menu_item, "MenuItem")

# assert the num of menu items
ppdFrame.assertNumOfAccessibles(ppdFrame.zoom_menu_items, \
                                      ppdFrame.NUM_ZOOM_MENU_ITEMS)

# do 'click' action on menu item
#for i in range(2,len(ppdFrame.zoom_menu_items)):
#    ppdFrame.zoom_menu_items[i].click()
#    #BUG510829 - ContextMenu: keyUp/Down doesn't rise 
#    #focused and selected states for menu items
#    #statesCheck(ppdFrame.zoom_menu_items[i], "MenuItem", \
#    #                         add_states=["checked","focused","selected"])
#    # BUG509276 - PrintPreviewDialog: All zoom menu items have the
#    # "checked" state
#    #statesCheck(ppdFrame.zoom_menu_items[i-1], "MenuItem")
  
################################################
# test numericupdown Accessibles
################################################
ppdFrame.assignNumericUpdownValue(5)
sleep(config.SHORT_DELAY)
ppdFrame.assertNumericUpdownValue(5)

ppdFrame.assignNumericUpdownValue(0)
sleep(config.SHORT_DELAY)
ppdFrame.assertNumericUpdownValue(0)

ppdFrame.spinbutton.mouseClick()
sleep(config.SHORT_DELAY)
ppdFrame.spinbutton.mouseClick()
sleep(config.SHORT_DELAY)
ppdFrame.keyCombo("Up", grabFocus=False)
sleep(config.SHORT_DELAY)
ppdFrame.assertNumericUpdownValue(1)

ppdFrame.keyCombo("Down", grabFocus=False)
sleep(config.SHORT_DELAY)
ppdFrame.assertNumericUpdownValue(0)

# TODO: is there a good way to test to make sure our clicking has
# had the expected affects on the GUI?

################################################
# test close and reopen the dialog
################################################
# test click action for close push button
ppdFrame.close_button.click(log=True)
sleep(config.SHORT_DELAY)
ppdFrame.dialog.assertClosed()
  
# BUG521440 - PrintPreviewDialog: PrintPreviewDialog dialog 
# which is reopened twice is not accessible
# click button to show PrintPreviewDialog dialog
#ppdFrame.button.click(log=True)
#sleep(config.MEDIUM_DELAY)
# find all of the PrintPreviewDialog controls
#ppdFrame.findAllPrintPreviewDialogAccessibles()
  
# close dialog window
#ppdFrame.dialog.altF4()

# TODO:
# Bug 523388 - PrintPreviewDialog:the Printing 
# frame has the wrong position and size
# Bug 508599 - PrintPreviewDialog: Default control focus is not consistent
# 509114 - PrintPreviewDialog: Zoom drop-down menu stays open even when 
# other buttons are clicked
# Bug 509127 - PrintPreviewDialog: Zoom drop-down "window" accessible 
# has wacky component interface info

# close application frame window
# if BUG521440 is fixed , please uncomment these codes
#ppdFrame.close_button.click(log=True)
#sleep(config.SHORT_DELAY)
ppdFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
