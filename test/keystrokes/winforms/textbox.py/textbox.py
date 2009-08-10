#!/usr/bin/python

from macaroon.playback import *
import utils

sequence = MacroSequence()

sequence.append(utils.StartRecordingAction())
sequence.append(WaitForWindowActivate("TextBox Control",None))
sequence.append(utils.AssertPresentationAction(
    "app active",
    ["BRAILLE LINE:  'TextBox Control Frame'",
     "     VISIBLE:  'TextBox Control Frame', cursor=1",
     "BRAILLE LINE:  ''",
     "     VISIBLE:  '', cursor=1",
     "SPEECH OUTPUT: 'TextBox Control frame'",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'text '"]))


sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("m"))
sequence.append(KeyComboAction("y"))
sequence.append(KeyComboAction("l"))
sequence.append(KeyComboAction("a"))
sequence.append(KeyComboAction("b"))
sequence.append(KeyComboAction("e"))
sequence.append(KeyComboAction("l"))
sequence.append(utils.AssertPresentationAction(
    "enter mylabel",
    ["BRAILLE LINE:  'm'",
      "     VISIBLE:  'm', cursor=2",
      "BRAILLE LINE:  'my'",
      "     VISIBLE:  'my', cursor=3",
      "BRAILLE LINE:  'myl'",
      "     VISIBLE:  'myl', cursor=4",
      "BRAILLE LINE:  'myla'",
      "     VISIBLE:  'myla', cursor=5",
      "BRAILLE LINE:  'mylab'",
      "     VISIBLE:  'mylab', cursor=6",
      "BRAILLE LINE:  'mylabe'",
      "     VISIBLE:  'mylabe', cursor=7",
      "BRAILLE LINE:  'mylabel'",
      "     VISIBLE:  'mylabel', cursor=8"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("KP_7"))
sequence.append(utils.AssertPresentationAction(
    "enter flat-review lnuip (KP_7)",
    ["BRAILLE LINE:  'explicitly set name for label'",
     "     VISIBLE:  'explicitly set name for label', cursor=1",
     "SPEECH OUTPUT: 'explicitly set name for label'"]))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Tab"))
sequence.append(utils.AssertPresentationAction(
    "jump to next ctrl",
    ["BRAILLE LINE:  ''",
     "     VISIBLE:  '', cursor=1",
    "SPEECH OUTPUT: ''",
    "SPEECH OUTPUT: 'text '"]))


sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("m"))
sequence.append(KeyComboAction("u"))
sequence.append(KeyComboAction("l"))
sequence.append(KeyComboAction("t"))
sequence.append(KeyComboAction("i"))
sequence.append(KeyComboAction("l"))
sequence.append(KeyComboAction("i"))
sequence.append(KeyComboAction("n"))
sequence.append(KeyComboAction("e"))
sequence.append(KeyComboAction("Return"))
sequence.append(KeyComboAction("Return"))
sequence.append(KeyComboAction("t"))
sequence.append(KeyComboAction("e"))
sequence.append(KeyComboAction("x"))
sequence.append(KeyComboAction("t"))
sequence.append(KeyComboAction("b"))
sequence.append(KeyComboAction("o"))
sequence.append(KeyComboAction("x"))
sequence.append(utils.AssertPresentationAction(
    "enter multiline<cr>textbox",
    ["BRAILLE LINE:  'm'",
    "     VISIBLE:  'm', cursor=2",
    "BRAILLE LINE:  'mu'",
    "     VISIBLE:  'mu', cursor=3",
    "BRAILLE LINE:  'mul'",
    "     VISIBLE:  'mul', cursor=4",
    "BRAILLE LINE:  'mult'",
    "     VISIBLE:  'mult', cursor=5",
    "BRAILLE LINE:  'multi'",
    "     VISIBLE:  'multi', cursor=6",
    "BRAILLE LINE:  'multil'",
    "     VISIBLE:  'multil', cursor=7",
    "BRAILLE LINE:  'multili'",
    "     VISIBLE:  'multili', cursor=8",
    "BRAILLE LINE:  'multilin'",
    "     VISIBLE:  'multilin', cursor=9",
    "BRAILLE LINE:  'multiline'",
    "     VISIBLE:  'multiline', cursor=10",
    "BRAILLE LINE:  ''",
    "     VISIBLE:  '', cursor=1",
    "BRAILLE LINE:  ''",
    "     VISIBLE:  '', cursor=1",
    "BRAILLE LINE:  't'",
    "     VISIBLE:  't', cursor=2",
    "BRAILLE LINE:  'te'",
    "     VISIBLE:  'te', cursor=3",
    "BRAILLE LINE:  'tex'",
    "     VISIBLE:  'tex', cursor=4",
    "BRAILLE LINE:  'text'",
    "     VISIBLE:  'text', cursor=5",
    "BRAILLE LINE:  'textb'",
    "     VISIBLE:  'textb', cursor=6",
    "BRAILLE LINE:  'textbo'",
    "     VISIBLE:  'textbo', cursor=7",
    "BRAILLE LINE:  'textbox'",
    "     VISIBLE:  'textbox', cursor=8"]))



sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("<Control>Tab"))
sequence.append(utils.AssertPresentationAction(
    "<ctrl+tab> - jump to next control",
    ["BRAILLE LINE:  ''",
     "     VISIBLE:  '', cursor=1",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'text '"]))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("a"))
sequence.append(KeyComboAction("b"))
sequence.append(KeyComboAction("c"))
sequence.append(utils.AssertPresentationAction(
    "enter abc",
    ["BRAILLE LINE:  'a'",
     "     VISIBLE:  'a', cursor=2",
     "BRAILLE LINE:  'ab'",
     "     VISIBLE:  'ab', cursor=3",
     "BRAILLE LINE:  'abc'",
     "     VISIBLE:  'abc', cursor=4"]))


sequence.append(utils.AssertionSummaryAction())

sequence.start()
