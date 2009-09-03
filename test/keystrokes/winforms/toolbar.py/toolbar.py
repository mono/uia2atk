#!/usr/bin/python

from macaroon.playback import *
import utils

sequence = MacroSequence()

sequence.append(utils.StartRecordingAction())
sequence.append(WaitForWindowActivate("ToolBar control",None))
sequence.append(utils.AssertPresentationAction(
    "app active",
    ["BRAILLE LINE:  'ToolBar control Frame'",
     "     VISIBLE:  'ToolBar control Frame', cursor=1",
     "BRAILLE LINE:  ' Combo'",
     "     VISIBLE:  ' Combo', cursor=1",
     "SPEECH OUTPUT: 'ToolBar control frame'",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'combo box'"]))
     
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("KP_4"))
sequence.append(utils.AssertPresentationAction(
    "flat-review move left",
    ["BRAILLE LINE:  'Open Save Print nop separator page: combo box'",
     "     VISIBLE:  'page: combo box', cursor=1",
     "SPEECH OUTPUT: 'page:'"]))
     
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("KP_4"))
sequence.append(utils.AssertPresentationAction(
    "flat-review move left",
    ["BRAILLE LINE:  'Open Save Print nop separator page: combo box'",
     "     VISIBLE:  'Open Save Print nop separator pa', cursor=21",
     "SPEECH OUTPUT: 'separator'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("KP_4"))
sequence.append(utils.AssertPresentationAction(
    "flat-review move left",
    ["BRAILLE LINE:  'Open Save Print nop separator page: combo box'",
     "     VISIBLE:  'Open Save Print nop separator pa', cursor=17",
     "SPEECH OUTPUT: 'nop'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("KP_Divide"))
sequence.append(utils.AssertPresentationAction(
    "do a left mouse click",
    ["BRAILLE LINE:  'ToolBar'",
     "     VISIBLE:  'ToolBar', cursor=1",
     "BRAILLE LINE:  'nop Button'",
     "     VISIBLE:  'nop Button', cursor=1",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'tool bar'",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'nop button'"]))



sequence.append(utils.AssertionSummaryAction())

sequence.start()
