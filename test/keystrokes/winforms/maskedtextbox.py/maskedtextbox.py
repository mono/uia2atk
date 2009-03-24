#!/usr/bin/python

from macaroon.playback import *
import utils

sequence = MacroSequence()

sequence.append(WaitForWindowActivate("Simple MaskedTextBox Example",None))
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
    ["BRAILLE LINE:  '  /  /'",
    "     VISIBLE:  '  /  /', cursor=2",
     "BRAILLE LINE:  '  /  /'",
     "     VISIBLE:  '  /  /', cursor=1",
     "BRAILLE LINE:  '1_/__/____'",
     "     VISIBLE:  '1_/__/____', cursor=1",
     "BRAILLE LINE:  '1_/__/____'",
     "     VISIBLE:  '1_/__/____', cursor=1",
     "BRAILLE LINE:  '1_/__/____'",
     "     VISIBLE:  '1_/__/____', cursor=2",
     "BRAILLE LINE:  '1_/__/____'",
     "     VISIBLE:  '1_/__/____', cursor=4",
     "BRAILLE LINE:  '1_/__/____'",
     "     VISIBLE:  '1_/__/____', cursor=1",
     "BRAILLE LINE:  '12/__/____'",
     "     VISIBLE:  '12/__/____', cursor=1",
     "BRAILLE LINE:  '12/__/____'",
     "     VISIBLE:  '12/__/____', cursor=1",
     "BRAILLE LINE:  '12/__/____'",
     "     VISIBLE:  '12/__/____', cursor=4",
     "BRAILLE LINE:  '12/__/____'",
     "     VISIBLE:  '12/__/____', cursor=5",
     "BRAILLE LINE:  '12/__/____'",
     "     VISIBLE:  '12/__/____', cursor=1",
     "BRAILLE LINE:  '12/1_/____'",
     "     VISIBLE:  '12/1_/____', cursor=1",
     "BRAILLE LINE:  '12/1_/____'",
     "     VISIBLE:  '12/1_/____', cursor=1",
     "BRAILLE LINE:  '12/1_/____'",
     "     VISIBLE:  '12/1_/____', cursor=5",
     "BRAILLE LINE:  '12/1_/____'",
     "     VISIBLE:  '12/1_/____', cursor=7",
     "BRAILLE LINE:  '12/1_/____'",
     "     VISIBLE:  '12/1_/____', cursor=1",
     "BRAILLE LINE:  '12/12/____'",
     "     VISIBLE:  '12/12/____', cursor=1",
     "BRAILLE LINE:  '12/12/____'",
     "     VISIBLE:  '12/12/____', cursor=1",
     "BRAILLE LINE:  '12/12/____'",
     "     VISIBLE:  '12/12/____', cursor=7",
     "BRAILLE LINE:  '12/12/____'",
     "     VISIBLE:  '12/12/____', cursor=8",
     "BRAILLE LINE:  '12/12/____'",
     "     VISIBLE:  '12/12/____', cursor=1",
     "BRAILLE LINE:  '12/12/2___'",
     "     VISIBLE:  '12/12/2___', cursor=1",
     "BRAILLE LINE:  '12/12/2___'",
     "     VISIBLE:  '12/12/2___', cursor=1",
     "BRAILLE LINE:  '12/12/2___'",
     "     VISIBLE:  '12/12/2___', cursor=8",
     "BRAILLE LINE:  '12/12/2___'",
     "     VISIBLE:  '12/12/2___', cursor=9",
     "BRAILLE LINE:  '12/12/2___'",
     "     VISIBLE:  '12/12/2___', cursor=1",
     "BRAILLE LINE:  '12/12/20__'",
     "     VISIBLE:  '12/12/20__', cursor=1",
     "BRAILLE LINE:  '12/12/20__'",
     "     VISIBLE:  '12/12/20__', cursor=1",
     "BRAILLE LINE:  '12/12/20__'",
     "     VISIBLE:  '12/12/20__', cursor=9",
     "BRAILLE LINE:  '12/12/20__'",
     "     VISIBLE:  '12/12/20__', cursor=10",
     "BRAILLE LINE:  '12/12/20__'",
     "     VISIBLE:  '12/12/20__', cursor=1",
     "BRAILLE LINE:  '12/12/200_'",
     "     VISIBLE:  '12/12/200_', cursor=1",
     "BRAILLE LINE:  '12/12/200_'",
     "     VISIBLE:  '12/12/200_', cursor=1",
     "BRAILLE LINE:  '12/12/200_'",
     "     VISIBLE:  '12/12/200_', cursor=10",
     "BRAILLE LINE:  '12/12/200_'",
     "     VISIBLE:  '12/12/200_', cursor=11",
     "BRAILLE LINE:  '12/12/200_'",
     "     VISIBLE:  '12/12/200_', cursor=1",
     "BRAILLE LINE:  '12/12/2009'",
     "     VISIBLE:  '12/12/2009', cursor=1",
     "BRAILLE LINE:  '12/12/2009'",
     "     VISIBLE:  '12/12/2009', cursor=1",
     "BRAILLE LINE:  '12/12/2009'",
     "     VISIBLE:  '12/12/2009', cursor=11"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Tab"))
sequence.append(utils.AssertPresentationAction(
    "jump to maskedtextbox 2",
    ["BRAILLE LINE:  '(860)-000-0000",
    "     VISIBLE:  '(860)-000-0000', cursor=1",
    "SPEECH OUTPUT: ''",
    "SPEECH OUTPUT: 'text (860)-000-0000'"]))
    
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
