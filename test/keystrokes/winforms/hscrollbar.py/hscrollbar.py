#!/usr/bin/python

from macaroon.playback import *
import utils

sequence = MacroSequence()

sequence.append(utils.StartRecordingAction())
sequence.append(WaitForWindowActivate("HScrollBar control",None))
sequence.append(utils.AssertPresentationAction(
    "hscrollbar frame active",
    ["BRAILLE LINE:  'HScrollBar control Frame'",
     "     VISIBLE:  'HScrollBar control Frame', cursor=1",
      "SPEECH OUTPUT: 'HScrollBar control frame'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("KP_Subtract"))
sequence.append(utils.AssertPresentationAction(
    "switch to flat-review",
    ["BRAILLE LINE:  'Value:'",
     "     VISIBLE:  'Value:', cursor=1",
     "SPEECH OUTPUT: 'Value:'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("KP_9"))
sequence.append(utils.AssertPresentationAction(
    "flat-review line down",
    ["BRAILLE LINE:  'horizontal ScrollBar 0%'",
     "     VISIBLE:  'horizontal ScrollBar 0%', cursor=1",
      "SPEECH OUTPUT: 'horizontal scroll bar 0 percent.'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("KP_Divide"))
sequence.append(utils.AssertPresentationAction(
    "left mouse click",
    ["BRAILLE LINE:  'horizontal ScrollBar 16%'",
     "     VISIBLE:  'horizontal ScrollBar 16%', cursor=1"]))



sequence.append(utils.AssertionSummaryAction())

sequence.start()
