#!/usr/bin/python

from macaroon.playback import *
import utils

sequence = MacroSequence()

sequence.append(utils.StartRecordingAction())
sequence.append(WaitForWindowActivate("ListBox control",None))
sequence.append(WaitForFocus("0", acc_role=pyatspi.ROLE_LIST_ITEM))
sequence.append(utils.AssertPresentationAction(
    "list focus",
    ["BRAILLE LINE:  'ListBox control Frame'",
     "     VISIBLE:  'ListBox control Frame', cursor=1",
     "BRAILLE LINE:  'List'",
      "     VISIBLE:  'List', cursor=1",
      "BRAILLE LINE:  '0 ListItem'",
      "     VISIBLE:  '0 ListItem', cursor=1",
      "SPEECH OUTPUT: 'ListBox control frame'",
      "SPEECH OUTPUT: ''",
      "SPEECH OUTPUT: 'list'",
      "SPEECH OUTPUT: ''",
      "SPEECH OUTPUT: '0'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("End"))
sequence.append(WaitForFocus("19", acc_role=pyatspi.ROLE_LIST_ITEM))
sequence.append(utils.AssertPresentationAction(
    "jump to last entry",
    ["BRAILLE LINE:  '19 ListItem'",
     "     VISIBLE:  '19 ListItem', cursor=1",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: '19'"]))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Home"))
sequence.append(WaitForFocus("0", acc_role=pyatspi.ROLE_LIST_ITEM))
sequence.append(utils.AssertPresentationAction(
    "jump back to first entry",
    ["BRAILLE LINE:  '0 ListItem'",
     "     VISIBLE:  '0 ListItem', cursor=1",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: '0'"]))
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("KP_7"))
sequence.append(utils.AssertPresentationAction(
    "lineup in flat-review kp_7",
    ["BRAILLE LINE:  'You select '",
     "     VISIBLE:  'You select ', cursor=1",
     "SPEECH OUTPUT: 'You select '",]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("5"))
sequence.append(WaitForFocus("5", acc_role=pyatspi.ROLE_LIST_ITEM))
sequence.append(utils.AssertPresentationAction(
    "jump to item 5",
    ["BRAILLE LINE:  '5 ListItem'",
     "     VISIBLE:  '5 ListItem', cursor=1",
     "BRAILLE LINE:  '5 ListItem'",
     "     VISIBLE:  '5 ListItem', cursor=1",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: '5'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("KP_Enter"))
sequence.append(utils.AssertPresentationAction(
    "ask: which item and how many",
    ["BRAILLE LINE:  '5 ListItem'",
     "     VISIBLE:  '5 ListItem', cursor=1",
     "SPEECH OUTPUT: 'list item'",
     "SPEECH OUTPUT: '5'",
     "SPEECH OUTPUT: 'item 6 of 30'"]))



sequence.append(utils.AssertionSummaryAction())

sequence.start()
