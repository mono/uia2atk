#!/usr/bin/python

from macaroon.playback import *
import utils

sequence = MacroSequence()


sequence.append(utils.StartRecordingAction())
sequence.append(WaitForWindowActivate("Panel control",None))
sequence.append(WaitForFocus("Bananas", acc_role=pyatspi.ROLE_CHECK_BOX))
sequence.append(utils.AssertPresentationAction(
    "focus on Bananas checkbox",
    ["BRAILLE LINE:  'Panel control Frame'",
     "     VISIBLE:  'Panel control Frame', cursor=1",
     "BRAILLE LINE:  '< > Bananas CheckBox'",
     "     VISIBLE:  '< > Bananas CheckBox', cursor=1",
     "SPEECH OUTPUT: 'Panel control frame'",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'Bananas check box not checked'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("space"))
sequence.append(utils.AssertPresentationAction(
    "check Bananas checkbox",
    ["BRAILLE LINE:  '<x> Bananas CheckBox'",
     "     VISIBLE:  '<x> Bananas CheckBox', cursor=1",
     "SPEECH OUTPUT: 'checked'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Tab"))
sequence.append(WaitForFocus("Chicken", acc_role=pyatspi.ROLE_CHECK_BOX))
sequence.append(utils.AssertPresentationAction(
    "focus on Chicken checkbox",
    ["BRAILLE LINE:  '< > Chicken CheckBox'",
     "     VISIBLE:  '< > Chicken CheckBox', cursor=1",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'Chicken check box not checked'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("space"))
sequence.append(utils.AssertPresentationAction(
    "check Chicken checkbox",
    ["BRAILLE LINE:  '<x> Chicken CheckBox'",
     "     VISIBLE:  '<x> Chicken CheckBox', cursor=1",
     "SPEECH OUTPUT: 'checked'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Tab"))
sequence.append(WaitForFocus("Stuffed Peppers", acc_role=pyatspi.ROLE_CHECK_BOX))
sequence.append(utils.AssertPresentationAction(
    "focus on Stuffed Peppers checkbox",
    ["BRAILLE LINE:  '<x> Stuffed Peppers CheckBox'",
     "     VISIBLE:  '<x> Stuffed Peppers CheckBox', cursor=1",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'Stuffed Peppers check box checked'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("space"))
sequence.append(utils.AssertPresentationAction(
    "un-check Stuffed Peppers checkbox",
    ["BRAILLE LINE:  '< > Stuffed Peppers CheckBox'",
     "     VISIBLE:  '< > Stuffed Peppers CheckBox', cursor=1",
     "SPEECH OUTPUT: 'not checked'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Tab"))
sequence.append(WaitForFocus("Male", acc_role=pyatspi.ROLE_RADIO_BUTTON))
sequence.append(utils.AssertPresentationAction(
    "focus on Male radiobutton",
    ["BRAILLE LINE:  '&=y Male RadioButton'",
     "     VISIBLE:  '&=y Male RadioButton', cursor=1",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'Male selected radio button'"]))
     
    
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Down"))
sequence.append(utils.AssertPresentationAction(
    "focus on Female radiobutton",
    ["BRAILLE LINE:  '&=y Female RadioButton'",
     "     VISIBLE:  '&=y Female RadioButton', cursor=1",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'Female selected radio button'"]))
    
sequence.append(utils.AssertionSummaryAction())

sequence.start()
