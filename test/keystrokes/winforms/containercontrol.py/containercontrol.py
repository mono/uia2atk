#!/usr/bin/python

from macaroon.playback import *
import utils

sequence = MacroSequence()

sequence.append(utils.StartRecordingAction())
sequence.append(WaitForWindowActivate("ContainerControl control",None))
sequence.append(utils.AssertPresentationAction(
    "app active",
    ["BRAILLE LINE:  'ContainerControl control Frame'",
     "     VISIBLE:  'ContainerControl control Frame', cursor=1",
     "BRAILLE LINE:  'Panel'",
     "     VISIBLE:  'Panel', cursor=1",
     "SPEECH OUTPUT: 'ContainerControl control frame'",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'panel'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("KP_8"))
sequence.append(utils.AssertPresentationAction(
    "switch to flat-review",
    ["BRAILLE LINE:  'Press Tab, please'",
     "     VISIBLE:  'Press Tab, please', cursor=1",
     "SPEECH OUTPUT: 'Press Tab, please'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Tab"))
sequence.append(utils.AssertPresentationAction(
    "jump to next control",
    ["BRAILLE LINE:  'Panel'",
     "     VISIBLE:  'Panel', cursor=1",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'panel'"]))
    
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("KP_8"))
sequence.append(utils.AssertPresentationAction(
    "switch to flat-review",
    ["BRAILLE LINE:  'I lose focus'",
     "     VISIBLE:  'I lose focus', cursor=1",
     "SPEECH OUTPUT: 'I lose focus'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Tab"))
sequence.append(utils.AssertPresentationAction(
    "jump to next control",
    ["BRAILLE LINE:  'Panel'",
     "     VISIBLE:  'Panel', cursor=1",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'panel'"]))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("KP_8"))
sequence.append(utils.AssertPresentationAction(
    "switch to flat-review",
    ["BRAILLE LINE:  'I got it'",
     "     VISIBLE:  'I got it', cursor=1",
     "SPEECH OUTPUT: 'I got it'"]))
    
sequence.append(utils.AssertionSummaryAction())

sequence.start()
