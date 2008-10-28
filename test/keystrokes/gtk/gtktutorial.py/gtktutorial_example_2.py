#!/usr/bin/python

from macaroon.playback import *
import utils

sequence = MacroSequence()

sequence.append(WaitForWindowActivate("Choose Wisely",None))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Tab"))
sequence.append(WaitForFocus("Button 2", acc_role=pyatspi.ROLE_PUSH_BUTTON))
sequence.append(utils.AssertPresentationAction(
    "Focus Button 2",
   ["BRAILLE LINE:  'gtktutorial.py Application Choose Wisely Frame You have not yet clicked a button Filler Button 2 Button'",
    "     VISIBLE:  'Button 2 Button', cursor=1",
    "SPEECH OUTPUT: ''",
    "SPEECH OUTPUT: 'Button 2 button'"]))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("KP_Enter"))
sequence.append(utils.AssertPresentationAction(
    "Button 2 Where Am I?",
   ["BRAILLE LINE:  'gtktutorial.py Application Choose Wisely Frame You have not yet clicked a button Filler Button 2 Button'",
    "     VISIBLE:  'Button 2 Button', cursor=1",
    "SPEECH OUTPUT: 'Button 2'",
    "SPEECH OUTPUT: 'button'",
    "SPEECH OUTPUT: "]))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Return"))
sequence.append(utils.AssertPresentationAction(
    "Clicked Button 2",
   ["BRAILLE LINE:  'gtktutorial.py Application Sample Check Button Frame'",
    "     VISIBLE:  'Sample Check Button Frame', cursor=1",
    "BRAILLE LINE:  'gtktutorial.py Application Sample Check Button Frame < > check button 1 CheckBox'",
    "     VISIBLE:  '< > check button 1 CheckBox', cursor=1",
    "SPEECH OUTPUT: 'Sample Check Button frame'",
    "SPEECH OUTPUT: ''",
    "SPEECH OUTPUT: 'check button 1 check box not checked'"]))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("space"))
sequence.append(utils.AssertPresentationAction(
    "Checked check button 1",
   ["BRAILLE LINE:  'gtktutorial.py Application Sample Check Button Frame <x> check button 1 CheckBox'",
    "     VISIBLE:  '<x> check button 1 CheckBox', cursor=1",
    "SPEECH OUTPUT: 'checked'"]))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("space"))
sequence.append(utils.AssertPresentationAction(
    "Unchecked check button 1",
   ["BRAILLE LINE:  'gtktutorial.py Application Sample Check Button Frame < > check button 1 CheckBox'",
    "     VISIBLE:  '< > check button 1 CheckBox', cursor=1",
    "SPEECH OUTPUT: 'not checked'"]))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Down"))
sequence.append(utils.AssertPresentationAction(
    "Move down to check button 2",
   ["BRAILLE LINE:  'gtktutorial.py Application Sample Check Button Frame < > check button 2 CheckBox'",
    "     VISIBLE:  '< > check button 2 CheckBox', cursor=1",
    "SPEECH OUTPUT: ''",
    "SPEECH OUTPUT: 'check button 2 check box not checked'"]))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("space"))
sequence.append(utils.AssertPresentationAction(
    "Checked check button 2",
   ["BRAILLE LINE:  'gtktutorial.py Application Sample Check Button Frame <x> check button 2 CheckBox'",
    "     VISIBLE:  '<x> check button 2 CheckBox', cursor=1",
    "SPEECH OUTPUT: 'checked'"]))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("space"))
sequence.append(utils.AssertPresentationAction(
    "Unchecked check button 2",
   ["BRAILLE LINE:  'gtktutorial.py Application Sample Check Button Frame < > check button 2 CheckBox'",
    "     VISIBLE:  '< > check button 2 CheckBox', cursor=1",
    "SPEECH OUTPUT: 'not checked'"]))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Tab"))
sequence.append(WaitForFocus("Close", acc_role=pyatspi.ROLE_PUSH_BUTTON))
sequence.append(utils.AssertPresentationAction(
    "Focus Close Button",
   ["BRAILLE LINE:  'gtktutorial.py Application Sample Check Button Frame Close Button'",
"     VISIBLE:  'Close Button', cursor=1",
"SPEECH OUTPUT: ''",
"SPEECH OUTPUT: 'Close button'"]))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Return"))
sequence.append(utils.AssertPresentationAction(
    "Close Sample Check Button Window",
   ["BRAILLE LINE:  'gtktutorial.py Application Choose Wisely Frame'",
    "     VISIBLE:  'Choose Wisely Frame', cursor=1",
    "BRAILLE LINE:  'gtktutorial.py Application Choose Wisely Frame button two was clicked last Filler Button 2 Button'",
    "     VISIBLE:  'Button 2 Button', cursor=1",
    "SPEECH OUTPUT: 'Choose Wisely frame'",
    "SPEECH OUTPUT: 'button two was clicked last'",
    "SPEECH OUTPUT: 'Button 2 button'"]))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("KP_7"))
sequence.append(utils.AssertPresentationAction(
    "Read label",
   ["BRAILLE LINE:  'button two was clicked last $l'",
    "     VISIBLE:  'button two was clicked last $l', cursor=1",
    "SPEECH OUTPUT: 'button two was clicked last'"]))
sequence.start()
