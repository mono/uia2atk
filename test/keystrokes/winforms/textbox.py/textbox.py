#!/usr/bin/python

from macaroon.playback import *
import utils

sequence = MacroSequence()

sequence.append(utils.StartRecordingAction())
sequence.append(WaitForWindowActivate("TextBox Control",None))
sequence.append(utils.AssertPresentationAction(
    "app active",
    ["BRAILLE LINE:  'TextBox Control Frame'",
     "     VISIBLE:  'TextBox Control Frame', cursor=1",
     "BRAILLE LINE:  ''",
     "     VISIBLE:  '', cursor=1",
     "SPEECH OUTPUT: 'TextBox Control frame'",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'text '"]))

sequence.append(utils.AssertionSummaryAction())

sequence.start()
