#!/usr/bin/env python
# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        12/07/2008
# Description: main test script of domainupdown
#              ../samples/domainupdown.py is the test sample script
#              domainupdown/* is the wrapper of domainupdown test sample script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of domainupdown widget
"""
# imports
from domainupdown import *
from helpers import *
from states import *
from actions import *
from sys import argv

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the domainupdown sample application
try:
  app = launchDomainUpDown(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
dudFrame = app.domainUpDownFrame

##############################
# check domainupdown's states
##############################
statesCheck(dudFrame.editable_domainupdown, "DomainUpDown", add_states=["focused"])
# VERIFYME: comment this due to bug457496
#statesCheck(dudFrame.uneditable_domainupdown, "DomainUpDown", invalid_states=["editable"])
statesCheck(dudFrame.uneditable_domainupdown, "DomainUpDown")

# move the focused to uneditable_domainupdown then check the states again
dudFrame.uneditable_domainupdown.mouseClick()
statesCheck(dudFrame.editable_domainupdown, "DomainUpDown")
# VERIFYME: comment this due to bug457496
#statesCheck(dudFrame.uneditable_domainupdown, "DomainUpDown", invalid_states=["editable"], add_states=["focused"])
statesCheck(dudFrame.uneditable_domainupdown, "DomainUpDown", add_states=["focused"])

##############################
# input text from UI
##############################
# editable DomainUpDown
dudFrame.editable_domainupdown.mouseClick()
dudFrame.editable_domainupdown.typeText("provo")
sleep(config.SHORT_DELAY)
dudFrame.assertText(dudFrame.editable_domainupdown, "Provo")
# check the state of selected item
# VERIFYME: the item which is selected by typing characters is lack of 'selected', 'showing' and 'visible' states
#statesCheck(dudFrame.editable_domainupdown.listitem[4], "ListItem", add_states=["selected"])
statesCheck(dudFrame.editable_domainupdown.listitem[4], "ListItem", invalid_states=["showing", "visible"])
# check other items' states
statesCheck(dudFrame.editable_domainupdown.listitem[3], "ListItem", invalid_states=["showing", "visible"])

# VERIFYME: we should not be able to input, but now you can due to bug457496
# uneditable DomainUpDown
dudFrame.uneditable_domainupdown.mouseClick()
dudFrame.uneditable_domainupdown.typeText("cambridge")
sleep(config.SHORT_DELAY)
dudFrame.assertText(dudFrame.uneditable_domainupdown, "Cambridge")
# check the state of selected item
# VERIFYME: the item which is selected by typing characters is lack of 'selected', 'showing' and 'visible' states
#statesCheck(dudFrame.uneditable_domainupdown.listitem[2], "ListItem", add_states=["selected"])
statesCheck(dudFrame.uneditable_domainupdown.listitem[2], "ListItem", invalid_states=["showing", "visible"])
# check other items' states
statesCheck(dudFrame.uneditable_domainupdown.listitem[3], "ListItem", invalid_states=["showing", "visible"])

#############################
# input text from AtkText
#############################
# editable DomainUpDown
dudFrame.editable_domainupdown.mouseClick()
dudFrame.inputText(dudFrame.editable_domainupdown, "Boston")
sleep(config.SHORT_DELAY)
dudFrame.assertText(dudFrame.editable_domainupdown, "Boston")
dudFrame.inputText(dudFrame.editable_domainupdown, "Provo")
sleep(config.SHORT_DELAY)
dudFrame.assertText(dudFrame.editable_domainupdown, "Provo")
# check the state of selected item
# VERIFYME: the item which is selected by typing characters is lack of 'selected', 'showing' and 'visible' states
#statesCheck(dudFrame.editable_domainupdown.listitem[4], "ListItem", add_states=["selected"])
statesCheck(dudFrame.editable_domainupdown.listitem[4], "ListItem", invalid_states=["showing", "visible"])
# check other items' states
statesCheck(dudFrame.editable_domainupdown.listitem[3], "ListItem", invalid_states=["showing", "visible"])

# uneditable DomainUpDown
dudFrame.uneditable_domainupdown.mouseClick()
dudFrame.inputText(dudFrame.uneditable_domainupdown, "Boston")
sleep(config.SHORT_DELAY)
# the text will not be changed since it is readonly spin button
dudFrame.assertText(dudFrame.uneditable_domainupdown, "Cambridge")
# check the state of selected item
statesCheck(dudFrame.uneditable_domainupdown.listitem[2], "ListItem", invalid_states=["showing", "visible"])
# check other items' states
statesCheck(dudFrame.uneditable_domainupdown.listitem[3], "ListItem", invalid_states=["showing", "visible"])

############################
# press Up/Down on editab_domainupdown
############################
dudFrame.editable_domainupdown.mouseClick()
dudFrame.keyCombo("Up", grabFocus=False)
sleep(config.SHORT_DELAY)
dudFrame.assertText(dudFrame.editable_domainupdown, "Madrid")
# check the state of selected item
statesCheck(dudFrame.editable_domainupdown.listitem[3], "ListItem", add_states=["selected"])
# check other items' states
statesCheck(dudFrame.editable_domainupdown.listitem[0], "ListItem", invalid_states=["showing", "visible"])

# press "Down" on editab_domainupdown
dudFrame.keyCombo("Down", grabFocus=False)
sleep(config.SHORT_DELAY)
dudFrame.assertText(dudFrame.editable_domainupdown, "Provo")
# check the state of selected item
statesCheck(dudFrame.editable_domainupdown.listitem[4], "ListItem", add_states=["selected"])
# check other items' states
statesCheck(dudFrame.editable_domainupdown.listitem[0], "ListItem", invalid_states=["showing", "visible"])

############################
# press Up/Down on uneditab_domainupdown
############################
dudFrame.uneditable_domainupdown.mouseClick()
dudFrame.keyCombo("Up", grabFocus=False)
sleep(config.SHORT_DELAY)
dudFrame.assertText(dudFrame.uneditable_domainupdown, "Beijing")
# check the state of selected item
statesCheck(dudFrame.uneditable_domainupdown.listitem[1], "ListItem", add_states=["selected"])
# check other items' states
statesCheck(dudFrame.uneditable_domainupdown.listitem[0], "ListItem", invalid_states=["showing", "visible"])

# press "Down" on uneditab_domainupdown
dudFrame.keyCombo("Down", grabFocus=False)
sleep(config.SHORT_DELAY)
dudFrame.assertText(dudFrame.uneditable_domainupdown, "Cambridge")
# check the state of selected item
statesCheck(dudFrame.uneditable_domainupdown.listitem[2], "ListItem", add_states=["selected"])
# check other items' states
statesCheck(dudFrame.uneditable_domainupdown.listitem[0], "ListItem", invalid_states=["showing", "visible"])

############################
# check AtkAction of spin button's child - list item
############################
# editable DomainUpDown
dudFrame.editable_domainupdown.mouseClick()
actionsCheck(dudFrame.editable_domainupdown.listitem[5], "ListItem")
# VERIFYME: failed due to bug457172
#dudFrame.assertText(dudFrame.editable_domainupdown, "San Diego")
# check the state of selected item
#statesCheck(dudFrame.uneditable_domainupdown.listitem[5], "ListItem", add_states=["selected"])
# check other items' states
#statesCheck(dudFrame.uneditable_domainupdown.listitem[0], "ListItem", invalid_states=["showing", "visible"])

# uneditable DomainUpDown
dudFrame.uneditable_domainupdown.mouseClick()
actionsCheck(dudFrame.uneditable_domainupdown.listitem[5], "ListItem")
# VERIFYME: failed due to bug457172
#dudFrame.assertText(dudFrame.uneditable_domainupdown, "San Diego")
# check the state of selected item
#statesCheck(dudFrame.uneditable_domainupdown.listitem[5], "ListItem", add_states=["selected"])
# check other items' states
#statesCheck(dudFrame.uneditable_domainupdown.listitem[0], "ListItem", invalid_states=["showing", "visible"])

############################
# end
############################
# close application frame window
dudFrame.quit()

print "INFO:  Log written to: %s" % config.OUTPUT_DIR
