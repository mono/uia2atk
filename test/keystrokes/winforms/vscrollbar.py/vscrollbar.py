#!/usr/bin/python

from macaroon.playback import *
import utils

sequence = MacroSequence()

sequence.append(utils.StartRecordingAction())
sequence.append(WaitForWindowActivate("VScrollBar control",None))
sequence.append(utils.AssertPresentationAction(
    "vscrollbar frame active",
    ["BRAILLE LINE:  'VScrollBar control Frame'",
     "     VISIBLE:  'VScrollBar control Frame', cursor=1",
      "SPEECH OUTPUT: 'VScrollBar control frame'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("KP_Subtract"))
sequence.append(utils.AssertPresentationAction(
    "switch to flat-review",
    ["BRAILLE LINE:  'Value: vertical ScrollBar 0%'",
     "     VISIBLE:  'Value: vertical ScrollBar 0%', cursor=1",
     "SPEECH OUTPUT: 'Value:'"]))
     
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("KP_6"))
sequence.append(KeyComboAction("KP_Divide"))
sequence.append(utils.AssertPresentationAction(
    "flat-review move right and do a left mouse click",
    ["BRAILLE LINE:  'Value: vertical ScrollBar 0%'",
     "     VISIBLE:  'Value: vertical ScrollBar 0%', cursor=8",
     "BRAILLE LINE:  'Value: 20'",
     "     VISIBLE:  'Value: 20', cursor=1",
     "BRAILLE LINE:  'Value: 20'",
    "     VISIBLE:  'Value: 20', cursor=1",
    "BRAILLE LINE:  'Value: vertical ScrollBar 16%'",
    "     VISIBLE:  'Value: vertical ScrollBar 16%', cursor=8",
    "SPEECH OUTPUT: 'vertical scroll bar 16 percent.'"]))


sequence.append(utils.AssertionSummaryAction())

sequence.start()
