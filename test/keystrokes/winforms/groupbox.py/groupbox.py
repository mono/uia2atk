#!/usr/bin/python

from macaroon.playback import *
import utils

sequence = MacroSequence()

sequence.append(utils.StartRecordingAction())
sequence.append(WaitForWindowActivate("GroupBox with Button",None))
sequence.append(utils.AssertPresentationAction(
    "focus on Button1",
    ["BRAILLE LINE:  'GroupBox with Button Frame'",
     "     VISIBLE:  'GroupBox with Button Frame', cursor=1",
     "BRAILLE LINE:  'button1 Button'",
     "     VISIBLE:  'button1 Button', cursor=1",
     "SPEECH OUTPUT: 'GroupBox with Button frame'",
     "SPEECH OUTPUT: 'button1 button'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Tab"))
sequence.append(WaitForFocus("button2", acc_role=pyatspi.ROLE_PUSH_BUTTON))
sequence.append(utils.AssertPresentationAction(
    "switch focus to Button2",
    ["BRAILLE LINE:  'button2 Button'",
    "     VISIBLE:  'button2 Button', cursor=1",
    "SPEECH OUTPUT: 'button2 button'"]))


sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("space"))
sequence.append(utils.AssertPresentationAction(
    "trigger button2",
    ["BRAILLE LINE:  'button2 Button'",
    "     VISIBLE:  'button2 Button', cursor=1",
    "SPEECH OUTPUT: 'button2 button'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("KP_7"))
sequence.append(utils.AssertPresentationAction(
    "lineup in flat-review (button2)",
    ["BRAILLE LINE:  '1'",
    "     VISIBLE:  '1', cursor=1",
    "SPEECH OUTPUT: '1'"]))

sequence.append(utils.AssertionSummaryAction())


sequence.start()
