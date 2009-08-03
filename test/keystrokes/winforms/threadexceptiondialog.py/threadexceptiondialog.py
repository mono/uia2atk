#!/usr/bin/python

from macaroon.playback import *
import utils

sequence = MacroSequence()

sequence.append(utils.StartRecordingAction())
sequence.append(WaitForWindowActivate("ThreadExceptionDialog control",None))
sequence.append(utils.AssertPresentationAction(
    "app active",
    ["BRAILLE LINE:  'ThreadExceptionDialog control Frame'",
     "     VISIBLE:  'ThreadExceptionDialog control Frame', cursor=1",
     "BRAILLE LINE:  'Raise an Exception Button'",
     "     VISIBLE:  'Raise an Exception Button', cursor=1",
     "SPEECH OUTPUT: 'ThreadExceptionDialog control frame'",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'Raise an Exception button'"]))

sequence.append(utils.AssertionSummaryAction())

sequence.start()
