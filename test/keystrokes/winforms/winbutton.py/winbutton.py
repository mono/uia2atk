#!/usr/bin/python

from macaroon.playback import *
import utils

sequence = MacroSequence()

sequence.append(utils.StartRecordingAction())
sequence.append(WaitForWindowActivate("Buttons",None))
sequence.append(KeyComboAction("space"))
sequence.append(WaitForWindowActivate("Message Dialog",None))
sequence.append(utils.AssertPresentationAction(
    "Message Dialog",
    ["BRAILLE LINE:  'Buttons Frame'",
     "     VISIBLE:  'Buttons Frame', cursor=1",
     "BRAILLE LINE:  'Button 1 Button'",
     "     VISIBLE:  'Button 1 Button', cursor=1",
    "BRAILLE LINE:  'Message Dialog Dialog'",
     "     VISIBLE:  'Message Dialog Dialog', cursor=1",
     "BRAILLE LINE:  'OK Button'",
     "     VISIBLE:  'OK Button', cursor=1",
     "SPEECH OUTPUT: 'Buttons frame'",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'Button 1 button'",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'Message Dialog'",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'OK button'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("<ALT>F4"))
sequence.append(WaitForFocus("Button 1", acc_role=pyatspi.ROLE_PUSH_BUTTON))
sequence.append(utils.AssertPresentationAction(
    "focus back to Button 1",
    ["BRAILLE LINE:  'Button 1 Button'",
     "     VISIBLE:  'Button 1 Button', cursor=1",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'Button 1 button'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Tab"))
sequence.append(WaitForFocus("Button 2", acc_role=pyatspi.ROLE_PUSH_BUTTON))
sequence.append(utils.AssertPresentationAction(
    "switch focus to Button 2",
    ["BRAILLE LINE:  'Button 2 Button'",
     "     VISIBLE:  'Button 2 Button', cursor=1",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'Button 2 button'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("space"))
sequence.append(WaitForWindowActivate("Message Dialog",None))
sequence.append(utils.AssertPresentationAction(
    "Message Dialog",
    ["BRAILLE LINE:  'Message Dialog Dialog'",
     "     VISIBLE:  'Message Dialog Dialog', cursor=1",
     "BRAILLE LINE:  'OK Button'",
     "     VISIBLE:  'OK Button', cursor=1",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'Message Dialog'",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'OK button'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("<ALT>F4"))
sequence.append(WaitForFocus("Button 2", acc_role=pyatspi.ROLE_PUSH_BUTTON))
sequence.append(utils.AssertPresentationAction(
    "focus back to Button 2",
    ["BRAILLE LINE:  'Button 2 Button'",
     "     VISIBLE:  'Button 2 Button', cursor=1",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'Button 2 button'"]))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Tab"))
sequence.append(WaitForFocus("openSUSE", acc_role=pyatspi.ROLE_PUSH_BUTTON))
sequence.append(utils.AssertPresentationAction(
    "switch focus to openSUSE button",
    ["BRAILLE LINE:  'openSUSE Button'",
     "     VISIBLE:  'openSUSE Button', cursor=1",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'openSUSE button'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("space"))
sequence.append(WaitForWindowActivate("Message Dialog",None))
sequence.append(utils.AssertPresentationAction(
    "Message Dialog",
    ["BRAILLE LINE:  'Message Dialog Dialog'",
     "     VISIBLE:  'Message Dialog Dialog', cursor=1",
     "BRAILLE LINE:  'OK Button'",
     "     VISIBLE:  'OK Button', cursor=1",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'Message Dialog'",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'OK button'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("<ALT>F4"))
sequence.append(WaitForFocus("openSUSE", acc_role=pyatspi.ROLE_PUSH_BUTTON))
sequence.append(utils.AssertPresentationAction(
    "focus back to openSUSE Button",
    ["BRAILLE LINE:  'openSUSE Button'",
     "     VISIBLE:  'openSUSE Button', cursor=1",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'openSUSE button'"]))

sequence.append(utils.AssertionSummaryAction())

sequence.start()
