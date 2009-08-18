#!/usr/bin/python

from macaroon.playback import *
import utils

sequence = MacroSequence()

sequence.append(utils.StartRecordingAction())
sequence.append(WaitForWindowActivate("DateTimePicker control",None))
sequence.append(utils.AssertPresentationAction(
    "app active",
    ["BRAILLE LINE:  'DateTimePicker control Frame'",
     "     VISIBLE:  'DateTimePicker control Frame', cursor=1",
     "BRAILLE LINE:  'The date you select is: Panel'",
     "     VISIBLE:  'The date you select is: Panel', cursor=1",
     "SPEECH OUTPUT: 'DateTimePicker control frame'",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'The date you select is: panel'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("KP_Subtract"))
sequence.append(utils.AssertPresentationAction(
    "switch to flat-review (kp_subtract)",
    ["BRAILLE LINE:  '<x> Tuesday ,   18   August   2009 check box Drop Down'",
     "     VISIBLE:  '<x> Tuesday ,   18   August   20', cursor=1",
     "SPEECH OUTPUT: 'checked'"]))


sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Right"))
sequence.append(utils.AssertPresentationAction(
    "move right to select a week-day",
    ["BRAILLE LINE:  'Tuesday'",
     "     VISIBLE:  'Tuesday', cursor=1",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'Tuesday'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Right"))
sequence.append(utils.AssertPresentationAction(
    "move right to select a day",
    ["BRAILLE LINE:  '18'",
     "     VISIBLE:  '18', cursor=1",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: '18 spin button'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Right"))
sequence.append(utils.AssertPresentationAction(
    "move right to select a month",
    ["BRAILLE LINE:  'August'",
     "     VISIBLE:  'August', cursor=1",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'August'"]))


sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Right"))
sequence.append(utils.AssertPresentationAction(
    "move right to select the year",
    ["BRAILLE LINE:  '2009'",
     "     VISIBLE:  '2009', cursor=1",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: '2009 spin button'"]))


sequence.append(utils.AssertionSummaryAction())

sequence.start()
