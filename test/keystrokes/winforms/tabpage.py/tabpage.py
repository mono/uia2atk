#!/usr/bin/python

from macaroon.playback import *
import utils

sequence = MacroSequence()


sequence.append(utils.StartRecordingAction())
sequence.append(WaitForWindowActivate("TabControl & TabPage control",None))
sequence.append(WaitForFocus("Tab 0", acc_role=pyatspi.ROLE_PAGE_TAB))
sequence.append(utils.AssertPresentationAction(
    "focus on tab0",
    ["BRAILLE LINE:  'TabControl & TabPage control Frame'",
     "     VISIBLE:  'TabControl & TabPage control Fra', cursor=1",
     "BRAILLE LINE:  'Tab 0",
     "     VISIBLE:  'Tab 0', cursor=1",
     "SPEECH OUTPUT: 'TabControl & TabPage control frame'",
     "SPEECH OUTPUT: 'Tab 0'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Right"))
sequence.append(WaitForFocus("Tab 1", acc_role=pyatspi.ROLE_PAGE_TAB))
sequence.append(utils.AssertPresentationAction(
    "switch focus to Tab 1",
    ["BRAILLE LINE:  'Tab 1'",
    "     VISIBLE:  'Tab 1', cursor=1",
    "SPEECH OUTPUT: 'Tab 1'"]))
    
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("KP_Subtract"))
sequence.append(utils.AssertPresentationAction(
    "flat-review tablist",
    ["BRAILLE LINE:  'Tab 0 Tab 1 Tab 2 Tab 3 Tab 4 Tab 5 Tab 6'",
    "     VISIBLE:  'Tab 0 Tab 1 Tab 2 Tab 3 Tab 4 Ta', cursor=1",
    "SPEECH OUTPUT: 'Tab'"]))


sequence.append(utils.AssertionSummaryAction())


sequence.start()
