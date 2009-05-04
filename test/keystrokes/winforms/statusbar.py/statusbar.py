#!/usr/bin/python

from macaroon.playback import *
import utils

sequence = MacroSequence()

sequence.append(utils.StartRecordingAction())
sequence.append(WaitForWindowActivate("StatusBar controls",None))
sequence.append(utils.AssertPresentationAction(
    "StatusBar controls frame active",
    ["BRAILLE LINE:  'StatusBar controls Frame'",
     "     VISIBLE:  'StatusBar controls Frame', cursor=1",
     "BRAILLE LINE:  'button1 Button'",
     "     VISIBLE:  'button1 Button', cursor=1",
     "SPEECH OUTPUT: 'StatusBar controls frame'",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'button1 button'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Return"))
sequence.append(KeyComboAction("KP_9"))
sequence.append(utils.AssertPresentationAction(
    "press button and go lndn to see the text",
    ["BRAILLE LINE:  'Changed text 1 times'",
     "     VISIBLE:  'Changed text 1 times', cursor=1",
     "SPEECH OUTPUT: 'Changed text 1 times'"]))


sequence.append(utils.AssertionSummaryAction())

sequence.start()
