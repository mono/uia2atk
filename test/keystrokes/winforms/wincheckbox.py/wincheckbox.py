#!/usr/bin/python

from macaroon.playback import *
import utils

sequence = MacroSequence()

sequence.append(WaitForWindowActivate("Check Button",None))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("space"))
sequence.append(utils.AssertPresentationAction(
    "Check Button 1 check",
    ["BRAILLE LINE:  '<x> check button 1 CheckBox'",
    "     VISIBLE:  '<x> check button 1 CheckBox', cursor=1",
    "SPEECH OUTPUT: 'checked'"]))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("space"))
sequence.append(utils.AssertPresentationAction(
    "Check Button 1 not checked",
    ["BRAILLE LINE:  '< > check button 1 CheckBox'",
    "     VISIBLE:  '< > check button 1 CheckBox', cursor=1",
    "SPEECH OUTPUT: 'not checked'"]))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Tab"))
sequence.append(WaitForFocus("check button 2", acc_role=pyatspi.ROLE_CHECK_BOX))
sequence.append(utils.AssertPresentationAction(
    "Switch focus to Check Button 2",
    ["BRAILLE LINE:  '< > check button 2 CheckBox'",
    "     VISIBLE:  '< > check button 2 CheckBox', cursor=1",
    "SPEECH OUTPUT: ''",
    "SPEECH OUTPUT: 'check button 2 check box not checked'"]))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("space"))
sequence.append(utils.AssertPresentationAction(
    "Check Button 2 checked",
    ["BRAILLE LINE:  '<x> check button 2 CheckBox'",
    "     VISIBLE:  '<x> check button 2 CheckBox', cursor=1",
    "SPEECH OUTPUT: 'checked'"]))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("space"))
sequence.append(utils.AssertPresentationAction(
    "Check Button 2 not checked",
    ["BRAILLE LINE:  '< > check button 2 CheckBox'",
    "     VISIBLE:  '< > check button 2 CheckBox', cursor=1",
    "SPEECH OUTPUT: 'not checked'"]))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Tab"))
sequence.append(WaitForFocus("Quit", acc_role=pyatspi.ROLE_PUSH_BUTTON))
sequence.append(utils.AssertPresentationAction(
    "Quit Button",
    ["BRAILLE LINE:  'Quit Button'",
    "     VISIBLE:  'Quit Button', cursor=1",
    "SPEECH OUTPUT: ''",
    "SPEECH OUTPUT: 'Quit button'"]))
sequence.append(KeyComboAction("Return"))

sequence.append(utils.AssertionSummaryAction())

sequence.start()
