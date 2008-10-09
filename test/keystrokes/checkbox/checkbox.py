#!/usr/bin/python
"""Test of menu accelerator label output using the gtk-demo UI Manager
demo.
"""
from macaroon.playback import *
import utils

sequence = MacroSequence()

sequence.append(utils.StartRecordingAction())
sequence.append(WaitForWindowActivate("Check Button",None))
sequence.append(utils.StartRecordingAction())
sequence.append(WaitForFocus("check button 1", acc_role=pyatspi.ROLE_CHECK_BOX))
sequence.append(utils.AssertPresentationAction(
    "Check Button",
    [" BRAILLE LINE: 'check button 1'",
    "      VISIBLE: 'check button 1', cursor=1",
    "SPEECH OUTPUT: 'check button 1'"]))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Return"))
sequence.append(KeyComboAction("Return"))
sequence.append(KeyComboAction("Tab"))
sequence.append(WaitForFocus("check button 2", acc_role=pyatspi.ROLE_CHECK_BOX))
sequence.append(utils.AssertPresentationAction(
    "Check Button",
    [" BRAILLE LINE: 'check button 2'",
    "      VISIBLE: 'check button 2', cursor=1",
    "SPEECH OUTPUT: 'check button 2'"]))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Return"))
sequence.append(KeyComboAction("Return"))
sequence.append(KeyComboAction("Tab"))
sequence.append(KeyComboAction("Return"))

sequence.append(PauseAction(1000))
sequence.append(utils.AssertionSummaryAction())

sequence.start()
