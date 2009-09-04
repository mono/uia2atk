#!/usr/bin/python

from macaroon.playback import *
import utils

sequence = MacroSequence()

sequence.append(utils.StartRecordingAction())
sequence.append(WaitForWindowActivate("ToolStripProgressBar control",None))
sequence.append(utils.AssertPresentationAction(
    "button focus",
    ["BRAILLE LINE:  'ToolStripProgressBar Sample Frame'",
     "     VISIBLE:  'ToolStripProgressBar Sample Fram', cursor=1",
     "BRAILLE LINE:  'button1 Button'",
     "     VISIBLE:  'button1 Button', cursor=1",
     "SPEECH OUTPUT: 'ToolStripProgressBar Sample frame'",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'button1 button'"]))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Return"))
sequence.append(utils.AssertPresentationAction(
    "press enter to increase the value",
            [""]))
            
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("KP_9"))
sequence.append(utils.AssertPresentationAction(
    "flatreview lndn to see the value",
            ["BRAILLE LINE:  'It is 20% of 100%'",
             "     VISIBLE:  'It is 20% of 100%', cursor=1",
             "SPEECH OUTPUT: 'It is 20% of 100%'"]))


sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("KP_9"))
sequence.append(utils.AssertPresentationAction(
    "flatreview lndn to see the next line",
            ["BRAILLE LINE:  'Progress 20%'",
             "     VISIBLE:  'Progress 20%', cursor=1",
             "SPEECH OUTPUT: 'progress bar 20 percent.'"]))

sequence.append(utils.AssertionSummaryAction())

sequence.start()
