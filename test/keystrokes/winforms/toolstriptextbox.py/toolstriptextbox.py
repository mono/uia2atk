#!/usr/bin/python

from macaroon.playback import *
import utils

sequence = MacroSequence()

sequence.append(utils.StartRecordingAction())
sequence.append(WaitForWindowActivate("ToolStripTextBox control",None))
sequence.append(utils.AssertPresentationAction(
    "ToolStripTextBox control active",
    ["BRAILLE LINE:  'ToolStripTextBox control Frame'",
     "     VISIBLE:  'ToolStripTextBox control Frame', cursor=1",
     "SPEECH OUTPUT: 'ToolStripTextBox control frame'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("KP_9"))
sequence.append(utils.AssertPresentationAction(
    "flatreview lndn (kp_9)",
        ["BRAILLE LINE:  'Your input:'",
         "     VISIBLE:  'Your input:', cursor=1",
         "SPEECH OUTPUT: 'Your input:'"]))
        
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("KP_7"))
sequence.append(KeyComboAction("KP_Divide"))
sequence.append(utils.AssertPresentationAction(
    "flatreview lnup (kp_7) and left mouse click (kp_divide) to activate input field",
        ["BRAILLE LINE:  '  '",
         "     VISIBLE:  '  ', cursor=1",
        "BRAILLE LINE:  ''",
        "     VISIBLE:  '', cursor=1",
        "BRAILLE LINE:  ''",
        "     VISIBLE:  '', cursor=1",
        "SPEECH OUTPUT: 'blank'",
        "SPEECH OUTPUT: ''",
        "SPEECH OUTPUT: 'text '"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("h"))
sequence.append(KeyComboAction("e"))
sequence.append(KeyComboAction("l"))
sequence.append(KeyComboAction("l"))
sequence.append(KeyComboAction("o"))
sequence.append(utils.AssertPresentationAction(
    "enter hello",
        ["BRAILLE LINE:  'h'",
         "     VISIBLE:  'h', cursor=2",
         "BRAILLE LINE:  'he'",
         "     VISIBLE:  'he', cursor=3",
         "BRAILLE LINE:  'hel'",
         "     VISIBLE:  'hel', cursor=4",
         "BRAILLE LINE:  'hell'",
         "     VISIBLE:  'hell', cursor=5",
         "BRAILLE LINE:  'hello'",
         "     VISIBLE:  'hello', cursor=6"]))
        
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("KP_9"))
sequence.append(utils.AssertPresentationAction(
    "flatreview lndn",
        ["BRAILLE LINE:  'Your input:hello'",
         "     VISIBLE:  'Your input:hello', cursor=1",
         "SPEECH OUTPUT: 'Your input:hello'"]))

sequence.append(utils.AssertionSummaryAction())

sequence.start()
