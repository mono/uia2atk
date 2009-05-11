#!/usr/bin/python

from macaroon.playback import *
import utils

sequence = MacroSequence()

sequence.append(utils.StartRecordingAction())
sequence.append(WaitForWindowActivate("Form control",None))
sequence.append(utils.AssertPresentationAction(
    "form active - button1 focus",
    ["BRAILLE LINE:  'Form control Frame'",
     "     VISIBLE:  'Form control Frame', cursor=1",
     "BRAILLE LINE:  'button1 Button'",
     "     VISIBLE:  'button1 Button', cursor=1",
     "SPEECH OUTPUT: 'Form control frame'",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'button1 button'"]))
     
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Return"))
sequence.append(utils.AssertPresentationAction(
    "trigger Button1",
    ["BRAILLE LINE:  'Message Form Dialog'",
    "     VISIBLE:  'Message Form Dialog', cursor=1",
    "BRAILLE LINE:  'OK Button'",
    "     VISIBLE:  'OK Button', cursor=1",
    "SPEECH OUTPUT: ''",
    "SPEECH OUTPUT: 'Message Form successful clicked me'",
    "SPEECH OUTPUT: ''",
    "SPEECH OUTPUT: 'OK button'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Return"))
sequence.append(utils.AssertPresentationAction(
    "trigger OK button",
    ["BRAILLE LINE:  'Form control Frame'",
     "     VISIBLE:  'Form control Frame', cursor=1",
     "BRAILLE LINE:  'button1 Button'",
     "     VISIBLE:  'button1 Button', cursor=1",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'Form control frame'",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'button1 button'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Down"))
sequence.append(KeyComboAction("Down"))
sequence.append(utils.AssertPresentationAction(
    "go to button3",
    ["BRAILLE LINE:  'button2 Button'",
     "     VISIBLE:  'button2 Button', cursor=1",
     "BRAILLE LINE:  'button3 Button'",
     "     VISIBLE:  'button3 Button', cursor=1",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'button2 button'",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'button3 button'"]))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Return"))
sequence.append(utils.AssertPresentationAction(
    "trigger button3",
    ["BRAILLE LINE:  'Extra Form Dialog'",
     "     VISIBLE:  'Extra Form Dialog', cursor=1",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'Extra Form'"]))
     
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("<ALT>F4"))
sequence.append(utils.AssertPresentationAction(
    "close dialog",
    ["BRAILLE LINE:  'Form control Frame'",
     "     VISIBLE:  'Form control Frame', cursor=1",
     "BRAILLE LINE:  'button3 Button'",
     "     VISIBLE:  'button3 Button', cursor=1",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'Form control frame'",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'button3 button'"]))
     
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Up"))
sequence.append(KeyComboAction("Return"))
sequence.append(utils.AssertPresentationAction(
    "go to button2 and press enter",
    ["BRAILLE LINE:  'button2 Button'",
     "     VISIBLE:  'button2 Button', cursor=1",
     "BRAILLE LINE:  'Extra Form Frame'",
     "     VISIBLE:  'Extra Form Frame', cursor=1",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'button2 button'",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'Extra Form frame'"]))

sequence.append(utils.AssertionSummaryAction())

sequence.start()
