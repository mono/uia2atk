#!/usr/bin/python

from macaroon.playback import *
import utils

sequence = MacroSequence()

sequence.append(utils.StartRecordingAction())
sequence.append(WaitForWindowActivate("SplitContainer control",None))
sequence.append(utils.AssertPresentationAction(
    "app active",
    ["BRAILLE LINE:  'SplitContainer control Frame'",
     "     VISIBLE:  'SplitContainer control Frame', cursor=1",
     "SPEECH OUTPUT: 'SplitContainer control frame'"]))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("KP_8"))
sequence.append(utils.AssertPresentationAction(
    "switch to flat-review and read the current line (KP_8)",
    ["BRAILLE LINE:  'label1 in splitcontainer.panel1'",
     "     VISIBLE:  'label1 in splitcontainer.panel1', cursor=1",
     "SPEECH OUTPUT: 'label1 in splitcontainer.panel1'"]))     
     
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("KP_9"))
sequence.append(utils.AssertPresentationAction(
    "line down to see the next panel (KP_9)",
    ["BRAILLE LINE:  'label2 in splitcontainer.panel2'",
     "     VISIBLE:  'label2 in splitcontainer.panel2', cursor=1",
     "SPEECH OUTPUT: 'label2 in splitcontainer.panel2'"]))
     


sequence.append(utils.AssertionSummaryAction())

sequence.start()
