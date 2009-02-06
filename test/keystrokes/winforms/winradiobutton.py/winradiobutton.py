#!/usr/bin/python

from macaroon.playback import *
import utils

sequence = MacroSequence()

sequence.append(WaitForWindowActivate("Radio Button",None))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Down"))
sequence.append(WaitForFocus("Banana", acc_role=pyatspi.ROLE_RADIO_BUTTON))
sequence.append(KeyComboAction("space"))
sequence.append(utils.AssertPresentationAction(
    "Apple Radio Button",
    ["BRAILLE LINE:  '&=y Banana RadioButton'",
     "     VISIBLE:  '&=y Banana RadioButton', cursor=1",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'Banana selected radio button'"]))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Down"))
sequence.append(WaitForFocus("Cherry", acc_role=pyatspi.ROLE_RADIO_BUTTON))
sequence.append(KeyComboAction("space"))
sequence.append(utils.AssertPresentationAction(
    "Banana Radio Button",
    ["BRAILLE LINE:  '&=y Cherry RadioButton'",
     "     VISIBLE:  '&=y Cherry RadioButton', cursor=1",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'Cherry selected radio button'"]))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Down"))
sequence.append(WaitForFocus("Apple", acc_role=pyatspi.ROLE_RADIO_BUTTON))
sequence.append(KeyComboAction("space"))
sequence.append(utils.AssertPresentationAction(
    "Cherry Radio Button",
    ["BRAILLE LINE:  '&=y Apple RadioButton'",
    "     VISIBLE:  '&=y Apple RadioButton', cursor=1",
    "SPEECH OUTPUT: ''",
    "SPEECH OUTPUT: 'Apple selected radio button'"]))

sequence.append(utils.AssertionSummaryAction())
sequence.start()
