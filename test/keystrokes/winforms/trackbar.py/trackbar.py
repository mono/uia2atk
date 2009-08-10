#!/usr/bin/python

from macaroon.playback import *
import utils

sequence = MacroSequence()

sequence.append(utils.StartRecordingAction())
sequence.append(WaitForWindowActivate("TrackBar control",None))
sequence.append(utils.AssertPresentationAction(
    "app active",
    ["BRAILLE LINE:  'TrackBar control Frame'",
     "     VISIBLE:  'TrackBar control Frame', cursor=1",
     "BRAILLE LINE:  '1 Slider'",
     "     VISIBLE:  '1 Slider', cursor=1",
     "SPEECH OUTPUT: 'TrackBar control frame'",
     "SPEECH OUTPUT: ''"]))
     
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("KP_Enter"))
sequence.append(utils.AssertPresentationAction(
    "control info",
    ["BRAILLE LINE:  '1 Slider'",
     "     VISIBLE:  '1 Slider', cursor=1",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'slider'",
     "SPEECH OUTPUT: '1.0'",
     "SPEECH OUTPUT: '1 percent'",
     "SPEECH OUTPUT: ''"]))
    
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("KP_7"))
sequence.append(utils.AssertPresentationAction(
    "flat-review lnup",
    ["BRAILLE LINE:  'The value of TrackBar(Vertical) is: 1'",
     "     VISIBLE:  'The value of TrackBar(Vertical) ', cursor=1",
     
     "SPEECH OUTPUT: 'The value of TrackBar(Vertical) is: 1'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("KP_7"))
sequence.append(utils.AssertPresentationAction(
    "flat-review lnup",
    ["BRAILLE LINE:  '1'",
     "     VISIBLE:  '1', cursor=1",
     "SPEECH OUTPUT: '1'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("KP_7"))
sequence.append(utils.AssertPresentationAction(
    "flat-review lnup",
    ["BRAILLE LINE:  'The value of TrackBar(Horizontal) is: 1'",
     "     VISIBLE:  'The value of TrackBar(Horizontal', cursor=1",
     "SPEECH OUTPUT: 'The value of TrackBar(Horizontal) is: 1'"]))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("KP_Subtract"))
sequence.append(KeyComboAction("Up"))
sequence.append(utils.AssertPresentationAction(
    "leave flat-review - increase focused slider",
    ["BRAILLE LINE:  '1 Slider'",
     "     VISIBLE:  '1 Slider', cursor=1",
     "BRAILLE LINE:  '2 Slider'",
     "     VISIBLE:  '2 Slider', cursor=1",
     "SPEECH OUTPUT: '2'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Tab"))
sequence.append(utils.AssertPresentationAction(
    "jump to next slider",
    ["BRAILLE LINE:  '1 Slider'",
     "     VISIBLE:  '1 Slider', cursor=1",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: '1 slider 1'"]))
    
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Right"))
sequence.append(utils.AssertPresentationAction(
    "increase focused slider",
    ["BRAILLE LINE:  '2 Slider'",
     "     VISIBLE:  '2 Slider', cursor=1",
     "SPEECH OUTPUT: '2'"]))
    
    
  

sequence.append(utils.AssertionSummaryAction())

sequence.start()
