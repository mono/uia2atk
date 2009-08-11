#!/usr/bin/python

from macaroon.playback import *
import utils

sequence = MacroSequence()

sequence.append(utils.StartRecordingAction())
sequence.append(WaitForWindowActivate("ComboBox control",None))
sequence.append(utils.AssertPresentationAction(
    "app active",
    ["BRAILLE LINE:  'ComboBox control Frame'",
     "     VISIBLE:  'ComboBox control Frame', cursor=1",
     "BRAILLE LINE:  '1'",
     "     VISIBLE:  '1', cursor=1",
     "SPEECH OUTPUT: 'ComboBox control frame'",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: '1'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Down"))
sequence.append(utils.AssertPresentationAction(
    "down next entry",
    ["BRAILLE LINE:  '2'",
     "     VISIBLE:  '2', cursor=1",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: '2'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("KP_Enter"))
sequence.append(utils.AssertPresentationAction(
    "control info (kp_enter)",
    ["BRAILLE LINE:  '2'",
     "     VISIBLE:  '2', cursor=1",
     "SPEECH OUTPUT: 'combo box'",
     "SPEECH OUTPUT: '2'",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'item 3 of 10'",
     "SPEECH OUTPUT: ''"]))

sequence.append(utils.AssertionSummaryAction())

sequence.start()
