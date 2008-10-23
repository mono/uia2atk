#!/usr/bin/python

from macaroon.playback import *
import utils

sequence = MacroSequence()

sequence.append(PauseAction(5000))
sequence.append(WaitForWindowActivate("Radio Button",None))
sequence.append(utils.StartRecordingAction())
sequence.append(WaitForFocus("Apple", acc_role=pyatspi.ROLE_RADIO_BUTTON))
sequence.append(KeyComboAction("Down"))
sequence.append(WaitForFocus("Banana", acc_role=pyatspi.ROLE_RADIO_BUTTON))
sequence.append(KeyComboAction("Down"))
sequence.append(WaitForFocus("Cherry", acc_role=pyatspi.ROLE_RADIO_BUTTON))
sequence.append(KeyComboAction("Down"))
sequence.append(utils.AssertPresentationAction(
    "Radio Button",
    ["BRAILLE LINE:  'gtkradiobutton.py Application Radio Button Frame & y Banana RadioButton'",
    "     VISIBLE:  '& y Banana RadioButton', cursor=1",
   "BRAILLE LINE:  'gtkradiobutton.py Application Radio Button Frame & y Cherry RadioButton'",
    "     VISIBLE:  '& y Cherry RadioButton', cursor=1",
    "BRAILLE LINE:  'gtkradiobutton.py Application Radio Button Frame & y Apple RadioButton'",
    "     VISIBLE:  '& y Apple RadioButton', cursor=1",
    "SPEECH OUTPUT: ''",
    "SPEECH OUTPUT: 'Banana not selected radio button'",
    "SPEECH OUTPUT: ''",
    "SPEECH OUTPUT: 'Cherry not selected radio button'",
    "SPEECH OUTPUT: ''",
    "SPEECH OUTPUT: 'Apple not selected radio button'"]))

sequence.append(utils.AssertionSummaryAction())
sequence.start()
