#!/usr/bin/python

from macaroon.playback import *
import utils

sequence = MacroSequence()

sequence.append(utils.StartRecordingAction())
sequence.append(WaitForWindowActivate("OpenFileDialog control",None))
sequence.append(utils.AssertPresentationAction(
    "app active",
    ["BRAILLE LINE:  'OpenFileDialog control Frame'",
     "     VISIBLE:  'OpenFileDialog control Frame', cursor=1",
     "BRAILLE LINE:  'OpenDialog Button'",
     "     VISIBLE:  'OpenDialog Button', cursor=1",
     "SPEECH OUTPUT: 'OpenFileDialog control frame'",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'OpenDialog button'"]))
     
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Return"))
sequence.append(WaitForWindowActivate("Open",None))
sequence.append(utils.AssertPresentationAction(
    "open dialog active",
        ["BRAILLE LINE:  'Open Frame'"]))

sequence.append(utils.AssertionSummaryAction())

sequence.start()
