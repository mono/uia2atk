#!/usr/bin/python

from macaroon.playback import *
import utils

sequence = MacroSequence()

sequence.append(utils.StartRecordingAction())
sequence.append(WaitForWindowActivate("Simple MaskedTextBox Example",None))
sequence.append(utils.AssertPresentationAction(
    "app active",
    ["BRAILLE LINE:  'Simple MaskedTextBox Example Frame'",
     "     VISIBLE:  'Simple MaskedTextBox Example Fra', cursor=1",
     "BRAILLE LINE:  '__/__/____'",
     "     VISIBLE:  '__/__/____', cursor=1",
     "SPEECH OUTPUT: 'Simple MaskedTextBox Example frame'",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'text __/__/____'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("1"))
sequence.append(KeyComboAction("2"))
sequence.append(KeyComboAction("1"))
sequence.append(KeyComboAction("2"))
sequence.append(KeyComboAction("2"))
sequence.append(KeyComboAction("0"))
sequence.append(KeyComboAction("0"))
sequence.append(KeyComboAction("9"))
sequence.append(utils.AssertPresentationAction(
    "entered 12/12/2009",
    ["BRAILLE LINE:  '1_/__/____'",
     "     VISIBLE:  '1_/__/____', cursor=1",
     "BRAILLE LINE:  '12/__/____'",
     "     VISIBLE:  '12/__/____', cursor=1",
     "BRAILLE LINE:  '12/1_/____'",
     "     VISIBLE:  '12/1_/____', cursor=5",
     "BRAILLE LINE:  '12/12/____'",
     "     VISIBLE:  '12/12/____', cursor=1",
     "BRAILLE LINE:  '12/12/2___'",
     "     VISIBLE:  '12/12/2___', cursor=8",
     "BRAILLE LINE:  '12/12/20__'",
     "     VISIBLE:  '12/12/20__', cursor=1",
     "BRAILLE LINE:  '12/12/200_'",
     "     VISIBLE:  '12/12/200_', cursor=1",
     "BRAILLE LINE:  '12/12/2009'",
     "     VISIBLE:  '12/12/2009', cursor=1"]))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Tab"))
sequence.append(utils.AssertPresentationAction(
    "jump to maskedtextbox 2",
    ["BRAILLE LINE:  '(86_)-___-____'",
     "     VISIBLE:  '(86_)-___-____', cursor=1",
    "SPEECH OUTPUT: ''",
    "SPEECH OUTPUT: 'text (86_)-___-____'"]))
    
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Tab"))
sequence.append(utils.AssertPresentationAction(
    "jump to maskedtextbox3",
    ["BRAILLE LINE:  '$999,999.00'",
    "     VISIBLE:  '$999,999.00', cursor=1",
    "SPEECH OUTPUT: 'text $999,999.00'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Tab"))
sequence.append(utils.AssertPresentationAction(
    "jump to maskedtextbox4",
    ["BRAILLE LINE:  'LLLLLLL'",
    "     VISIBLE:  'LLLLLLL', Cursor=1",
    "SPEECH OUTPUT: 'text LLLLLLL'"]))


sequence.append(utils.AssertionSummaryAction())


sequence.start()
