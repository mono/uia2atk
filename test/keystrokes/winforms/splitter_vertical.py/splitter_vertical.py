#!/usr/bin/python

from macaroon.playback import *
import utils

sequence = MacroSequence()

sequence.append(utils.StartRecordingAction())
sequence.append(WaitForWindowActivate("Vertical Splitter",None))
sequence.append(utils.AssertPresentationAction(
    "app active",
    ["BRAILLE LINE:  'Vertical Splitter Frame'",
     "     VISIBLE:  'Vertical Splitter Frame', cursor=1",
     "BRAILLE LINE:  'Right Side Button'",
     "     VISIBLE:  'Right Side Button', cursor=1",
     "SPEECH OUTPUT: 'Vertical Splitter frame'",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'Right Side button'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Tab"))
sequence.append(utils.AssertPresentationAction(
    "Tab to next control",
    ["BRAILLE LINE:  'TreeView Node'",
     "     VISIBLE:  'TreeView Node', cursor=1",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'TreeView Node'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Down"))
sequence.append(utils.AssertPresentationAction(
    "down to next entry",
    ["BRAILLE LINE:  'Another Node'",
     "     VISIBLE:  'Another Node', cursor=1",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'Another Node'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("KP_Enter"))
sequence.append(utils.AssertPresentationAction(
    "get control information  (KP_Enter)",
    ["BRAILLE LINE:  'Another Node'",
     "     VISIBLE:  'Another Node', cursor=1",
     "SPEECH OUTPUT: 'tree table'",
     "SPEECH OUTPUT: 'cell'",
     "SPEECH OUTPUT: 'Another Node'",
     "SPEECH OUTPUT: 'column 1 of 1'",
     "SPEECH OUTPUT: 'row 2 of 2'"]))
     

sequence.append(utils.AssertionSummaryAction())

sequence.start()
