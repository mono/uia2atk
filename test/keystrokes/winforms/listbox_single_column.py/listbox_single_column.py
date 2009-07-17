#!/usr/bin/python

from macaroon.playback import *
import utils

sequence = MacroSequence()

sequence.append(utils.StartRecordingAction())
sequence.append(WaitForWindowActivate("ListBox control",None))
#sequence.append(WaitForFocus("0", acc_role=pyatspi.ROLE_TABLE_CELL))
sequence.append(utils.AssertPresentationAction(
    "list focus",
    ["BRAILLE LINE:  'ListBox control Frame'",
     "     VISIBLE:  'ListBox control Frame', cursor=1",
      "BRAILLE LINE:  '0'",
      "     VISIBLE:  '0', cursor=1",
      "SPEECH OUTPUT: 'ListBox control frame'",
      "SPEECH OUTPUT: ''",
      "SPEECH OUTPUT: '0'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("End"))
#sequence.append(WaitForFocus("19", acc_role=pyatspi.ROLE_TABLE_CELL))
sequence.append(utils.AssertPresentationAction(
    "jump to last entry",
    ["BRAILLE LINE:  '19'",
     "     VISIBLE:  '19', cursor=1",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: '19'"]))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Home"))
#sequence.append(WaitForFocus("0", acc_role=pyatspi.ROLE_TABLE_CELL))
sequence.append(utils.AssertPresentationAction(
    "jump back to first entry",
    ["BRAILLE LINE:  '0'",
     "     VISIBLE:  '0', cursor=1",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: '0'"]))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("KP_7"))
sequence.append(utils.AssertPresentationAction(
    "lineup in flat-review kp_7",
    ["BRAILLE LINE:  'You select '",
     "     VISIBLE:  'You select ', cursor=1",
     "SPEECH OUTPUT: 'You select '",]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("5"))
#sequence.append(WaitForFocus("5", acc_role=pyatspi.ROLE_TABLE_CELL))
sequence.append(utils.AssertPresentationAction(
    "jump to item 5",
    ["BRAILLE LINE:  '5'",
     "     VISIBLE:  '5', cursor=1",
     "BRAILLE LINE:  '5'",
     "     VISIBLE:  '5', cursor=1",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: '5'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("KP_Enter"))
sequence.append(utils.AssertPresentationAction(
    "ask: which item and how many",
    ["BRAILLE LINE:  '5'",
     "     VISIBLE:  '5', cursor=1",
     "SPEECH OUTPUT: '5'",
     "SPEECH OUTPUT: 'item 6 of 20'"]))



sequence.append(utils.AssertionSummaryAction())

sequence.start()
