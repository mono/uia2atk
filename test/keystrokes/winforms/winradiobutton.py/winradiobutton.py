#!/usr/bin/python

from macaroon.playback import *
import utils

sequence = MacroSequence()

sequence.append(WaitForWindowActivate("Radio Button",None))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Down"))
sequence.append(utils.AssertPresentationAction(
    "Apple Radio Button",
    ["BRAILLE LINE:  'ipy Application Radio Button Frame & y Banana RadioButton'",
    "     VISIBLE:  '& y Banana RadioButton', cursor=1",
    "SPEECH OUTPUT: ''",
    "SPEECH OUTPUT: 'Banana not selected radio button'"]))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Down"))
sequence.append(utils.AssertPresentationAction(
    "Banana Radio Button",
    ["BRAILLE LINE:  'ipy Application Radio Button Frame & y Cherry RadioButton'",
    "     VISIBLE:  '& y Cherry RadioButton', cursor=1",
    "SPEECH OUTPUT: ''",
    "SPEECH OUTPUT: 'Cherry not selected radio button'"]))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Down"))
sequence.append(utils.AssertPresentationAction(
    "Cherry Radio Button",
    ["BRAILLE LINE:  'ipy Application Radio Button Frame & y Apple RadioButton'",
    "     VISIBLE:  '& y Apple RadioButton', cursor=1",
    "SPEECH OUTPUT: ''",
    "SPEECH OUTPUT: 'Apple not selected radio button'"]))

sequence.append(utils.AssertionSummaryAction())
sequence.start()
