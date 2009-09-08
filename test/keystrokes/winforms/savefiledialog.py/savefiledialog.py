#!/usr/bin/python

from macaroon.playback import *
import utils

sequence = MacroSequence()

sequence.append(utils.StartRecordingAction())
sequence.append(WaitForWindowActivate("SaveFileDialog control",None))
sequence.append(utils.AssertPresentationAction(
    "app active",
    ["BRAILLE LINE:  'SaveFileDialog control Frame'",
     "     VISIBLE:  'SaveFileDialog control Frame', cursor=1",
     "BRAILLE LINE:  'Click me Button'",
     "     VISIBLE:  'Click me Button', cursor=1",
     "SPEECH OUTPUT: 'SaveFileDialog control frame'",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'Click me button'"]))
     
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Return"))
sequence.append(WaitForWindowActivate("Save",None))
sequence.append(utils.AssertPresentationAction(
    "open the dialog",
        ["BRAILLE LINE:  'Save As Dialog'",
         "     VISIBLE:  'Save As Dialog', cursor=1",
         "SPEECH OUTPUT: ''",
         "SPEECH OUTPUT: 'Save As Save in: File name: Save as type:'"]))

sequence.append(utils.AssertionSummaryAction())

sequence.start()
