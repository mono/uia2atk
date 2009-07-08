#!/usr/bin/python

from macaroon.playback import *
import utils

sequence = MacroSequence()

sequence.append(utils.StartRecordingAction())
sequence.append(WaitForWindowActivate("Help Provider Demonstration",None))
sequence.append(utils.AssertPresentationAction(
    "helpprovider frame active",
    ["BRAILLE LINE:  'Help Provider Demonstration Frame'",
     "     VISIBLE:  'Help Provider Demonstration Frame', cursor=1",
     "BRAILLE LINE:  '1800 S Novell Place'",
     "     VISIBLE:  '1800 S Novell Place', cursor=1",
     "SPEECH OUTPUT: 'Help Provider Demonstration frame'",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'text 1800 S Novell Place'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("F1"))
sequence.append(utils.AssertPresentationAction(
    "show help info",
    ["BRAILLE LINE:  'Enter the city here.'",
     "     VISIBLE:  'Enter the city here.', cursor=1",
     "SPEECH OUTPUT: 'Enter the city here.'"]))

sequence.append(utils.AssertionSummaryAction())

sequence.start()
