#!/usr/bin/python
"""Test of menu accelerator label output using the gtk-demo UI Manager
demo.
"""
from macaroon.playback import *
import utils

sequence = MacroSequence()

sequence.append(WaitForWindowActivate("Check Button",None))
sequence.append(utils.StartRecordingAction())
sequence.append(WaitForFocus("check button 1", acc_role=pyatspi.ROLE_CHECK_BOX))
sequence.append(KeyComboAction("space"))
sequence.append(KeyComboAction("space"))
sequence.append(KeyComboAction("Tab"))
sequence.append(PauseAction(20000))
sequence.append(WaitForFocus("check button 2", acc_role=pyatspi.ROLE_CHECK_BOX))
sequence.append(KeyComboAction("space"))
sequence.append(KeyComboAction("space"))
sequence.append(KeyComboAction("Tab"))
sequence.append(utils.AssertPresentationAction(
    "Check Button",
    ["BRAILLE LINE:  'ipy Application Check Button Frame <x> check button 1 CheckBox'",
    "     VISIBLE:  '<x> check button 1 CheckBox', cursor=1",
    "BRAILLE LINE:  'ipy Application Check Button Frame < > check button 1 CheckBox'",
    "     VISIBLE:  '< > check button 1 CheckBox', cursor=1",
    "BRAILLE LINE:  'ipy Application Check Button Frame < > check button 2 CheckBox'",
    "     VISIBLE:  '< > check button 2 CheckBox', cursor=1",
    "BRAILLE LINE:  'ipy Application Check Button Frame <x> check button 2 CheckBox'",
    "     VISIBLE:  '<x> check button 2 CheckBox', cursor=1",
    "BRAILLE LINE:  'ipy Application Check Button Frame < > check button 2 CheckBox'",
    "     VISIBLE:  '< > check button 2 CheckBox', cursor=1",
    "BRAILLE LINE:  'ipy Application Check Button Frame Quit Button'",
    "     VISIBLE:  'Quit Button', cursor=1",
    "SPEECH OUTPUT: 'checked'",
    "SPEECH OUTPUT: 'not checked'",
    "SPEECH OUTPUT: ''",
    "SPEECH OUTPUT: 'check button 2 check box not checked'",
    "SPEECH OUTPUT: 'checked'",
    "SPEECH OUTPUT: 'not checked'",
    "SPEECH OUTPUT: ''",
    "SPEECH OUTPUT: 'Quit button'"]))
sequence.append(KeyComboAction("Return"))

sequence.append(utils.AssertionSummaryAction())

sequence.start()
