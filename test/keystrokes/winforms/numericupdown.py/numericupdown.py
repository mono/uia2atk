#!/usr/bin/python

from macaroon.playback import *
import utils

sequence = MacroSequence()

sequence.append(utils.StartRecordingAction())
sequence.append(WaitForWindowActivate("NumericUpDown Example",None))
sequence.append(utils.AssertPresentationAction(
    "NumericUpDown Example active",
    ["BRAILLE LINE:  'NumericUpDown Example Frame'",
     "     VISIBLE:  'NumericUpDown Example Frame', cursor=1",
     "BRAILLE LINE:  '10'",
     "     VISIBLE:  '10', cursor=1",
     "SPEECH OUTPUT: 'NumericUpDown Example frame'",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: '10 spin button'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Down"))
sequence.append(utils.AssertPresentationAction(
    "decrease the value (down)",
    ["BRAILLE LINE:  '-10'",
     "     VISIBLE:  '-10', cursor=1",
     "BRAILLE LINE:  '-10'",
     "     VISIBLE:  '-10', cursor=1",
     "SPEECH OUTPUT: '-10'",
     "SPEECH OUTPUT: '-10'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Up"))
sequence.append(utils.AssertPresentationAction(
    "increase the value (up)",
    ["BRAILLE LINE:  '10'",
     "     VISIBLE:  '10', cursor=1",
     "BRAILLE LINE:  '10'",
     "     VISIBLE:  '10', cursor=1",
     "SPEECH OUTPUT: '10'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Delete"))
sequence.append(KeyComboAction("Delete"))
sequence.append(KeyComboAction("1"))
sequence.append(KeyComboAction("0"))
sequence.append(KeyComboAction("0"))
sequence.append(KeyComboAction("Return"))
sequence.append(utils.AssertPresentationAction(
    "remove the value and enter 100",
    ["BRAILLE LINE:  '1'",
     "     VISIBLE:  '1', cursor=1",
     "BRAILLE LINE:  '1'",
     "     VISIBLE:  '1', cursor=1",
     "BRAILLE LINE:  ''",
     "     VISIBLE:  '', cursor=1",
     "BRAILLE LINE:  ''",
     "     VISIBLE:  '', cursor=1",
     "BRAILLE LINE:  '1'",
     "     VISIBLE:  '1', cursor=1",
     "BRAILLE LINE:  '1'",
     "     VISIBLE:  '1', cursor=1",
     "BRAILLE LINE:  '10'",
     "     VISIBLE:  '10', cursor=1",
     "BRAILLE LINE:  '10'",
     "     VISIBLE:  '10', cursor=1",
     "BRAILLE LINE:  '100'",
     "     VISIBLE:  '100', cursor=1",
     "BRAILLE LINE:  '100'",
     "     VISIBLE:  '100', cursor=1",
     "SPEECH OUTPUT: '0'",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: '1'",
     "SPEECH OUTPUT: '10'",
     "SPEECH OUTPUT: '100'"]))



sequence.append(utils.AssertionSummaryAction())

sequence.start()
