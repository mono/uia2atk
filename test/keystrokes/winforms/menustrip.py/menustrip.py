#!/usr/bin/python

from macaroon.playback import *
import utils

sequence = MacroSequence()

sequence.append(utils.StartRecordingAction())
sequence.append(WaitForWindowActivate("MenuStrip Control",None))
sequence.append(utils.AssertPresentationAction(
    "MenuStrip Control active",
    ["BRAILLE LINE:  'MenuStrip Control Frame'",
     "     VISIBLE:  'MenuStrip Control Frame', cursor=1",
     "SPEECH OUTPUT: 'MenuStrip Control frame'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("KP_Subtract"))
sequence.append(utils.AssertPresentationAction(
    "switch to flatreview to see the menubar",
    ["BRAILLE LINE:  'File Edit'",
     "     VISIBLE:  'File Edit', cursor=1",
     "SPEECH OUTPUT: 'File'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("<ALT>f"))
sequence.append(utils.AssertPresentationAction(
    "open file menu <alt+f>",
    ["BRAILLE LINE:  'File Menu'",
     "     VISIBLE:  'File Menu', cursor=1",
     "BRAILLE LINE:  'File Menu'",
     "     VISIBLE:  'File Menu', cursor=1",
     "BRAILLE LINE:  'New Menu'",
     "     VISIBLE:  'New Menu', cursor=1",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'File menu'",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'New menu'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Down"))
sequence.append(utils.AssertPresentationAction(
    "next item in file menu (down)",
    ["BRAILLE LINE:  'Open'",
     "     VISIBLE:  'Open', cursor=1",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'Open'"]))
     
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Up"))
sequence.append(utils.AssertPresentationAction(
    "prev item in file menu (up)",
    ["BRAILLE LINE:  'New Menu'",
     "     VISIBLE:  'New Menu', cursor=1",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'New menu'"]))
     
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Right"))
sequence.append(utils.AssertPresentationAction(
    "move to menu of item New (Right)",
    ["BRAILLE LINE:  'Document'",
     "     VISIBLE:  'Document', cursor=1",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'Document'"]))
     
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Right"))
sequence.append(utils.AssertPresentationAction(
    "move to edit menu  (Right)",
    ["BRAILLE LINE:  'Edit Menu'",
     "     VISIBLE:  'Edit Menu', cursor=1",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'Edit menu'"]))
     
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("KP_Enter"))
sequence.append(utils.AssertPresentationAction(
    "get information (how many items and which item) ) (KP_Enter)",
    ["BRAILLE LINE:  'Edit Menu'",
     "     VISIBLE:  'Edit Menu', cursor=1",
     "SPEECH OUTPUT: 'menu bar'",
     "SPEECH OUTPUT: 'Edit'",
     "SPEECH OUTPUT: 'menu'",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'item 2 of 2'",
     "SPEECH OUTPUT: ''"]))



sequence.append(utils.AssertionSummaryAction())

sequence.start()
