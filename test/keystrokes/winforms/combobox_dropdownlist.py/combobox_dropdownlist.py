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
     "BRAILLE LINE:  ' Combo'",
     "     VISIBLE:  ' Combo', cursor=1",
     "SPEECH OUTPUT: 'ComboBox control frame'",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'combo box'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Down"))
sequence.append(utils.AssertPresentationAction(
    "down next entry",
    ["BRAILLE LINE:  '0'",
     "     VISIBLE:  '0', cursor=1",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: '0'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("KP_Enter"))
sequence.append(utils.AssertPresentationAction(
    "control info (kp_enter)",
    ["BRAILLE LINE:  '0'"
     "     VISIBLE:  '0', cursor=1",
     "SPEECH OUTPUT: 'combo box'",
     "SPEECH OUTPUT: '0'",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'item 1 of 10'",
     "SPEECH OUTPUT: ''"]))

sequence.append(utils.AssertionSummaryAction())

sequence.start()
