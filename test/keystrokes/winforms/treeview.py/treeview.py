#!/usr/bin/python

from macaroon.playback import *
import utils

sequence = MacroSequence()

sequence.append(utils.StartRecordingAction())
sequence.append(WaitForWindowActivate("TreeView control",None))
sequence.append(utils.AssertPresentationAction(
    "app active",
    ["BRAILLE LINE:  'TreeView control Frame'",
     "     VISIBLE:  'TreeView control Frame', cursor=1",
     "BRAILLE LINE:  'Parent 1 collapsed'",
     "     VISIBLE:  'Parent 1 collapsed', cursor=1",
     "SPEECH OUTPUT: 'TreeView control frame'",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'Parent 1 collapsed'"]))

sequence.append(utils.AssertionSummaryAction())

sequence.start()
