#!/usr/bin/python

from macaroon.playback import *
import utils

sequence = MacroSequence()

sequence.append(utils.StartRecordingAction())
sequence.append(WaitForWindowActivate("TreeView control",None))
sequence.append(utils.AssertPresentationAction(
    "app active",
    ["BRAILLE LINE:  'TreeView control Frame'",
     "     VISIBLE:  'TreeView control Frame', cursor=1",
     "BRAILLE LINE:  'Parent 1 collapsed'",
     "     VISIBLE:  'Parent 1 collapsed', cursor=1",
     "SPEECH OUTPUT: 'TreeView control frame'",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'Parent 1 collapsed'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Down"))
sequence.append(utils.AssertPresentationAction(
    "down to parent2",
    ["BRAILLE LINE:  'Parent 2 collapsed'",
     "     VISIBLE:  'Parent 2 collapsed', cursor=1",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'Parent 2 collapsed'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("KP_Enter"))
sequence.append(utils.AssertPresentationAction(
    "control information",
    ["BRAILLE LINE:  'Parent 2 collapsed'",
     "     VISIBLE:  'Parent 2 collapsed', cursor=1",
     "SPEECH OUTPUT: 'tree table'",
     "SPEECH OUTPUT: 'cell'",
     "SPEECH OUTPUT: 'Parent 2'",
     "SPEECH OUTPUT: 'column 1 of 1'",
     "SPEECH OUTPUT: 'row 2 of 2'",
     "SPEECH OUTPUT: 'collapsed'"]))


sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Right"))
sequence.append(utils.AssertPresentationAction(
    "open parent 2",
    ["BRAILLE LINE:  'Parent 2 expanded'",
     "     VISIBLE:  'Parent 2 expanded', cursor=1",
     "SPEECH OUTPUT: 'expanded 0 items'"]))    
    
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Down"))
sequence.append(utils.AssertPresentationAction(
    "down to child3",
    ["BRAILLE LINE:  'Child 3'",
     "     VISIBLE:  'Child 3', cursor=1",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'Child 3'"]))



sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Left"))
sequence.append(utils.AssertPresentationAction(
    "jump to parent 2",
    ["BRAILLE LINE:  'Parent 2 expanded'",
     "     VISIBLE:  'Parent 2 expanded', cursor=1",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'Parent 2 expanded 0 items'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Left"))
sequence.append(utils.AssertPresentationAction(
    "close parent 2",
    ["BRAILLE LINE:  'Parent 2 collapsed'",
     "     VISIBLE:  'Parent 2 collapsed', cursor=1",
     "SPEECH OUTPUT: 'collapsed'"]))
     
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Up"))
sequence.append(utils.AssertPresentationAction(
    "up  parent 1",
    ["BRAILLE LINE:  'Parent 1 collapsed'",
     "     VISIBLE:  'Parent 1 collapsed', cursor=1",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'Parent 1 collapsed'"]))
    
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Right"))
sequence.append(utils.AssertPresentationAction(
    "open  parent 1",
    ["BRAILLE LINE:  'Parent 1 expanded'",
     "     VISIBLE:  'Parent 1 expanded', cursor=1",
     "SPEECH OUTPUT: 'expanded 0 items'"]))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Down"))
sequence.append(utils.AssertPresentationAction(
    "down child 1",
    ["BRAILLE LINE:  'Child 1'",
     "     VISIBLE:  'Child 1', cursor=1",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'Child 1'"]))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Down"))
sequence.append(utils.AssertPresentationAction(
    "down child 2",
    ["BRAILLE LINE:  'Child 2 collapsed'",
     "     VISIBLE:  'Child 2 collapsed', cursor=1",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'Child 2 collapsed'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Right"))
sequence.append(utils.AssertPresentationAction(
    "open child 2",
    ["BRAILLE LINE:  'Child 2 expanded'",
     "     VISIBLE:  'Child 2 expanded', cursor=1",
     "SPEECH OUTPUT: 'expanded 0 items'"]))
     
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Down"))
sequence.append(utils.AssertPresentationAction(
    "grand child 2",
    ["BRAILLE LINE:  'Grandchild collapsed'",
     "     VISIBLE:  'Grandchild collapsed', cursor=1",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'Grandchild collapsed'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Right"))
sequence.append(utils.AssertPresentationAction(
    "open grand child 2",
    ["BRAILLE LINE:  'Grandchild expanded'",
     "     VISIBLE:  'Grandchild expanded', cursor=1",
     "SPEECH OUTPUT: 'expanded 0 items'"]))
    



    


sequence.append(utils.AssertionSummaryAction())

sequence.start()
