#!/usr/bin/python

from macaroon.playback import *
import utils

sequence = MacroSequence()

#sequence.append(PauseAction(5000))
sequence.append(WaitForWindowActivate("Buttons",None))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("space"))
#sequence.append(WaitForWindowActivate("Message Dialog",None))
sequence.append(utils.AssertPresentationAction(
    "Message Dialog",
    ["BRAILLE LINE:  'ipy Application Message Dialog Frame OK Button'",
    "     VISIBLE:  'OK Button', cursor=1",
    "SPEECH OUTPUT: ''",
    "SPEECH OUTPUT: 'OK button'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("<ALT>F4"))
sequence.append(utils.AssertPresentationAction(
    "focus back to Button 1",
    ["BRAILLE LINE:  'ipy Application Buttons Frame Button 1 Button'",
    "     VISIBLE:  'Button 1 Button', cursor=1",
    "SPEECH OUTPUT: 'Button 1 button'"]))

#sequence.append(WaitForWindowActivate("Buttons",None))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Tab"))
sequence.append(utils.AssertPresentationAction(
    "switch focus to Button 2",
    ["BRAILLE LINE:  'ipy Application Buttons Frame Button 2 Button'",
    "     VISIBLE:  'Button 2 Button', cursor=1",
    "SPEECH OUTPUT: 'Button 2 button'"]))

#sequence.append(WaitForFocus("Button 2", acc_role=pyatspi.ROLE_PUSH_BUTTON))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("space"))
sequence.append(utils.AssertPresentationAction(
    "Message Dialog",
    ["BRAILLE LINE:  'ipy Application Message Dialog Frame OK Button'",
    "     VISIBLE:  'OK Button', cursor=1",
    "SPEECH OUTPUT: ''",
    "SPEECH OUTPUT: 'OK button'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("<ALT>F4"))
#sequence.append(WaitForWindowActivate("Buttons",None))
sequence.append(utils.AssertPresentationAction(
    "focus back to Button 2",
    ["BRAILLE LINE:  'ipy Application Buttons Frame Button 2 Button'",
    "     VISIBLE:  'Button 2 Button', cursor=1",
    "SPEECH OUTPUT: 'Button 2 button'"]))

#sequence.append(WaitForFocus("openSUSE", acc_role=pyatspi.ROLE_PUSH_BUTTON))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Tab"))
sequence.append(utils.AssertPresentationAction(
    "switch focus to openSUSE Button",
    ["BRAILLE LINE:  'ipy Application Buttons Frame openSUSE Button'",
    "     VISIBLE:  'openSUSE Button', cursor=1",
    "SPEECH OUTPUT: 'openSUSE button'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("space"))
sequence.append(utils.AssertPresentationAction(
    "Message Dialog",
    ["BRAILLE LINE:  'ipy Application Message Dialog Frame OK Button'",
    "     VISIBLE:  'OK Button', cursor=1",
    "SPEECH OUTPUT: ''",
    "SPEECH OUTPUT: 'OK button'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("<ALT>F4"))
sequence.append(utils.AssertPresentationAction(
    "focus back to openSUSE Button",
    ["BRAILLE LINE:  'ipy Application Buttons Frame openSUSE Button'",
    "     VISIBLE:  'openSUSE Button', cursor=1",
    "SPEECH OUTPUT: 'openSUSE button'"]))

sequence.append(utils.AssertionSummaryAction())
sequence.start()
