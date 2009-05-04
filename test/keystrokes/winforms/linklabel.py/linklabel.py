#!/usr/bin/python

from macaroon.playback import *
import utils

sequence = MacroSequence()

sequence.append(utils.StartRecordingAction())
sequence.append(WaitForWindowActivate("LinkLabel control",None))
sequence.append(utils.AssertPresentationAction(
    "LinkLabel control frame active",
    ["BRAILLE LINE:  'LinkLabel control Frame'",
     "     VISIBLE:  'LinkLabel control Frame', cursor=1",
     "BRAILLE LINE:  'openSUSE: English Website'",
     "     VISIBLE:  'openSUSE: English Website', cursor=1",
     "SPEECH OUTPUT: 'LinkLabel control frame'",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'openSUSE: English Website\n \n  Novell Webmail label'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Down"))
sequence.append(utils.AssertPresentationAction(
    "next entry",
    ["BRAILLE LINE:  'calculator:/usr/bin/gcalctool'",
     "     VISIBLE:  'calculator:/usr/bin/gcalctool', cursor=1",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'calculator:/usr/bin/gcalctool label'"]))

sequence.append(utils.AssertionSummaryAction())

sequence.start()
