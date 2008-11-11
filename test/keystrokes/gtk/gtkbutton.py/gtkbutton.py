#!/usr/bin/python

from macaroon.playback import *
import utils

sequence = MacroSequence()

sequence.append(utils.StartRecordingAction())
sequence.append(PauseAction(1000))
sequence.append(WaitForWindowActivate("Buttons",None))
sequence.append(PauseAction(1000))
sequence.append(WaitForFocus("Button 1", acc_role=pyatspi.ROLE_PUSH_BUTTON))
sequence.append(KeyComboAction("space"))
sequence.append(PauseAction(1000))
sequence.append(WaitForWindowActivate("Message Dialog",None))
sequence.append(utils.AssertPresentationAction(
    "Button 1",
    ["BRAILLE LINE:  'gtkbutton.py Application Message Dialog Dialog'",
     "     VISIBLE:  'Message Dialog Dialog', cursor=1",
     "BRAILLE LINE:  'gtkbutton.py Application Message Dialog Dialog OK Button'",
     "     VISIBLE:  'OK Button', cursor=1",
     "SPEECH OUTPUT: 'Message Dialog'",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'OK button'"]))
sequence.append(KeyComboAction("space"))
sequence.append(PauseAction(1000))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Tab"))
sequence.append(WaitForFocus("Button 2", acc_role=pyatspi.ROLE_PUSH_BUTTON))
sequence.append(utils.AssertPresentationAction(
    "Button 2",
    ["BRAILLE LINE:  'gtkbutton.py Application Buttons Frame Button 2 Button'",
     "     VISIBLE:  'Button 2 Button', cursor=1",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'Button 2 button'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Tab"))
sequence.append(WaitForFocus("openSUSE", acc_role=pyatspi.ROLE_PUSH_BUTTON))
sequence.append(KeyComboAction("space"))
sequence.append(KeyComboAction("<ALT>F4"))
sequence.append(utils.AssertPresentationAction(
    "openSUSE Button",
    ["BRAILLE LINE:  'gtkbutton.py Application Message Dialog Dialog'",
    "     VISIBLE:  'Message Dialog Dialog', cursor=1",
    "BRAILLE LINE:  'gtkbutton.py Application Buttons Frame'",
    "     VISIBLE:  'Buttons Frame', cursor=1",
    "BRAILLE LINE:  'gtkbutton.py Application Buttons Frame openSUSE Button'",
    "     VISIBLE:  'openSUSE Button', cursor=1",
    "SPEECH OUTPUT: 'Message Dialog'",
    "SPEECH OUTPUT: 'Buttons frame'",
    "SPEECH OUTPUT: ''",
    "SPEECH OUTPUT: 'openSUSE button'"]))

sequence.append(utils.AssertionSummaryAction())

sequence.start()
