#!/usr/bin/python

from macaroon.playback import *
import utils

sequence = MacroSequence()

sequence.append(utils.StartRecordingAction())
sequence.append(WaitForWindowActivate("ToolStripComboBox Control",None))
sequence.append(utils.AssertPresentationAction(
    "ToolStripComboBox control active",
    ["BRAILLE LINE:  'ToolStripComboBox Control Frame'",
     "     VISIBLE:  'ToolStripComboBox Control Frame', cursor=1",
     "SPEECH OUTPUT: 'ToolStripComboBox Control frame'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("KP_9"))
sequence.append(utils.AssertPresentationAction(
    "flatreview lndn (kp_9)",
        ["BRAILLE LINE:  'Please Select one Font Size from the ComboBox'",
         "     VISIBLE:  'Please Select one Font Size from', cursor=1",
         "SPEECH OUTPUT: 'Please Select one Font Size from the ComboBox'"]))
        
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("KP_7"))
sequence.append(KeyComboAction("KP_Divide"))
sequence.append(utils.AssertPresentationAction(
    "flatreview lnup (kp_7) and right mouse click (kp_divide) to activate combobox",
        ["BRAILLE LINE:  '8'",
         "     VISIBLE:  '8', cursor=1",
         "BRAILLE LINE:  '8 Combo'",
         "     VISIBLE:  '8 Combo', cursor=1",
         "BRAILLE LINE:  '8 Combo'",
         "     VISIBLE:  '8 Combo', cursor=1",
         "SPEECH OUTPUT: '8'",
         "SPEECH OUTPUT: ''",
         "SPEECH OUTPUT: '8 combo box'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Down"))
sequence.append(utils.AssertPresentationAction(
    "move down to next combo entry",
        ["BRAILLE LINE:  '10 Combo'",
         "     VISIBLE:  '10 Combo', cursor=1",
         "BRAILLE LINE:  '10 Combo'",
         "     VISIBLE:  '10 Combo', cursor=1",
         "SPEECH OUTPUT: '10'",
         "SPEECH OUTPUT: '10'"]))
        
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("KP_9"))
sequence.append(utils.AssertPresentationAction(
    "flatreview lndn",
        ["BRAILLE LINE:  'The font size is 10'",
         "     VISIBLE:  'The font size is 10', cursor=1",
         "SPEECH OUTPUT: 'The font size is 10'"]))

sequence.append(utils.AssertionSummaryAction())

sequence.start()
