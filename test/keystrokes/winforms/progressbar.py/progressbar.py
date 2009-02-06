#!/usr/bin/python

from macaroon.playback import *
import utils

sequence = MacroSequence()

sequence.append(utils.StartRecordingAction())
sequence.append(WaitForWindowActivate("ProgressBar control",None))
sequence.append(WaitForFocus("Click", acc_role=pyatspi.ROLE_PUSH_BUTTON))
sequence.append(utils.AssertPresentationAction(
    "click button focus",
    ["BRAILLE LINE:  'ProgressBar control Frame'",
     "     VISIBLE:  'ProgressBar control Frame', cursor=1",
     "BRAILLE LINE:  'Click Button'",
     "     VISIBLE:  'Click Button', cursor=1",
     "SPEECH OUTPUT: 'ProgressBar control frame'",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'Click button'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("space"))
sequence.append(KeyComboAction("KP_Subtract"))
sequence.append(utils.AssertPresentationAction(
    "push button and swich to flat-review (20 percent)",
    ["BRAILLE LINE:  'It is 20% percent of 100% Click'",
     "     VISIBLE:  'It is 20% percent of 100% Click', cursor=27",
     "SPEECH OUTPUT: 'Click'"]))
     
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("space"))
sequence.append(KeyComboAction("KP_5"))
sequence.append(utils.AssertPresentationAction(
    "push button again already in flat-review (40 percent)",
    ["BRAILLE LINE:  'It is 40% percent of 100% Click'",
     "     VISIBLE:  'It is 40% percent of 100% Click', cursor=27",
     "SPEECH OUTPUT: 'Click'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("KP_9"))
sequence.append(utils.AssertPresentationAction(
    "line down in flat-review (40 percent)",
    ["BRAILLE LINE:  'Progress 40%'",
     "     VISIBLE:  'Progress 40%', cursor=1",
     "SPEECH OUTPUT: 'progress bar 40 percent.'"]))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("space"))
sequence.append(KeyComboAction("KP_5"))
sequence.append(utils.AssertPresentationAction(
    "push button again and refresh the line in flat-review (60 percent)",
    ["BRAILLE LINE:  'Progress 60%'",
     "     VISIBLE:  'Progress 60%', cursor=1",
     "SPEECH OUTPUT: 'progress bar 60 percent.'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("KP_7"))
sequence.append(utils.AssertPresentationAction(
    "line up in flat-review (60 percent)",
    ["BRAILLE LINE:  'It is 60% percent of 100% Click'",
     "     VISIBLE:  'It is 60% percent of 100% Click', cursor=27",
     "SPEECH OUTPUT: 'Click'"]))
     

sequence.append(utils.AssertionSummaryAction())

sequence.start()
