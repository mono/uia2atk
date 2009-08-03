#!/usr/bin/python

from macaroon.playback import *
import utils

sequence = MacroSequence()

sequence.append(utils.StartRecordingAction())
sequence.append(WaitForWindowActivate("ContainerControl control",None))
sequence.append(utils.AssertPresentationAction(
    "app active",
    ["BRAILLE LINE:  'ContainerControl control Frame'",
     "     VISIBLE:  'ContainerControl control Frame', cursor=1",
     "BRAILLE LINE:  'Panel'",
     "     VISIBLE:  'Panel', cursor=1",
     "SPEECH OUTPUT: 'ContainerControl control frame'",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'panel'"]))

sequence.append(utils.AssertionSummaryAction())

sequence.start()
