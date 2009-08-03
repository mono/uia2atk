#!/usr/bin/python

from macaroon.playback import *
import utils

sequence = MacroSequence()

sequence.append(utils.StartRecordingAction())
sequence.append(WaitForWindowActivate("TrackBar control",None))
sequence.append(utils.AssertPresentationAction(
    "app active",
    ["BRAILLE LINE:  'TrackBar control Frame'",
     "     VISIBLE:  'TrackBar control Frame', cursor=1",
     "BRAILLE LINE:  '1 Slider'",
     "     VISIBLE:  '1 Slider', cursor=1",
     "SPEECH OUTPUT: 'TrackBar control frame'",
     "SPEECH OUTPUT: ''"]))

sequence.append(utils.AssertionSummaryAction())

sequence.start()
