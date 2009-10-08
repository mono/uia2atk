#!/usr/bin/python

from macaroon.playback import *
import utils

sequence = MacroSequence()

sequence.append(utils.StartRecordingAction())
sequence.append(WaitForWindowActivate("ScrollBar control",None))
sequence.append(utils.AssertPresentationAction(
    "app active",
    ["BRAILLE LINE:  'ScrollBar control Frame'",
     "     VISIBLE:  'ScrollBar control Frame', cursor=1",
     "SPEECH OUTPUT: 'ScrollBar control frame'"]))


sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("KP_8"))
sequence.append(utils.AssertPresentationAction(
    "read line in flat-review KP_8",
    ["BRAILLE LINE:  'listbox with vertical scrollbar'",
     "     VISIBLE:  'listbox with vertical scrollbar', cursor=1",
     "SPEECH OUTPUT: 'listbox with vertical scrollbar'"]))
     
sequence.append(utils.AssertionSummaryAction())

sequence.start()
