#!/usr/bin/python

from macaroon.playback import *
import utils

sequence = MacroSequence()

sequence.append(utils.StartRecordingAction())
sequence.append(WaitForWindowActivate("Horizontal Splitter",None))
sequence.append(utils.AssertPresentationAction(
    "app active",
    ["BRAILLE LINE:  'Horizontal Splitter Frame'",
     "     VISIBLE:  'Horizontal Splitter Frame', cursor=1",
     "SPEECH OUTPUT: 'Horizontal Splitter frame'"]))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("KP_8"))
sequence.append(utils.AssertPresentationAction(
    "switch to flat-review and read the current line (KP_8)",
    ["BRAILLE LINE:  'label3 on one side against splitter'",
     "     VISIBLE:  'label3 on one side against split', cursor=1",
     "SPEECH OUTPUT: 'label3 on one side against splitter'"]))     
     
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("KP_9"))
sequence.append(utils.AssertPresentationAction(
    "line down to see the next label (KP_9)",
    ["BRAILLE LINE:  'label2 on one side against splitter'",
     "     VISIBLE:  'label2 on one side against split', cursor=1",
     "SPEECH OUTPUT: 'label2 on one side against splitter'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("KP_9"))
sequence.append(utils.AssertPresentationAction(
    "line down to see the next label (KP_9)",
    ["BRAILLE LINE:  'label1 on one side against splitter'",
     "     VISIBLE:  'label1 on one side against split', cursor=1",
     "SPEECH OUTPUT: 'label1 on one side against splitter'"]))     

     
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("KP_9"))
sequence.append(utils.AssertPresentationAction(
    "line down to see the next label (KP_9)",
    ["BRAILLE LINE:  'label0 on one side against splitter'",
     "     VISIBLE:  'label0 on one side against split', cursor=1",
     "SPEECH OUTPUT: 'label0 on one side against splitter'"]))
     


sequence.append(utils.AssertionSummaryAction())

sequence.start()
