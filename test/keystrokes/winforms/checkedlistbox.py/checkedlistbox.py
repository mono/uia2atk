#!/usr/bin/python

from macaroon.playback import *
import utils

sequence = MacroSequence()

sequence.append(utils.StartRecordingAction())
sequence.append(WaitForWindowActivate("CheckedListBox control",None))
sequence.append(utils.AssertPresentationAction(
    "app active",
    ["BRAILLE LINE:  'CheckedListBox control Frame'",
     "     VISIBLE:  'CheckedListBox control Frame', cursor=1",
     "SPEECH OUTPUT: 'CheckedListBox control frame'"]))


sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Down"))
sequence.append(utils.AssertPresentationAction(
    "down to second ~checkbox",
    ["BRAILLE LINE:  '< > 1 CheckBox'",
     "     VISIBLE:  '< > 1 CheckBox', cursor=1",
     "SPEECH OUTPUT: '1 check box not checked'"]))
     
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("space"))
sequence.append(utils.AssertPresentationAction(
    "check 1",
    ["BRAILLE LINE:  '<x> 1 CheckBox'",
     "     VISIBLE:  '<x> 1 CheckBox', cursor=1",
     "SPEECH OUTPUT: 'checked'"]))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Up"))
sequence.append(utils.AssertPresentationAction(
    "up to first checkbox",
    ["BRAILLE LINE:  '< > 0 CheckBox'",
     "     VISIBLE:  '< > 0 CheckBox', cursor=1",
     "SPEECH OUTPUT: '0 check box not checked'"]))

sequence.append(utils.AssertionSummaryAction())

sequence.start()
