#!/usr/bin/python

from macaroon.playback import *
import utils

sequence = MacroSequence()

sequence.append(utils.StartRecordingAction())
sequence.append(WaitForWindowActivate("FontDialog control",None))
sequence.append(utils.AssertPresentationAction(
    "app active",
    ["BRAILLE LINE:  'FontDialog control Frame'",
     "     VISIBLE:  'FontDialog control Frame', cursor=1",
     "BRAILLE LINE:  'Click me Button'",
     "     VISIBLE:  'Click me Button', cursor=1",
     "SPEECH OUTPUT: 'FontDialog control frame'",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'Click me button'"]))

sequence.append(utils.AssertionSummaryAction())

sequence.start()
