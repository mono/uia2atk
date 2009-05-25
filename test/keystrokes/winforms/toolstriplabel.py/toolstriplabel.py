#!/usr/bin/python

from macaroon.playback import *
import utils

sequence = MacroSequence()

sequence.append(utils.StartRecordingAction())
sequence.append(WaitForWindowActivate("ToolStripLabel Control",None))
sequence.append(utils.AssertPresentationAction(
    "toolstriplabel active",
    ["BRAILLE LINE:  'ToolStripLabel Control Frame'",
     "     VISIBLE:  'ToolStripLabel Control Frame', cursor=1",
     "SPEECH OUTPUT: 'ToolStripLabel Control frame'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("KP_8"))
sequence.append(utils.AssertPresentationAction(
    "(kp_8)",
            ["BRAILLE LINE:  'Accessibility ToolStripLabel with image'",
             "     VISIBLE:  'Accessibility ToolStripLabel wit', cursor=1",
             "SPEECH OUTPUT: 'Accessibility ToolStripLabel with image'"]))

sequence.append(utils.AssertionSummaryAction())

sequence.start()
