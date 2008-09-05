#!/usr/bin/python

from macaroon.playback import *
import utils

sequence = MacroSequence()

sequence.append(utils.StartRecordingAction())
sequence.append(WaitForWindowActivate("Choose Wisely",None))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("KP_7"))
sequence.append(utils.AssertPresentationAction(
    "Read label",
    ["BRAILLE LINE:  'You have not yet clicked a button $l'",
     "     VISIBLE:  'You have not yet clicked a butto', cursor=1",
     "SPEECH OUTPUT: 'You have not yet clicked a button'"]))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Tab"))
sequence.append(WaitForFocus("Button 2", acc_role=pyatspi.ROLE_PUSH_BUTTON))
sequence.append(utils.AssertPresentationAction(
    "Focus Button 2",
    ["BRAILLE LINE:  'gtktutorial.py Application Choose Wisely Frame You have not yet clicked a button Filler Button 2 Button'",
    "     VISIBLE:  'Button 2 Button', cursor=1",
    "BRAILLE LINE:  'gtktutorial.py Application Choose Wisely Frame You have not yet clicked a button Filler Button 2 Button'",
    "     VISIBLE:  'Button 2 Button', cursor=1",
    "SPEECH OUTPUT: ''",
    "SPEECH OUTPUT: 'Button 2 button'"]))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Left"))
sequence.append(WaitForFocus("Button 1", acc_role=pyatspi.ROLE_PUSH_BUTTON))
sequence.append(utils.AssertPresentationAction(
    "Focus Button 1",
    ["BRAILLE LINE:  'gtktutorial.py Application Choose Wisely Frame You have not yet clicked a button Filler Button 1 Button'",
    "     VISIBLE:  'Button 1 Button', cursor=1",
    "SPEECH OUTPUT: ''",
    "SPEECH OUTPUT: 'Button 1 button'"]))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("KP_Enter"))
sequence.append(utils.AssertPresentationAction(
    "Button 1 Where Am I?",
   ["BRAILLE LINE:  'gtktutorial.py Application Choose Wisely Frame You have not yet clicked a button Filler Button 1 Button'",
    "     VISIBLE:  'Button 1 Button', cursor=1",
    "SPEECH OUTPUT: 'Button 1'",
    "SPEECH OUTPUT: 'button'",
    "SPEECH OUTPUT: "]))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Return"))
sequence.append(WaitForFocus("Column 0", acc_role=pyatspi.ROLE_TABLE_COLUMN_HEADER))
sequence.append(utils.AssertPresentationAction(
    "Clicked Button 2",
   ["BRAILLE LINE:  'gtktutorial.py Application Sample Tree View Frame'",
    "     VISIBLE:  'Sample Tree View Frame', cursor=1",
    "BRAILLE LINE:  'gtktutorial.py Application Sample Tree View Frame TreeTable Column 0 ColumnHeader'",
    "     VISIBLE:  'Column 0 ColumnHeader', cursor=1",
    "SPEECH OUTPUT: 'Sample Tree View frame'",
    "SPEECH OUTPUT: ''",
    "SPEECH OUTPUT: 'Column 0 column header'"]))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Down"))
sequence.append(WaitForFocus("", acc_role=pyatspi.ROLE_TREE_TABLE))
sequence.append(utils.AssertPresentationAction(
    "Arrow down to parent 0",
   ["BRAILLE LINE:  'gtktutorial.py Application Sample Tree View Frame TreeTable'",
    "     VISIBLE:  'TreeTable', cursor=1",
    "BRAILLE LINE:  'gtktutorial.py Application Sample Tree View Frame TreeTable Column 0 ColumnHeader parent 0 collapsed'",
    "     VISIBLE:  'parent 0 collapsed', cursor=1",
    "SPEECH OUTPUT: ''",
    "SPEECH OUTPUT: 'tree table'",
    "SPEECH OUTPUT: ''",
    "SPEECH OUTPUT: 'Column 0 column header'",
    "SPEECH OUTPUT: 'parent 0 collapsed'"]))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Down"))
sequence.append(utils.AssertPresentationAction(
    "Arrow down to parent 1",
   ["BRAILLE LINE:  'gtktutorial.py Application Sample Tree View Frame TreeTable Column 0 ColumnHeader parent 1 collapsed TREE LEVEL 1'",
    "     VISIBLE:  'parent 1 collapsed TREE LEVEL 1', cursor=1",
    "SPEECH OUTPUT: ''",
    "SPEECH OUTPUT: 'parent 1 collapsed'"]))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("<Shift>Right"))
sequence.append(utils.AssertPresentationAction(
    "Expand parent 1",
   ["BRAILLE LINE:  'gtktutorial.py Application Sample Tree View Frame TreeTable Column 0 ColumnHeader parent 1 expanded TREE LEVEL 1'",
    "     VISIBLE:  'parent 1 expanded TREE LEVEL 1', cursor=1",
    "SPEECH OUTPUT: 'expanded 3 items'"]))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Down"))
sequence.append(utils.AssertPresentationAction(
    "Arrow down to child 0 of parent 1",
   ["BRAILLE LINE:  'gtktutorial.py Application Sample Tree View Frame TreeTable Column 0 ColumnHeader child 0 of parent 1 TREE LEVEL 2'",
    "     VISIBLE:  'child 0 of parent 1 TREE LEVEL 2', cursor=1",
    "SPEECH OUTPUT: ''",
    "SPEECH OUTPUT: 'child 0 of parent 1'",
    "SPEECH OUTPUT: 'tree level 2'"]))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Down"))
sequence.append(utils.AssertPresentationAction(
    "Arrow down to child 1 of parent 1",
   ["BRAILLE LINE:  'gtktutorial.py Application Sample Tree View Frame TreeTable Column 0 ColumnHeader child 1 of parent 1 TREE LEVEL 2'",
    "     VISIBLE:  'child 1 of parent 1 TREE LEVEL 2', cursor=1",
    "SPEECH OUTPUT: ''",
    "SPEECH OUTPUT: 'child 1 of parent 1'"]))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Down"))
sequence.append(utils.AssertPresentationAction(
    "Arrow down to child 2 of parent 1",
   ["BRAILLE LINE:  'gtktutorial.py Application Sample Tree View Frame TreeTable Column 0 ColumnHeader child 2 of parent 1 TREE LEVEL 2'",
    "     VISIBLE:  'child 2 of parent 1 TREE LEVEL 2', cursor=1",
    "SPEECH OUTPUT: ''",
    "SPEECH OUTPUT: 'child 2 of parent 1'"]))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("<Alt>F4"))
sequence.append(utils.AssertPresentationAction(
    "Close the tree view frame",
   ["BRAILLE LINE:  'gtktutorial.py Application Choose Wisely Frame'",
    "     VISIBLE:  'Choose Wisely Frame', cursor=1",
    "BRAILLE LINE:  'gtktutorial.py Application Choose Wisely Frame button one was clicked last Filler Button 1 Button'",
    "     VISIBLE:  'Button 1 Button', cursor=1",
    "SPEECH OUTPUT: 'Choose Wisely frame'",
    "SPEECH OUTPUT: 'button one was clicked last'",
    "SPEECH OUTPUT: 'Button 1 button'"]))
sequence.start()
