#!/usr/bin/python

from macaroon.playback import *
import utils

sequence = MacroSequence()

sequence.append(utils.StartRecordingAction())
sequence.append(WaitForWindowActivate("ErrorProvider control",None))
sequence.append(utils.AssertPresentationAction(
    "errorprovider frame active",
    ["BRAILLE LINE:  'ErrorProvider control Frame'",
      "     VISIBLE:  'ErrorProvider control Frame', cursor=1",
      "BRAILLE LINE:  ''",
      "     VISIBLE:  '', cursor=1",
      "SPEECH OUTPUT: 'ErrorProvider control frame'",
      "SPEECH OUTPUT: ''",
      "SPEECH OUTPUT: 'text '"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("m"))
sequence.append(KeyComboAction("o"))
sequence.append(KeyComboAction("n"))
sequence.append(KeyComboAction("o"))
sequence.append(utils.AssertPresentationAction(
    "enter the name mono",
    ["BRAILLE LINE:  'm'",
     "     VISIBLE:  'm', cursor=2",
     "BRAILLE LINE:  'mo'",
     "     VISIBLE:  'mo', cursor=3",
     "BRAILLE LINE:  'mon'",
     "     VISIBLE:  'mon', cursor=4",
     "BRAILLE LINE:  'mono'",
     "     VISIBLE:  'mono', cursor=5"]))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("KP_Subtract"))
sequence.append(utils.AssertPresentationAction(
    "switch to flat-review",
    ["BRAILLE LINE:  'Name: mono AlwaysBlink'",
     "     VISIBLE:  'Name: mono AlwaysBlink', cursor=10",
     "SPEECH OUTPUT: 'mono'"]))



sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Tab"))
sequence.append(utils.AssertPresentationAction(
    "jump to field age",
    ["BRAILLE LINE:  ''",
     "     VISIBLE:  '', cursor=1",
     "BRAILLE LINE:  ''",
     "     VISIBLE:  '', cursor=1",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'text '"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("KP_Subtract"))
sequence.append(utils.AssertPresentationAction(
    "switch to flat-review (age)",
    ["BRAILLE LINE:  'Age:  BlinkIfDifferentError'",
     "     VISIBLE:  'Age:  BlinkIfDifferentError', cursor=6",
     "SPEECH OUTPUT: 'blank'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Tab"))
sequence.append(utils.AssertPresentationAction(
    "jump to next filed - age is empty so an error_msg should be displayed",
    ["BRAILLE LINE:  'Age required'",
     "     VISIBLE:  'Age required', cursor=1",
    "SPEECH OUTPUT: 'Age required'"]))

sequence.append(utils.AssertionSummaryAction())

sequence.start()
