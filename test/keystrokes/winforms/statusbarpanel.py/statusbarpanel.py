#!/usr/bin/python

from macaroon.playback import *
import utils

sequence = MacroSequence()

sequence.append(utils.StartRecordingAction())
sequence.append(WaitForWindowActivate("Help Provider Demonstration",None))
sequence.append(utils.AssertPresentationAction(
    "statusbarpanel  frame active",
    ["BRAILLE LINE:  'StatusBar_StatusBarPanel controls Frame'",
     "     VISIBLE:  'StatusBar_StatusBarPanel control', cursor=1",
     "BRAILLE LINE:  'button1 Button'",
     "     VISIBLE:  'button1 Button', cursor=1",
     "SPEECH OUTPUT: 'StatusBar_StatusBarPanel controls frame'",
     "SPEECH OUTPUT: ''",
    "SPEECH OUTPUT: 'button1 button'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Down"))
sequence.append(utils.AssertPresentationAction(
    "button2",
    ["BRAILLE LINE:  'button2 Button'",
     "     VISIBLE:  'button2 Button', cursor=1",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'button2 button'"]))

sequence.append(utils.AssertionSummaryAction())

sequence.start()
