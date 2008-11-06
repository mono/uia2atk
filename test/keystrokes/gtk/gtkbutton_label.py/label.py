#!/usr/bin/python

from macaroon.playback import *
import utils

sequence = MacroSequence()

sequence.append(PauseAction(5000))

sequence.append(utils.StartRecordingAction())
sequence.append(WaitForWindowActivate("Choose Wisely",None))
sequence.append(KeyComboAction("space"))
sequence.append(utils.AssertPresentationAction(
    "The Label when clicking 'Button 1'",
    ["BRAILLE LINE:  'gtkradiobutton.py Application Radio Button Frame & y Banana RadioButton'",
    "     VISIBLE:  '& y Banana RadioButton', cursor=1",
    "SPEECH OUTPUT: ''",
    "SPEECH OUTPUT: 'Banana not selected radio button'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Tab"))
sequence.append(WaitForFocus("Button 2", acc_role=pyatspi.ROLE_PUSH_BUTTON))
sequence.append(KeyComboAction("space"))
sequence.append(utils.AssertPresentationAction(
    "The Label when clicking 'Button 2'",
    ["BRAILLE LINE:  'gtkradiobutton.py Application Radio Button Frame & y Banana RadioButton'",
    "     VISIBLE:  '& y Banana RadioButton', cursor=1",
    "SPEECH OUTPUT: ''",
    "SPEECH OUTPUT: 'Banana not selected radio button'"]))
sequence.append(utils.AssertionSummaryAction())

sequence.start()
