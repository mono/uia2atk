#!/usr/bin/python

from macaroon.playback import *
import utils

sequence = MacroSequence()

sequence.append(WaitForWindowActivate("CheckBox control",None))
sequence.append(utils.StartRecordingAction())
sequence.append(WaitForFocus("Bananas", acc_role=pyatspi.ROLE_CHECK_BOX))
sequence.append(utils.AssertPresentationAction(
    "focus on Bananas",
    ["BRAILLE LINE:  'CheckBox control Frame'",
     "     VISIBLE:  'CheckBox control Frame', cursor=1",
     "BRAILLE LINE:  '< > Bananas CheckBox'",
     "     VISIBLE:  '< > Bananas CheckBox', cursor=1",
     "SPEECH OUTPUT: 'CheckBox control frame'",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'Bananas check box not checked'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("space"))
sequence.append(utils.AssertPresentationAction(
    "Bananas checked",
    ["BRAILLE LINE:  '<x> Bananas CheckBox'",
    "     VISIBLE:  '<x> Bananas CheckBox', cursor=1",
    "SPEECH OUTPUT: 'checked'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Tab"))
sequence.append(WaitForFocus("Chicken", acc_role=pyatspi.ROLE_CHECK_BOX))
sequence.append(utils.AssertPresentationAction(
    "switch focus to Chicken",
    ["BRAILLE LINE:  '< > Chicken CheckBox'",
    "     VISIBLE:  '< > Chicken CheckBox', cursor=1",
    "SPEECH OUTPUT: ''",
    "SPEECH OUTPUT: 'Chicken check box not checked'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("space"))
sequence.append(utils.AssertPresentationAction(
    "Chicken checked",
    ["BRAILLE LINE:  '<x> Chicken CheckBox'",
    "     VISIBLE:  '<x> Chicken CheckBox', cursor=1",
    "SPEECH OUTPUT: 'checked'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Down"))
sequence.append(WaitForFocus("Stuffed Peppers", acc_role=pyatspi.ROLE_CHECK_BOX))
sequence.append(utils.AssertPresentationAction(
    "switch focus to Stuffed Peppers",
    ["BRAILLE LINE:  '<x> Stuffed Peppers CheckBox'",
    "     VISIBLE:  '<x> Stuffed Peppers CheckBox', cursor=1",
    "SPEECH OUTPUT: ''",
    "SPEECH OUTPUT: 'Stuffed Peppers check box checked'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Tab"))
sequence.append(WaitForFocus("Fried Lizard", acc_role=pyatspi.ROLE_CHECK_BOX))
sequence.append(utils.AssertPresentationAction(
    "switch focus to Fried Lizard",
    ["BRAILLE LINE:  '< > Fried Lizard CheckBox'",
    "     VISIBLE:  '< > Fried Lizard CheckBox', cursor=1",
    "SPEECH OUTPUT: ''",
    "SPEECH OUTPUT: 'Fried Lizard check box not checked'"]))


sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("KP_Subtract"))
sequence.append(utils.AssertPresentationAction(
    "switch to flat-review",
    ["BRAILLE LINE:  '< > Fried Lizard < > Soylent Green'",
    "     VISIBLE:  '< > Fried Lizard < > Soylent Green', cursor=1",
    "SPEECH OUTPUT: 'not checked'"]))




sequence.append(utils.AssertionSummaryAction())

sequence.start()
