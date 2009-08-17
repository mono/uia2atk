#!/usr/bin/python

from macaroon.playback import *
import utils

sequence = MacroSequence()

sequence.append(utils.StartRecordingAction())
sequence.append(WaitForWindowActivate("PrintPreviewControl control",None))
sequence.append(utils.AssertPresentationAction(
    "button focus",
    ["BRAILLE LINE:  'PrintPreviewControl control Frame'",
     "     VISIBLE:  'PrintPreviewControl control Fram', cursor=1",
     "BRAILLE LINE:  'Button Button'",
     "     VISIBLE:  'Button Button', cursor=1",
     "SPEECH OUTPUT: 'PrintPreviewControl control frame'",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'Button button'"]))

sequence.append(utils.AssertionSummaryAction())

sequence.start()
